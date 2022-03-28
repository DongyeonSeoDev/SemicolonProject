using UnityEngine;
using System.Collections.Generic;
using System;
using Water;

public class StageManager : MonoSingleton<StageManager>
{
    private Dictionary<string, StageGround> idToStageObjDict = new Dictionary<string, StageGround>();
    private Dictionary<string, StageDataSO> idToStageDataDict = new Dictionary<string, StageDataSO>();
    private Dictionary<string, StageBundleDataSO> idToStageFloorDict = new Dictionary<string, StageBundleDataSO>();
    private Dictionary<int, Dictionary<AreaType, List<StageDataSO>>> randomRoomDict = new Dictionary<int, Dictionary<AreaType, List<StageDataSO>>>();

    private StageGround currentStage = null;
    private StageDataSO currentStageData = null;
    public StageDataSO CurrentStageData => currentStageData;

    private int currentFloor = 1;
    private int currentStageNumber = 0;
    private AreaType currentArea;
    public AreaType CurrentAreaType => currentArea;

    //private bool completeLoadNextMap; //다음 맵을 완전히 불러왔는지

    public DoorDirType PassDir { get; set; } //전 스테이지에서 지나간 문 방향

    [SerializeField] private int MaxStage;
    [SerializeField] private string startStageID;
    [HideInInspector] public Vector2 respawnPos;

    //public Sprite openDoorSpr, closeDoorSpr;
    public Sprite[] doorSprites;
    public Dictionary<string, Sprite> doorSprDic = new Dictionary<string, Sprite>();

    public GameObject recoveryObjPref;
    public GameObject imprecationObjPref;

    public Transform stageParent;
    public Transform npcParent;

    public bool IsStageClear { get; set; }
    public Vector3 MapCenterPoint { get; private set; }

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
        Util.DelayFunc(() => NextStage(startStageID), 0.2f);
        respawnPos = idToStageDataDict[startStageID].stage.GetComponent<StageGround>().playerSpawnPoint.position;

        PoolManager.CreatePool(recoveryObjPref, npcParent, 1, "RecoveryObjPrefObjPref1");
        PoolManager.CreatePool(imprecationObjPref, npcParent, 1, "ImprecationObjPref1");
    }

    private void DefineEvent()
    {
        EventManager.StartListening("Loading", () =>
        {
            PoolManager.PoolObjSetActiveFalse("RecoveryObjPrefObjPref1");
            PoolManager.PoolObjSetActiveFalse("ImprecationObjPref1");
            PoolManager.PoolObjSetActiveFalse("NormalPointLight2D");
        });
        EventManager.TriggerEvent("StartBGM", startStageID);
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("StartNextStage", StartNextStage);
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
        }

        foreach (AreaType type in Enum.GetValues(typeof(AreaType)))
        {
            randomRoomDict[floor][type] = randomRoomDict[floor][type].ToRandomList();
        }
    }

    public void NextStage(string id)
    {
        //현재 스테이지 옵젝을 꺼주고 다음 스테이지를 불러와서 켜주고 스테이지 번호를 1 증가시킴
        if (currentStage) currentStage.gameObject.SetActive(false);

        currentStageData = idToStageDataDict[id];
        currentArea = currentStageData.areaType;
        //IsLastStage = currentStageData.endStage;
        currentStage = null;

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
            SlimeGameManager.Instance.CurrentPlayerBody.transform.position = MapCenterPoint;
        }
        else
        {
            //다음 스테이지 가면 해당 방향에 맞게 담 스테이지의 문 근처에서 스폰이 되며 그 문은 지나간 상태의 스프라이트로 바꿈
            MapCenterPoint = currentStage.GetOpposeDoor(PassDir).playerSpawnPos.position;
            SlimeGameManager.Instance.CurrentPlayerBody.transform.position = MapCenterPoint;
        }
        
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
                EventManager.TriggerEvent("SpawnEnemy", currentStageData.stageID);
                break;
            case AreaType.CHEF:
                SetClearStage();
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

    public void SetClearStage()
    {
        IsStageClear = true;
        currentStage.OpenDoors();
    }

    public void StartNextStage()
    {
        if (currentArea == AreaType.MONSTER)
            EventManager.TriggerEvent("EnemyMove", currentStageData.stageID);
        EventManager.TriggerEvent("StartBGM", currentStageData.stageID);
        UIManager.Instance.InsertTopCenterNoticeQueue(string.IsNullOrEmpty(currentStageData.stageName) ? Global.AreaTypeToString(currentArea) : currentStageData.stageName);
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
        currentStageNumber = 0;
        InsertRandomMaps(currentFloor, false);
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
        RandomRoomType room = (RandomRoomType)UnityEngine.Random.Range(0, Global.EnumCount<RandomRoomType>());
        //랜덤맵일 때는 EventManager.TriggerEvent(Global.EnterNextMap)가 실행안되므로 저주나 회복일 땐 따로 부름. 몹 구역일 땐 어차피 NextStage로 호출함
        switch (room)
        {
            case RandomRoomType.IMPRECATION: //저주 구역
                currentArea = AreaType.IMPRECATION;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitImprecationArea(true);
                PoolManager.GetItem("ImprecationObjPref1").transform.position = currentStage.objSpawnPos.position;
                break;
            case RandomRoomType.MONSTER:  //몬스터 구역
                --currentStageNumber;
                int targetStage = Mathf.Clamp(currentStageData.stageFloor.floor + UnityEngine.Random.Range(-1, 2), 1, MaxStage); //현재 층에서 몇 층을 더할지 정함
                StageBundleDataSO sbData = idToStageFloorDict.Values.Find(x=>x.floor == targetStage); //현재 층에서 -1 or 0 or 1층을 더한 층을 가져온다
                NextStage(sbData.stages.FindRandom(stage => stage.areaType == AreaType.MONSTER).stageID); //뽑은 층에서 몬스터 지역들중에 랜덤으로 가져온다
                break;
            case RandomRoomType.RECOVERY:  //회복 구역
                currentArea = AreaType.RECOVERY;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitRecoveryArea(true);
                PoolManager.GetItem("RecoveryObjPrefObjPref1").transform.position = currentStage.objSpawnPos.position;
                PoolManager.GetItem("NormalPointLight2D").transform.position = currentStage.objSpawnPos.position;
                break;
        }
    }
}
