using UnityEngine;
using System.Collections.Generic;
using System;
using Water;

public class StageManager : MonoSingleton<StageManager>
{
    #region dictionary
    private Dictionary<string, StageGround> idToStageObjDict = new Dictionary<string, StageGround>();
    private Dictionary<string, StageDataSO> idToStageDataDict = new Dictionary<string, StageDataSO>();
    private Dictionary<string, StageBundleDataSO> idToStageFloorDict = new Dictionary<string, StageBundleDataSO>();
    private Dictionary<int, Dictionary<AreaType, List<StageDataSO>>> randomRoomDict = new Dictionary<int, Dictionary<AreaType, List<StageDataSO>>>();
    private Dictionary<string, NPC> npcDict = new Dictionary<string, NPC>();
    #endregion

    #region current
    private StageGround currentStage = null;
    private StageDataSO currentStageData = null;
    public StageDataSO CurrentStageData => currentStageData;
    public StageGround CurrentStageGround => currentStage;

    private int currentStageMonsterBundleOrder = 1;

    private int currentFloor = 1;
    private int currentStageNumber = 0;
    private AreaType currentArea = AreaType.NONE;
    public AreaType CurrentAreaType => currentArea;

    private List<NPC> currentMapNPCList = new List<NPC>();

    //private bool completeLoadNextMap; //다음 맵을 완전히 불러왔는지
    #endregion

    #region Map Values
    [SerializeField] private int MaxStage;
    [SerializeField] private string startStageID;
    [HideInInspector] public Vector2 respawnPos;

    //public Sprite openDoorSpr, closeDoorSpr;
    public Sprite[] doorSprites;
    public Dictionary<string, Sprite> doorSprDic = new Dictionary<string, Sprite>();
    #endregion

    #region obj
    public GameObject recoveryObjPref;
    public GameObject imprecationObjPref;
    #endregion

    #region parent
    public Transform stageParent;
    public Transform npcParent;
    #endregion

    public DoorDirType PassDir { get; set; } //전 스테이지에서 지나간 문 방향
    public bool IsStageClear { get; set; }
    public Vector3 MapCenterPoint { get; private set; }

    private string CurrentMonstersOrderID => currentStageData.stageMonsterBundleCount < currentStageMonsterBundleOrder ? string.Empty : currentStageData.stageMonsterBundleID[currentStageMonsterBundleOrder - 1];

    private Dictionary<int, List<RandomRoomType>> randomZoneTypeListDic = new Dictionary<int, List<RandomRoomType>>(); //랜덤 구역에서 나올 구역 타입들을 미리 넣어놓음
    private List<RandomRoomType> randomZoneRestTypes = new List<RandomRoomType>(); //랜덤구역에서 나올 지역 타입들 현재 남은 것
    private int prevRandRoomType = -1;  // 이전 랜덤 구역의 타입

    //public bool IsLastStage { get; set; } 

    /* #region ValuableForEditor
     [Header("Test")]
     public string stageSOFolderName;
     #endregion*/


    private void Awake()
    {
        foreach (StageDataSO data in Resources.LoadAll<StageDataSO>("Stage/SO/"))
        {
            idToStageDataDict.Add(data.stageID, data);
            data.SetStageMonsterBundleID();
        }
        foreach(StageBundleDataSO data in Resources.LoadAll<StageBundleDataSO>("Stage/SO/BundleSO"))
        {
            idToStageFloorDict.Add(data.id, data);
            data.SetStageDic();
        }

        int cnt = Global.EnumCount<AreaType>();
        foreach (StageBundleDataSO data in idToStageFloorDict.Values)
        {
            
            randomRoomDict.Add(data.floor, new Dictionary<AreaType, List<StageDataSO>>());
            for(int i=0; i<cnt; i++)
            {
                randomRoomDict[data.floor].Add((AreaType)i, new List<StageDataSO>());
            }

            randomZoneTypeListDic.Add(data.floor, new List<RandomRoomType>());
        }

        cnt = Global.EnumCount<DoorDirType>();
        for(int i=0; i<cnt; i++)
        {
            doorSprDic.Add(((DoorDirType)i).ToString() + "Close", doorSprites[i]);
            doorSprDic.Add(((DoorDirType)i).ToString() + "Open", doorSprites[i + cnt]);
            doorSprDic.Add(((DoorDirType)i).ToString() + "Exit", doorSprites[i + cnt * 2]);
        }

        EventManager.StartListening(Global.EnterNextMap, () =>
        {
            currentStageNumber++;
        });

        
    }

    private void Start()
    {
        Init();
        DefineEvent();
    }

    private void Init()
    {
        InsertRandomMaps(currentFloor, true);
        SetRandomAreaRandomIncounter();
        Util.DelayFunc(() => NextStage(startStageID), 0.2f);
        respawnPos = idToStageDataDict[startStageID].stage.GetComponent<StageGround>().playerSpawnPoint.position;

        PoolManager.CreatePool(recoveryObjPref, npcParent, 1, "RecoveryObjPrefObjPref1");
        PoolManager.CreatePool(imprecationObjPref, npcParent, 1, "ImprecationObjPref1");
    }

    private void DefineEvent()
    {
        EventManager.StartListening("ExitCurrentMap", () =>
        {
            switch(currentArea)
            {
                case AreaType.RECOVERY:
                    PoolManager.PoolObjSetActiveFalse("RecoveryObjPrefObjPref1");
                    break;
                case AreaType.IMPRECATION:
                    PoolManager.PoolObjSetActiveFalse("ImprecationObjPref1");
                    break;
                case AreaType.CHEF:
                    currentMapNPCList.ForEach(x => x.gameObject.SetActive(false));
                    currentMapNPCList.Clear();
                    break;
            }

            SoundManager.Instance.SetBGMPitch(1);
        });
        EventManager.TriggerEvent("StartBGM", startStageID);
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("StartNextStage", StartNextStage);
    }

    private void SetRandomAreaRandomIncounter()
    {
        StageBundleDataSO data = idToStageFloorDict[FloorToFloorID(currentFloor)];
        Dictionary<RandomRoomType, int> ranMapCnt = new Dictionary<RandomRoomType, int>();
        List<RandomRoomType> tempList = new List<RandomRoomType>();

        int cnt = data.stages.FindAll(x => x.areaType == AreaType.RANDOM).Count;
        int rrCnt = Global.EnumCount<RandomRoomType>();

        int q = cnt / rrCnt;
        int r = cnt % rrCnt;

        int i, count = 0;

        randomZoneTypeListDic[currentFloor].Clear();

        for (i = 0; i < rrCnt; i++)
        {
            for (int j = 0; j < q; j++)
            {
                tempList.Add((RandomRoomType)i);
                count++;
            }
            ranMapCnt.Add((RandomRoomType)i, q);
        }

        //랜덤맵 / 랜덤타입이 나누어떨어지지 않으면 임의의 랜덤맵을 나머지만큼 집어넣는데 겹치지 않게 함
        List<RandomRoomType> rList = new List<RandomRoomType>();
        if (r > 0)
        {
            for (i = 0; i < r; i++)
            {
                RandomRoomType rr = (RandomRoomType)UnityEngine.Random.Range(0, rrCnt);
                if (rList.Contains(rr))
                {
                    i--;
                }
                else
                {
                    rList.Add(rr);
                }
            }

            for (i = 0; i < rList.Count; i++)
            {
                tempList.Add(rList[i]);
            }
        }

        //남은 랜덤 방 수 간격이 rrCnt - 1미만으로 됐는지 체크
        Func<(bool, RandomRoomType)> checkInterval = () =>
        {
            int min = int.MaxValue;
            foreach(int i in ranMapCnt.Values)
            {
                if (i < min) min = i;
            }

            foreach (RandomRoomType r in ranMapCnt.Keys)
            {
                if (ranMapCnt[r] - min >= rrCnt - 1) return (false, r);
            }
            return (true, RandomRoomType.MONSTER); //두번째인자는 false일 때를 맞추기 위해서 그냥 한 거
        };

        int pre = -1;
        for(;;)
        {
            (bool, RandomRoomType) res = checkInterval();

            if (res.Item1)
            {
                int ran;
                do
                {
                    ran = UnityEngine.Random.Range(0, rrCnt);
                } while (pre == ran || ranMapCnt[(RandomRoomType)ran] == 0);

                pre = ran;
                randomZoneTypeListDic[currentFloor].Add((RandomRoomType)ran);
            }
            else
            {
                pre = (int)res.Item2;
                randomZoneTypeListDic[currentFloor].Add(res.Item2);
            }
            ranMapCnt[(RandomRoomType)pre]--;
          
            if (--count == 0)
                break;
        }
    }

    private string FloorToFloorID(int floor)
    {
        foreach(StageBundleDataSO data in idToStageFloorDict.Values)
        {
            if (data.floor == floor) return data.id;
        }
        return string.Empty;
    }

    private void InsertRandomMaps(int floor, bool init)
    {
        if (init)
        {
            //List<string> stageIDList = new List<string>();
            StageBundleDataSO bundle = idToStageFloorDict[FloorToFloorID(floor)]; //층 정보 받음


            /*foreach (StageDataSO data in bundle.stages)
            {
                stageIDList.Add(data.stageID);
            }*/
            foreach (AreaType type in Enum.GetValues(typeof(AreaType)))  //기존 랜덤 맵 초기화
            {
                randomRoomDict[floor][type].Clear();
            }

            /*for (int i = 0; i < bundle.randomStageList.Count; i++) //로직 바꿀거
            {
                for (int j = 0; j < bundle.randomStageList[i].nextStageTypes.Length; j++) //지금 한 방식대로라면 굳이 이렇게 할 필요 없지만 나중에 로직 바꿀 수도 있으니 일단 일케 함
                {
                    int rand;
                    StageDataSO data;
                    do
                    {
                        rand = Random.Range(0, stageIDList.Count);
                        data = idToStageDataDict[stageIDList[rand]];
                    } while (data.areaType != bundle.randomStageList[i].nextStageTypes[j]);
                    randomRoomDict[floor][bundle.randomStageList[i].nextStageTypes[j]].Add(data);
                    stageIDList.RemoveAt(rand);
                }
            }*/

            Dictionary<AreaType, List<string>> areaDic = new Dictionary<AreaType, List<string>>();
            for (int i = 0; i < Global.EnumCount<AreaType>(); i++) areaDic.Add((AreaType)i, new List<string>());
            for (int i = 0; i < bundle.stages.Count; i++)
            {
                areaDic[bundle.stages[i].areaType].Add(bundle.stages[i].stageID);
            }

            foreach (AreaType key in areaDic.Keys)
            {
                for (int i = 0; i < areaDic[key].Count; i++)
                {
                    randomRoomDict[floor][key].Add(idToStageDataDict[areaDic[key][i]]);
                }
            }
        }  //end of init

        foreach (AreaType type in Enum.GetValues(typeof(AreaType)))
        {
            randomRoomDict[floor][type] = randomRoomDict[floor][type].ToRandomList();
        }

        /*randomZoneRestTypes.Clear();
        for(int i=0; i<randomZoneTypeListDic[currentFloor].Count; i++)
        {
            randomZoneRestTypes.Add(randomZoneTypeListDic[currentFloor][i]);
        }
        randomZoneRestTypes = randomZoneRestTypes.ToRandomList(15);*/
    }

    public void NextStage(string id)
    {
        //EventManager.TriggerEvent("ExitStage");
        EventManager.TriggerEvent("ExitCurrentMap");

        //현재 스테이지 옵젝을 꺼주고 다음 스테이지를 불러와서 켜주고 스테이지 번호를 1 증가시킴
        if (currentStage) currentStage.gameObject.SetActive(false);

        currentStageData = idToStageDataDict[id];
        currentArea = currentStageData.areaType;
        //IsLastStage = currentStageData.endStage;
        currentStage = null;
        currentStageMonsterBundleOrder = 1;

        if (!idToStageObjDict.TryGetValue(id, out currentStage))
        {
            currentStage = Instantiate(currentStageData.stage, transform.position, Quaternion.identity, stageParent).GetComponent<StageGround>();
            idToStageObjDict.Add(id, currentStage);
        }
        else
        {
            currentStage.gameObject.SetActive(true);
        }

        //스테이지의 문을 초기화
        currentStage.stageDoors.ForEach(x => 
        { 
            x.gameObject.SetActive(false);
            x.IsExitDoor = false;
        });

        //Player Position
        if(currentStage.playerSpawnPoint)
        {
            //주로 처음 게임 스타트 지점
            MapCenterPoint = currentStage.playerSpawnPoint.position;
            
        }
        else
        {
            //다음 스테이지 가면 해당 방향에 맞게 담 스테이지의 문 근처에서 스폰이 되며 그 문은 지나간 상태의 스프라이트로 바꿈
            MapCenterPoint = currentStage.GetOpposeDoor(PassDir).playerSpawnPos.position;
            
        }

        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = MapCenterPoint;
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);  //Camera Move Range Set
        GameManager.Instance.ResetDroppedItems(); //Inactive Items

        //다음 스테이지 경우의 수만큼 문을 켜주고 문에 다음 스테이지 타입에 맞게 랜덤으로 다음 스테이지 설정
        int idx = 0;
        int count = currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes.Length;
        currentStage.stageDoors.ToRandomList(5).ForEach(door =>
        {
            if(!door.gameObject.activeSelf && idx < count)
            {
                try
                {
                    door.nextStageData = randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[idx]][0];
                    randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[idx]].RemoveAt(0);
                    randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[idx]].Add(door.nextStageData);
                    ++idx;
                    door.gameObject.SetActive(true);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.Log($"스테이지를 불러오지 못함 {currentFloor} - {currentStageNumber} : idx: {idx}");
                }
            }
        });

        //해당 스테이지의 타입에 따라 무언가를 함
        switch (currentStageData.areaType)
        {
            case AreaType.START:
                currentStage.stageDoors.ForEach(x => { x.gameObject.SetActive(true); x.IsExitDoor = false; });
                SetClearStage();
                break;
            case AreaType.MONSTER:
                //EventManager.TriggerEvent("SpawnEnemy", currentStageData.stageID);
                break;
            case AreaType.CHEF:
                SetClearStage();
                ChefStage();
                break;
            case AreaType.PLANTS:
                SetClearStage();
                break;
            case AreaType.RANDOM:
                IsStageClear = false;
                EnterRandomArea();
                return;   //랜덤 맵이면 함수를 빠져나간다.
            case AreaType.BOSS:
                //보스구역이면 무엇을 할까
                break;
        }

        EventManager.TriggerEvent(Global.EnterNextMap);
    }

    public void SetMonsterStage()
    {
        IsStageClear = false;
        currentStage.CloseDoor();
    }

    public void NextEnemy()
    {
        if (currentArea == AreaType.MONSTER)
        {
            string id = CurrentMonstersOrderID;

            if(string.IsNullOrEmpty(id))
            {
                StageClear();
                return;
            }

            Util.DelayFunc(() =>
            {
                EventManager.TriggerEvent("SpawnEnemy", id);
                EventManager.TriggerEvent("EnemyMove", id);
            }, 2, this);
            currentStageMonsterBundleOrder++;
        }
    }

    public void SetClearStage()
    {
        IsStageClear = true;
        currentStage.OpenDoors();
    }

    public void StartNextStage()
    {
        if (currentArea == AreaType.MONSTER)
        {
            SetMonsterStage();
            NextEnemy();
        }
        EventManager.TriggerEvent("StartBGM", currentStageData.stageID);

        string str = string.IsNullOrEmpty(currentStageData.stageName) ? Global.AreaTypeToString(currentArea) : currentStageData.stageName;
        if (currentArea != AreaType.BOSS)
        {
            UIManager.Instance.InsertTopCenterNoticeQueue(str);
        }
        else
        {
            UIManager.Instance.InsertTopCenterNoticeQueue(str, UIManager.Instance.bossNoticeMsgVGrd);
        }
    }

    public void StageClear()
    {
        SetClearStage();

        EventManager.TriggerEvent("StageClear");

        //CinemachineCameraScript.Instance.SetCinemachineConfiner(CinemachineCameraScript.Instance.boundingCollider);

        /*if (IsLastStage)
        {
            EventManager.TriggerEvent("GameClear");
        }*/
        if(currentArea==AreaType.BOSS)
        {
            EventManager.TriggerEvent("GameClear");
        }
    }

    private void Respawn()
    {
        //prevRandRoomType = -1;
        currentStageNumber = 0;
        InsertRandomMaps(currentFloor, false);
        SetRandomAreaRandomIncounter();
        NextStage(startStageID);
    }

    public StageDataSO GetStageData(string id = "")
    {
        if (string.IsNullOrEmpty(id)) id = startStageID;
        if (idToStageDataDict.ContainsKey(id))
        {
            return idToStageDataDict[id];
        }
        else
        {
            Debug.Log("존재하지 않는 스테이지 아이디 : " + id);
            return null;
        }
    }

    private void EnterRandomArea()
    {
        //RandomRoomType room = (RandomRoomType)UnityEngine.Random.Range(0, Global.EnumCount<RandomRoomType>());
        //room = RandomRoomType.IMPRECATION;
        //랜덤맵일 때는 EventManager.TriggerEvent(Global.EnterNextMap)가 실행안되므로 저주나 회복일 땐 따로 부름. 몹 구역일 땐 어차피 NextStage로 호출함

        RandomRoomType room;
        if (randomZoneRestTypes.Count > 0)
        {
            //이전과 겹치지 않게 해줌.
            int rand;
            bool oneType = IsRestRandomAreaOneType();
            do
            {
                rand = UnityEngine.Random.Range(0, randomZoneRestTypes.Count);
            } while ((int)randomZoneRestTypes[rand] == prevRandRoomType && !oneType);

            room = randomZoneRestTypes[rand];
            randomZoneRestTypes.RemoveAt(rand);
            prevRandRoomType = (int)room;
        }
        else //혹시 모를 예외처리
        {
            room = (RandomRoomType)UnityEngine.Random.Range(0, Global.EnumCount<RandomRoomType>());
        }

        switch (room)
        {
            case RandomRoomType.IMPRECATION: //저주 구역
                currentArea = AreaType.IMPRECATION;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitImprecationArea(true);
                SoundManager.Instance.SetBGMPitchByLerp(1, -0.7f, 1f);
                PoolManager.GetItem("ImprecationObjPref1").transform.position = currentStage.objSpawnPos.position;
                break;

            case RandomRoomType.MONSTER:  //몬스터 구역
                //--currentStageNumber;
                int targetStage = Mathf.Clamp(currentStageData.stageFloor.floor + UnityEngine.Random.Range(-1, 2), 1, MaxStage); //현재 층에서 몇 층을 더할지 정함
                StageBundleDataSO sbData = idToStageFloorDict.Values.Find(x=>x.floor == targetStage); //현재 층에서 -1 or 0 or 1층을 더한 층을 가져온다
                NextStage(sbData.stages.FindRandom(stage => stage.areaType == AreaType.MONSTER).stageID); //뽑은 층에서 몬스터 지역들중에 랜덤으로 가져온다
                break;

            case RandomRoomType.RECOVERY:  //회복 구역
                currentArea = AreaType.RECOVERY;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitRecoveryArea(true);
                PoolManager.GetItem("RecoveryObjPrefObjPref1").transform.position = currentStage.objSpawnPos.position;

                if (StateManager.Instance.IsPlayerFullHP && StateManager.Instance.IsPlayerNoImpr)
                {
                    SetClearStage();
                }
                break;
        }
    }

    private bool IsRestRandomAreaOneType() //남은 랜덤 구역 타입이 한 타입밖에 존재하지 않은지 체크
    {
        RandomRoomType type = randomZoneRestTypes[0];

        for(int i=1; i<randomZoneRestTypes.Count; i++)
        {
            if (type != randomZoneRestTypes[i]) return false;
        }

        return true;
    }

    private NPC GetNPC(string id)
    { 
        if(npcDict.ContainsKey(id)) return npcDict[id];

        NPC npc = Instantiate(Resources.Load<GameObject>("Prefabs/NPC/" + id), transform).GetComponent<NPC>();
        npcDict.Add(npc.npcId, npc);
        return npc;
    }

    private void ChefStage()
    {
        NPC npc = GetNPC(currentStageData.mapNPC.name);
        npc.gameObject.SetActive(true);
        npc.transform.position = currentStage.objSpawnPos.position;
        currentMapNPCList.Add(npc);
    }
}
