using UnityEngine;
using System.Collections.Generic;
using System;
using Water;
using FkTweening;
using System.IO;
using UnityEngine.Tilemaps;
using Enemy;

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

    [HideInInspector] public int enemyKill = 0;  //현재 스테이지에서 죽인 적의 수
    private int enemyCount = -1;  //현재 스테이지에서 나오는 모든 적의 수 (1세트 2세트로 나오는거 다 포함)
    private int currentStageMonsterBundleOrder = 1;

    private int currentFloor = 1;
    public int CurrentFloor => currentFloor;
    private int currentStageNumber = 0;
    public int CurrentStageNumber => currentStageNumber;
    private AreaType currentArea = AreaType.NONE;
    public AreaType CurrentAreaType => currentArea;

    private List<NPC> currentMapNPCList = new List<NPC>();

    //private bool completeLoadNextMap; //다음 맵을 완전히 불러왔는지
    #endregion

    #region Map Values
    [SerializeField] private int maxStage;
    [SerializeField] private string startStageID;

    public List<Single<Sprite[]>> doorSpriteList;
    public Dictionary<int, Dictionary<string, Sprite>> doorSprDic = new Dictionary<int, Dictionary<string, Sprite>>();
    private bool[] floorInitSet;
    #endregion

    #region obj
    public GameObject recoveryObjPref;
    public GameObject imprecationObjPref;
    #endregion

    #region parent
    public Transform stageParent;
    public Transform npcParent;
    #endregion

    #region Map Incounter
    [Space(15)]
    private Dictionary<int, Dictionary<AreaType, float>> areaWeightDic = new Dictionary<int, Dictionary<AreaType, float>>();
    private Dictionary<int, Dictionary<EnemyType, float>> mobAreaWeightDic = new Dictionary<int, Dictionary<EnemyType, float>>();
    [Header("[Incounter Variables]")]
    [SerializeField] private List<Pair<int, List<EnemyType>>> floorSpecies;  //층마다 어떤 몹들 나오는지
    [SerializeField] private List<Pair<AreaType, float>> areaWeight;  //구역마다 기본 가중치
    [SerializeField] private float mobWeight = 15; //몬스터당 기본 가중치 

    private bool isSelectRandomArea = false;
    #endregion

    public Func<bool> canNextStage = null;  //다음 스테이지를 갈 때 이게 null이면 가지고 무언가 값이 있으면 그것을 실행해서 true면 지나가짐

    public DoorDirType PassDir { get; set; } //전 스테이지에서 지나간 문 방향
    public bool IsBattleArea => currentArea == AreaType.MONSTER || currentArea == AreaType.BOSS;
    public bool IsFighting => IsBattleArea && !IsStageClear;
    public bool IsStageClear { get; set; }
    public Vector3 MapCenterPoint { get; private set; }

    public event Action NextStagePreEvent;

    private string CurrentMonstersOrderID => currentStageData.stageMonsterBundleCount < currentStageMonsterBundleOrder ? string.Empty : currentStageData.stageMonsterBundleID[currentStageMonsterBundleOrder - 1];

    private Dictionary<int, List<RandomRoomType>> randomZoneTypeListDic = new Dictionary<int, List<RandomRoomType>>(); //랜덤 구역에서 나올 구역 타입들을 미리 넣어놓음

    private Dictionary<string, StageData> stageDataDictionary = new Dictionary<string, StageData>();
    private string stageDataPath = Path.Combine("Enemy", "StageData", "StageData");

    public LinkedListNode<string> s;
    public LinkedList<string> ss;

    private void Awake()
    {
        //맵 데이터
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

        //스테이지 데이터
        string jsonData = Resources.Load(stageDataPath).ToString();
        var stageData = JsonUtility.FromJson<JsonParse<StageData>>(jsonData);

        for (int i = 0; i < stageData.jsonData.Count; i++)
        {
            stageDataDictionary.Add(stageData.jsonData[i].stageName, stageData.jsonData[i]);
        }

        //맵 인카운터 준비 + 스테이지 로직 (신)
        float[] mobStageWeight = new float[maxStage];
        for(int i=0; i<maxStage; i++)
        {
            mobStageWeight[i] = 0;
            for(int j=0; j<floorSpecies[i].second.Count; j++)
            {
                mobStageWeight[i] += mobWeight;
            }
        }

        for(int i=0; i<maxStage; i++)
        {
            areaWeightDic.Add(i + 1 , new Dictionary<AreaType, float>());
            mobAreaWeightDic.Add(i + 1, new Dictionary<EnemyType, float>());

            for(int j=0; j<areaWeight.Count; j++)
            {
                areaWeightDic[i + 1][areaWeight[j].first] = areaWeight[j].second;
            }
            areaWeightDic[i + 1][AreaType.MONSTER] = mobStageWeight[i];
            for(int j=0; j<floorSpecies[i].second.Count; j++)
            {
                mobAreaWeightDic[i + 1][floorSpecies[i].second[j]] = mobWeight;
            }
        }

        //스테이지 로직 준비 (구) + 랜덤구역 데이터 세팅
        int cnt = Global.EnumCount<AreaType>();
        foreach (StageBundleDataSO data in idToStageFloorDict.Values)
        {
            
            randomRoomDict.Add(data.floor, new Dictionary<AreaType, List<StageDataSO>>());
            for(int i=0; i<cnt; i++)
            {
                randomRoomDict[data.floor].Add((AreaType)i, new List<StageDataSO>());
            }

            randomZoneTypeListDic.Add(data.floor, new List<RandomRoomType>());  //랜덤구역 키 값 세팅
        }

        //문 스프라이트 세팅
        cnt = Global.EnumCount<DoorDirType>();
        int cnt2 = cnt * 2;
        for(int i=0; i<= maxStage; i++)
        {
            doorSprDic.Add(i, new Dictionary<string, Sprite>());
            for(int j=0; j<cnt; j++)
            {
                doorSprDic[i].Add(((DoorDirType)j).ToString() + "Close", doorSpriteList[i].value[j]);
                doorSprDic[i].Add(((DoorDirType)j).ToString() + "Open", doorSpriteList[i].value[j+cnt]);
                doorSprDic[i].Add(((DoorDirType)j).ToString() + "Exit", doorSpriteList[i].value[j+cnt2]);
            }
        }

        EventManager.StartListening(Global.EnterNextMap, () =>  //해당 방을 입장했을 때, 마지막에 호출
        {
            currentStageNumber++;
            isSelectRandomArea = false;
        });

        floorInitSet = new bool[maxStage + 1];  //튜토리얼 (0스테이지) 포함하므로 +1
        for(int i=0; i<floorInitSet.Length; i++)
            floorInitSet[i] = false;
    }

    private void Start()
    {
        Init();
        DefineEvent();

#if UNITY_EDITOR

        GameManager.Instance.testKeyInputActionDict.Add(KeyCode.F10, () =>
        {
            if (IsStageClear)
            {
                UIManager.Instance.RequestSystemMsg("이미 문 열려있다");
                return;
            }

           SetClearStage();
            UIManager.Instance.RequestSystemMsg("강제로 문을 열음");
            EnemyManager.Instance.PlayerDeadEvent();
        });

        GameManager.Instance.testKeyInputActionDict.Add(KeyCode.F11, () =>
        {
            foreach(AreaType key in areaWeightDic[currentFloor].Keys)
            {
                Debug.Log($"{key} : {areaWeightDic[currentFloor][key]}");
            }
        });

        GameManager.Instance.testKeyInputActionDict.Add(KeyCode.F12, () =>
        {
            foreach(EnemyType key in mobAreaWeightDic[currentFloor].Keys)
            {
                Debug.Log($"{key} : {mobAreaWeightDic[currentFloor][key]}");
            }
        });

        GameManager.Instance.testKeyInputActionDict.Add(KeyCode.Equals, () =>
        {
            foreach(AreaType key in randomRoomDict[currentFloor].Keys)
            {
                Debug.Log(key.ToString());
                foreach(StageDataSO data in randomRoomDict[currentFloor][key])
                {
                    Debug.Log(data.stageID);
                }
            }
        });
#endif
    }

    private void Init()
    {
        startStageID = GameManager.Instance.savedData.stageInfo.currentStageID;
        PassDir = GameManager.Instance.savedData.stageInfo.passDoorDir;
        if (TutorialManager.Instance.IsTestMode) startStageID = "Stage1-00";
        currentFloor = GetStageData(startStageID).stageFloor.floor;
        InsertRandomMaps(currentFloor);
        SetRandomAreaRandomIncounter();
        Util.DelayFunc(() =>
        {
            NextStage(startStageID, true);
            UIManager.Instance.StartLoadingIn();
        }, 0.2f);

        PoolManager.CreatePool(recoveryObjPref, npcParent, 1, "RecoveryObjPrefObjPref1");
        PoolManager.CreatePool(imprecationObjPref, npcParent, 1, "ImprecationObjPref1");
    }

    private void DefineEvent()
    {
        EventManager.StartListening("ExitCurrentMap", () =>   //맵을 나갈 때 나가기 전의 처리
        {
            switch(currentArea)
            {
                case AreaType.RECOVERY:
                    PoolManager.PoolObjSetActiveFalse("RecoveryObjPrefObjPref1");
                    Util.StopCo("SetRecoveryEnv", this);
                    Util.StopCo("RecoveryInteract", this);
                    break;
                case AreaType.IMPRECATION:
                    PoolManager.PoolObjSetActiveFalse("ImprecationObjPref1");
                    Util.StopCo("SetImprecationEnv", this);
                    Util.StopCo("ImprecationInteract", this);
                    break;
                case AreaType.CHEF:
                    ClearNPC();
                    break;
                case AreaType.STAT:
                    ClearNPC();
                    break;
                case AreaType.BOSS:
                    SkillUIManager.Instance.SetEnableSlot(SkillType.SPECIALATTACK2, true);
                    break;

            }

            SoundManager.Instance.SetBGMPitch(1);
            DOUtil.StopCo("Next Enemys Spawn", this);
            EnemyManager.Instance.PlayerDeadEvent();

            if (canNextStage != null) canNextStage = null;
        });
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("StartNextStage", StartNextStage);
        EventManager.StartListening("PickupMiniGame", (Action<bool>)(start => currentStage.StageLightActive(!start)));
        EventManager.StartListening("GameClear", () =>
        {
            Debug.Log("Game Clear : " + currentFloor + "-" + currentStageNumber);
        });
        EventManager.TriggerEvent("StartBGM", startStageID);
    }

    private void SetRandomAreaRandomIncounter()  //랜덤구역 랜덤 인카운터
    {
        if (currentFloor == 0) return;

        StageBundleDataSO data = idToStageFloorDict[FloorToFloorID(currentFloor)];
        Dictionary<RandomRoomType, int> ranMapCnt = new Dictionary<RandomRoomType, int>();

        int cnt = data.stages.FindAll(x => x.areaType == AreaType.RANDOM).Count;
        int rrCnt = Global.EnumCount<RandomRoomType>();

        int q = cnt / rrCnt;
        int r = cnt % rrCnt;

        int i, count = 0;

        randomZoneTypeListDic[currentFloor].Clear();

        count = rrCnt * q;
        for (i = 0; i < rrCnt; i++)
        {
            ranMapCnt.Add((RandomRoomType)i, q);
        }

        //랜덤맵 / 랜덤타입이 나누어떨어지지 않으면 임의의 랜덤맵을 나머지만큼 집어넣는데 겹치지 않게 함
        List<RandomRoomType> rList = new List<RandomRoomType>();
        
        if (r > 0)
        {
            rList = ranMapCnt.Keys.ToList().ToRandomList();

            int rr = rrCnt - r;
            for (i = 0; i < rr; i++) rList.RemoveAt(0);
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

        //나머지 남은 방들을 랜덤한 위치에 집어넣음
        int chk = 0;  //너무 많이 for문 도는지 체크하기 위한 변수
        for(i=0; i<r; i++)
        {
            if (++chk > 40) Debug.LogWarning("Too many iterations");

            pre = UnityEngine.Random.Range(0, randomZoneTypeListDic[currentFloor].Count);

            if(pre > 1 && randomZoneTypeListDic[currentFloor][pre-1] == rList[i])
            {
                if(randomZoneTypeListDic[currentFloor][pre - 2] == rList[i]) //넣은 위치로부터 뒤에 두 개가 같은 맵이고 넣을 맵도 같은 맵이면
                {
                    i--;
                    continue;
                }
            }

            // 0 1 2 3 4 5 리스트에 insert(4, 100)을 하면 0 1 2 3 100 4 5가 됨
            if(pre < randomZoneTypeListDic[currentFloor].Count - 1 && randomZoneTypeListDic[currentFloor][pre] == rList[i])
            {
                if(randomZoneTypeListDic[currentFloor][pre + 1] == rList[i]) //넣고나서 앞에 두 개가 같은 맵이고 넣을 맵도 같은 맵이면
                {
                    i--;
                    continue;
                }
            }
            
            if(pre > 0 && randomZoneTypeListDic[currentFloor][pre - 1] == rList[i])
            {
                if(randomZoneTypeListDic[currentFloor][pre] == rList[i]) //넣은 위치에서 앞과 뒤가 같은 맵이고 넣을 맵도 같은 맵이면
                {
                    i--;
                    continue;
                }
            }

            randomZoneTypeListDic[currentFloor].Insert(pre, rList[i]);
        }
    }

    public Sprite GetDoorSprite(DoorDirType ddt, string state) => doorSprDic[currentFloor][ddt.ToString() + state];

    private string FloorToFloorID(int floor)
    {
        foreach(StageBundleDataSO data in idToStageFloorDict.Values)
        {
            if (data.floor == floor) return data.id;
        }
        return string.Empty;
    }

    private void InsertRandomMaps(int floor)  //맵 리스트를 섞어줌
    {
        if (currentFloor == 0) return;

        if (!floorInitSet[floor])
        {
            floorInitSet[floor] = true; 
            StageBundleDataSO bundle = idToStageFloorDict[FloorToFloorID(floor)]; //층 정보 받음

            foreach (AreaType type in Enum.GetValues(typeof(AreaType)))  //기존 랜덤 맵 초기화  (안해도 되긴하지만 일단 걍 함)
            {
                randomRoomDict[floor][type].Clear();
            }

            for (int i = 0; i < bundle.stages.Count; i++)
            {
                randomRoomDict[floor][bundle.stages[i].areaType].Add(idToStageDataDict[bundle.stages[i].stageID]);
            }
        }  

        foreach (AreaType type in Enum.GetValues(typeof(AreaType)))
        {
            if (type == AreaType.START || type == AreaType.LOBBY) continue;

            randomRoomDict[floor][type] = randomRoomDict[floor][type].ToRandomList();
        }
    }

    //다음 스테이지로 감. (검은색 로딩 화면 중에 이루어지는 함수)
    public void NextStage(string id, bool teleport)
    {
        Debug.Log("Next Stage : " + id);

        EventManager.TriggerEvent("ExitCurrentMap");

        //현재 스테이지 옵젝을 꺼주고 다음 스테이지를 불러와서 켜주고 스테이지 번호를 1 증가시킴
        if (currentStage) currentStage.gameObject.SetActive(false);

        currentStageData = idToStageDataDict[id];
        currentArea = currentStageData.areaType;
        currentStage = null;
        currentStageMonsterBundleOrder = 1;

        enemyKill = 0;
        enemyCount = -1;

        if(currentStageData.stageFloor.floor != currentFloor) //다음 스테이지가 층이 다르면 새로 세팅 해야함
        {
            currentStageNumber = 0;
            currentFloor++;
            InsertRandomMaps(currentFloor);
            SetRandomAreaRandomIncounter();

            Debug.Log("Floor UP : " + currentFloor);
        }

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
        else if (currentStage.lobbySpawnPoint && teleport)  //주로 로비에서 부활하거나 시작했을 때
        {
            MapCenterPoint = currentStage.lobbySpawnPoint.position;
            currentStage.GetOpposeDoor(PassDir);
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

        UpdateAreaWeights();  //다음 문의 구역을 지정하기 전에 확률을 갱신시켜준다.

        if (currentStageNumber + 1 < currentStageData.stageFloor.LastStageNumber - 2)
        {
            SetNextDoors(door => true);
        }
        else if (currentStageNumber + 1 == currentStageData.stageFloor.LastStageNumber - 2)
        {
            SetNextDoors(door => door.dirType != DoorDirType.BACK);
        }
        else if (currentStageNumber + 1 == currentStageData.stageFloor.LastStageNumber - 1)
        {
            SetNextDoors(door => door.dirType == DoorDirType.FRONT);
        }
        else
        {
            //Last Stage
            Debug.Log("Last Stage : " + currentStageData.stageID);
            
            if (currentFloor > 0)
            {
                currentStage.stageDoors.ForEach(door =>
                {
                    if (!door.IsExitDoor)
                    {
                        if (door.dirType != DoorDirType.FRONT)
                            door.gameObject.SetActive(false);
                        else
                        {
                            door.nextStageData = currentStageData.stageFloor.nextStageSOInBoss;
                            door.gameObject.SetActive(true);
                        }
                    }
                });
            }
            else  //튜토리얼 스테이지면
            {
                currentStage.stageDoors.ForEach(door =>
                {
                    if (!door.gameObject.activeSelf)
                    {
                        door.gameObject.SetActive(true);
                    }
                });
            }
        }

        CheckStageBug();

        if (currentStageData.isSaveStage)
        {
            SaveStage();
        }

        {
            //해당 스테이지의 타입에 따라 무언가를 함  (해당 방을 입장했을 때)
            switch (currentStageData.areaType)
            {
                case AreaType.START:
                    if (currentStage.stageDoors.Length == 1)
                    {
                        currentStage.stageDoors[0].gameObject.SetActive(true);
                        currentStage.stageDoors[0].IsExitDoor = false;
                    }

                    if(currentStageData.mapEvent == null)
                    {
                        SetClearStage();
                    }
                    else
                    {
                        IsStageClear = false;
                    }

                    break;
                case AreaType.MONSTER:
                    SetMonsterStage();
                    break;
                case AreaType.CHEF:
                    SetClearStage();
                    ChefStage();
                    break;
                case AreaType.PLANTS:
                    SetClearStage();
                    break;
                case AreaType.STAT:
                    SetClearStage();
                    ChefStage();
                    StatStore.Instance.EnteredStatArea();
                    break;
                case AreaType.RANDOM:
                    SetMonsterStage();
                    IsStageClear = false;
                    EnterRandomArea();
                    return;   //랜덤 맵이면 함수를 빠져나간다.
                case AreaType.BOSS:
                    SetMonsterStage();
                    SkillUIManager.Instance.SetEnableSlot(SkillType.SPECIALATTACK2, false);
                    break;
            }

            SetEnemyManagerStageData(id);

            EventManager.TriggerEvent(Global.EnterNextMap);  //해당 방 입장 후에 필요한 이벤트들 처리
        }
    }

    private void ResetMapWeight(int floor)  //구역마다 가중치를 초깃값으로 리셋
    {
        float mobStageWeight = 0f;
        int i;
        for (i = 0; i < floorSpecies[floor-1].second.Count; i++)
        {
            mobStageWeight += mobWeight;
        }

        for (i = 0; i < areaWeight.Count; i++)
        {
            areaWeightDic[floor][areaWeight[i].first] = areaWeight[i].second;
        }
        areaWeightDic[floor][AreaType.MONSTER] = mobStageWeight;
        for (i = 0; i < floorSpecies[floor-1].second.Count; i++)
        {
            mobAreaWeightDic[floor][floorSpecies[floor-1].second[i]] = mobWeight;
        }
    }

    private AreaType GetRandomArea(List<AreaType> list)  //다음 구역을 가중치에 맞게 리스트에 없는 구역응로 랜덤 반환
    {
        List<AreaType> li = areaWeightDic[currentFloor].Keys.ToList();
        if (currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes.Length == 1)
        {
            Debug.Log("Next Door Count : 1");
            return currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[0];
        }
        float total = 0, weight = 0;
        int i;
        for (i = 0; i < li.Count; i++)
            total += areaWeightDic[currentFloor][li[i]];

        for(i=0; i<list.Count; i++)
        {
            if (list[i] == AreaType.MONSTER) continue;

            li.Remove(list[i]);
            total -= areaWeightDic[currentFloor][list[i]];
        }

        float sel = total * UnityEngine.Random.value;

        for (i = 0; i < li.Count; i++)
        {
            weight += areaWeightDic[currentFloor][li[i]];
            if (sel < weight)
            {
                return li[i];
            }
        }

        Debug.Log("문제 발생 : 확인 필요");
        Debug.Log($"total : {total}, sel : {sel}, weight : {weight}");
        for (i = 0; i < li.Count; i++) Debug.Log(li[i].ToString());
        for (i = 0; i < list.Count; i++) Debug.Log(list[i].ToString());
        return GetRandomArea(list); 
    }

    #region Weight Update

    private void UpdateAreaWeights()  //들어간 현재 구역에 맞게 구역들마다 가중치를 새로 갱신시켜줌
    {
        if (isSelectRandomArea) return; //랜덤 구역이 선택되고 몬스터 구역으로 정해지면 NextStage 함수가 두 번 호출되면서 이 함수도 두 번 호출되는 것을 막아줌
        if (currentFloor > 0)
        {
            AreaType type = currentStageData.areaType;
            if (areaWeightDic[currentFloor].ContainsKey(type))
            {
                float w = areaWeightDic[currentFloor][type]; //가중치를 저장해두는 변수
                float rate, rate2;
                switch (type)  //로직이 구역마다 어느 비율로 어떤 구역들이 얼마나 내려가는지 다 달라서 switch문으로 다 따로 처리해야할 것 같음
                {
                    case AreaType.MONSTER:
                        EnemyType eType = currentStageData.enemySpeciesArea;
                        rate = mobAreaWeightDic[currentFloor][eType] * 0.6666f;
                        rate2 = mobAreaWeightDic[currentFloor][eType] * 0.3333f;
                        mobAreaWeightDic[currentFloor][eType] -= rate;
                        for (int i = 0; i < floorSpecies[currentFloor - 1].second.Count; i++)
                        {
                            if(floorSpecies[currentFloor - 1].second[i] != eType)
                            {
                                mobAreaWeightDic[currentFloor][floorSpecies[currentFloor - 1].second[i]] -= rate2;
                            }
                        }
                        rate2 = (rate + rate2 + rate2) * 0.25f;
                        PlusWeight(AreaType.RANDOM, rate2);
                        PlusWeight(AreaType.STAT, rate2);
                        PlusWeight(AreaType.CHEF, rate2);
                        PlusWeight(AreaType.PLANTS, rate2);
                        break;

                    case AreaType.RANDOM:
                        rate = w * 0.36f;
                        SetWeight(type, w - rate);
                        rate2 = rate * 0.3333f;
                        CalcMobWeights(rate2);
                        break;

                    case AreaType.CHEF:
                        SetWeight(type, 0f);
                        rate = w * 0.5f;
                        MinusWeight(AreaType.PLANTS, rate);
                        MinusWeight(AreaType.STAT, rate);
                        rate2 = w * 0.25f;
                        PlusWeight(AreaType.RANDOM, rate2);
                        CalcMobWeights(rate2);
                        break;

                    case AreaType.STAT:
                        SetWeight(type, 0f);
                        rate = w * 0.2f;
                        PlusWeight(AreaType.PLANTS, rate);
                        PlusWeight(AreaType.CHEF, rate);
                        PlusWeight(AreaType.RANDOM, rate);
                        CalcMobWeights(rate);
                        break;

                    case AreaType.PLANTS:
                        SetWeight(type, 0f);
                        rate = w * 0.2f;
                        PlusWeight(AreaType.STAT, rate);
                        PlusWeight(AreaType.CHEF, rate);
                        PlusWeight(AreaType.RANDOM, rate);
                        CalcMobWeights(rate);
                        break;

                    default:
                        Debug.Log("이상한 구역이 인카운터에 영향을 받고 있음. 확인 필요 : " + type.ToString());
                        break;
                }

                UpdateMobAreaWeight();

                foreach (AreaType key in Global.GetEnumArr<AreaType>())  //주의 : Dictionary를 foreach로 순회중에 내부의 값을 바꾸면 순회가 중단되고 다음에 실행될 코드도 실행이 안됨.
                {
                    if (areaWeightDic[currentFloor].ContainsKey(key))
                    {
                        if (areaWeightDic[currentFloor][key] < 0f) areaWeightDic[currentFloor][key] = 0f;
                    }
                }
            }
        }
    }

    private void UpdateMobAreaWeight()  //몬스터 구역 가중치 갱신시켜줌
    {
        float w = 0f;
        foreach(float f in mobAreaWeightDic[currentFloor].Values) w += f;
        areaWeightDic[currentFloor][AreaType.MONSTER] = w;
    }

#endregion

    #region Weight Calcurate
    private void SetWeight(AreaType type, float weight)
    {
        areaWeightDic[currentFloor][type] = weight;
    }
    private void PlusWeight(AreaType type, float value)
    {
        areaWeightDic[currentFloor][type] += value;
    }
    private void MinusWeight(AreaType type, float value)
    {
        areaWeightDic[currentFloor][type] -= value;
    }
    private void CalcMobWeights(float value)
    {
        if (currentFloor <= 0 || currentFloor > maxStage)
        {
            Debug.Log("뭔가 좀 이상한 층 : " + currentFloor.ToString());
            return;
        }

        for (int i = 0; i < floorSpecies[currentFloor - 1].second.Count; i++)
        {
            mobAreaWeightDic[currentFloor][floorSpecies[currentFloor - 1].second[i]] += value;
        }
    }
    #endregion

    private void SetNextDoors(Func<StageDoor, bool> canSetDoorPos) //canSetDoorPos -> 이 자리에 문을 생성할 수 있는지
    {
        int idx = 0;
        int count = currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes.Length;

        if (currentFloor > 0)
        {
            List<AreaType> list = new List<AreaType>();  //이미 다음 스테이지의 문으로 정해진 구역들 리스트
            List<EnemyType> eList = new List<EnemyType>(); // 이미 뽑힌 몬스터 구역

            currentStage.stageDoors.ToRandomList(5).ForEach(door =>
            {
                if (!door.gameObject.activeSelf && idx < count && canSetDoorPos(door))  //로비에서는 어차피 문이 두 개. 입구/출구. 걸어서 가는 경우라면 상관없음
                {
                    try
                    {
                        AreaType type = GetRandomArea(list);
                        list.Add(type);
                        if (type != AreaType.MONSTER)
                        {
                            door.nextStageData = randomRoomDict[currentFloor][type][0];
                            randomRoomDict[currentFloor][type].RemoveAt(0);
                        }
                        else
                        {
                            //가중치를 통해서 어떤 몬스터 구역 소환할지 정함
                            float w = 0f, total = areaWeightDic[currentFloor][AreaType.MONSTER];
                            List<EnemyType> li = mobAreaWeightDic[currentFloor].Keys.ToList();
                            foreach (EnemyType key in mobAreaWeightDic[currentFloor].Keys)
                            {
                                if (eList.Contains(key))
                                {
                                    total -= mobAreaWeightDic[currentFloor][key];
                                    li.Remove(key);
                                }
                            }

                            //float sel = UnityEngine.Random.Range(0f, total);
                            float sel = UnityEngine.Random.value * total; 
                            EnemyType target = floorSpecies[currentFloor - 1].second[0];
                            for(int i= 0; i < li.Count; i++)
                            {
                                w += mobAreaWeightDic[currentFloor][li[i]];
                                if(sel < w)
                                {
                                    target = li[i];
                                    break;
                                }
                            }

                            eList.Add(target);

                            //해당 몬스터 구역 뽑아옴
                            foreach(StageDataSO data in randomRoomDict[currentFloor][AreaType.MONSTER])
                            {
                                if(data.enemySpeciesArea == target)
                                {
                                    door.nextStageData = data;
                                    break;
                                }
                            }
                            randomRoomDict[currentFloor][type].Remove(door.nextStageData);
                        }
                        randomRoomDict[currentFloor][type].Add(door.nextStageData);

                        //door.nextStageData = randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[idx]][0];
                        //randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[idx]].RemoveAt(0);
                        //randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[idx]].Add(door.nextStageData);

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
        }
        else  //튜토리얼 맵이라면
        {
            currentStage.stageDoors.ForEach(door =>
            {
                if (!door.gameObject.activeSelf)
                {
                    door.gameObject.SetActive(true);
                }
            });
        }
    }

    private void CheckStageBug()
    {
        List<AreaType> aList = new List<AreaType>();
        List<EnemyType> eList = new List<EnemyType>();
        AreaType aType;
        EnemyType eType;

        for(int i=0; i<currentStage.stageDoors.Length; i++)
        {
            if (!currentStage.stageDoors[i].gameObject.activeSelf || currentStage.stageDoors[i].IsExitDoor || !currentStage.stageDoors[i].nextStageData) continue;

            aType = currentStage.stageDoors[i].nextStageData.areaType;
            if (aList.Contains(aType))
            {
                if(aType != AreaType.MONSTER)
                {
                    Debug.LogWarning("문제 발생: 같은 타입의 방이 동시에 존재 " + aType.ToString());
                }
                else
                {
                    eType = currentStage.stageDoors[i].nextStageData.enemySpeciesArea;
                    if (eList.Contains(eType))
                    {
                        Debug.Log("문제 발생: 같은 몬스터 타입의 방이 동시에 존재 " + eType.ToString());
                    }
                    else
                    {
                        eList.Add(eType);
                    }
                }
            }
            else
            {
                aList.Add(aType);
            }
        }
    }

    public void SetMonsterStage()
    {
        IsStageClear = false;
        currentStage.CloseDoor();
    }

    public void NextEnemy(bool autoSpawn)
    {
        string id = CurrentMonstersOrderID;

        if (string.IsNullOrEmpty(id))
        {
            if(!autoSpawn && enemyKill == GetCurStageEnemyCount())
                StageClear();
            return;
        }

        if (currentArea != AreaType.BOSS)
        {
            Util.PriDelayFunc("Next Enemys Spawn", () => EventManager.TriggerEvent("SpawnEnemy", id), 1f, this, false);

            if(currentStageData.stageMonsterBundleCount > currentStageMonsterBundleOrder)
            {
                Util.PriDelayFunc("AutoSpawnNextEnemy", () => NextEnemy(true), 
                    currentStageData.nextEnemysSpawnInterval[currentStageMonsterBundleOrder - 1] + 1f, this, false);
            }
        }
        else
        {
            EventManager.TriggerEvent("SpawnEnemy", id);
        }

        currentStageMonsterBundleOrder++;
    }

    public int GetCurStageEnemyCount()  //현재 스테이지의 몹 수 전체 수. => 몇 세트씩 나뉘어서 나오는 것들 전부 합쳐서
    {
        if(enemyCount > 0) return enemyCount;

        string[] strs = currentStageData.stageMonsterBundleID;
        int count = 0;

        //count += Enemy.EnemyManager.Instance.enemyDictionary[strs[i]].Count;  // 아직 스테이지 로드가 안 된 경우에는 값을 못불러온다

        Dictionary<string,List<EnemySpawnData>> spawnData =  CSVEnemySpawn.Instance.enemySpawnDatas;
        for (int i = 0; i < strs.Length; i++)
        {
            count += spawnData[strs[i]].Count;
        }

        enemyCount = count;
        return count;
    }

    public void SetClearStage()
    {
        IsStageClear = true;
        currentStage.OpenDoors();
    }

    public void StartNextStage()  //다음 스테이지에 완전히 들어왔고 그 스테이지를 시작해줌
    {
        Debug.Log("Start Next Stage");
        NextStagePreEvent();

        if (currentArea == AreaType.MONSTER || currentArea == AreaType.BOSS) //전투구역이면 적 소환
        {
            EnemyManager.Instance.enemyCount = 0;
            NextEnemy(false);
        }
        
        EventManager.TriggerEvent("StartBGM", currentStageData.stageID); //BGM
       
        //구역 이름을 띄워줌
        string str = string.IsNullOrEmpty(currentStageData.stageName) ? Global.AreaTypeToString(currentArea) : currentStageData.stageName;
        if (currentArea != AreaType.BOSS)
        {
            UIManager.Instance.InsertTopCenterNoticeQueue(str);
        }
        else
        {
            UIManager.Instance.InsertTopCenterNoticeQueue(str, UIManager.Instance.bossNoticeMsgVGrd); 
        }
        
        //맵 입장 이벤트가 있으면 실행
        if(currentStageData.mapEvent != null)
        {
            currentStageData.mapEvent.OnEnterEvent();
        }
    }

    public void StageClear()
    {
        SetClearStage();

        EventManager.TriggerEvent("StageClear");

        if(currentArea==AreaType.BOSS)
        {
            //EventManager.TriggerEvent("GameClear");
            Debug.Log("Boss Stage Clear!!");

            if (currentFloor == maxStage)
            {
                EventManager.TriggerEvent("GameClear");
            }
        }
    }

    private void Respawn()
    {
        Debug.Log("Respawn : " + startStageID);

        currentFloor = 1;
        currentStageNumber = 0;
        PassDir = GameManager.Instance.savedData.stageInfo.passDoorDir;
        InsertRandomMaps(currentFloor);
        ResetMapWeight(currentFloor);
        SetRandomAreaRandomIncounter();
        NextStage(startStageID, true);
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

    public StageBundleDataSO GetStageBundleData(int floor)
    {
        string str = FloorToFloorID(floor);
        if (!string.IsNullOrEmpty(str))
        {
            return idToStageFloorDict[str];
        }
        Debug.Log("잘못된 접근 : " + floor + "층");
        return null;
    }

    private void EnterRandomArea()  //랜덤구역에 입장함
    {
        //RandomRoomType room = (RandomRoomType)UnityEngine.Random.Range(0, Global.EnumCount<RandomRoomType>());
        //room = RandomRoomType.IMPRECATION;
        //랜덤맵일 때는 EventManager.TriggerEvent(Global.EnterNextMap)가 실행안되므로 저주나 회복일 땐 따로 부름. 몹 구역일 땐 어차피 NextStage로 호출함

        RandomRoomType room = randomZoneTypeListDic[currentFloor][0];
        randomZoneTypeListDic[currentFloor].RemoveAt(0);
        randomZoneTypeListDic[currentFloor].Add(room); //혹시 모를 예외 처리

        switch (room)
        {
            case RandomRoomType.IMPRECATION: //저주 구역
                currentArea = AreaType.IMPRECATION;
                EventManager.TriggerEvent(Global.EnterNextMap);

                ImprecationObj io = PoolManager.GetItem<ImprecationObj>("ImprecationObjPref1");
                //io.transform.position = currentStage.objSpawnPos.position;
                Util.PriDelayFunc("SetImprecationEnv", () =>
                {
                    Environment.Instance.OnEnteredOrExitImprecationArea(true);
                    Util.PriDelayFunc("ImprecationInteract", io.Interaction, Global.ImprAndRecoInteractDelay, this, false);
                }, Global.ImprAndRecoEffDelay, this, false);
                break;

            case RandomRoomType.MONSTER:  //몬스터 구역
                isSelectRandomArea = true;
                //int targetStage = Mathf.Clamp(currentStageData.stageFloor.floor + UnityEngine.Random.Range(-1, 2), 1, MaxStage); //현재 층에서 몇 층을 더할지 정함
                //StageBundleDataSO sbData = idToStageFloorDict.Values.Find(x=>x.floor == targetStage); //현재 층에서 -1 or 0 or 1층을 더한 층을 가져온다
                NextStage(GetStageBundleData(currentFloor).monsterStages.ToRandomElement().stageID, false); //뽑은 층에서 몬스터 지역들중에 랜덤으로 가져온다
                break;

            case RandomRoomType.RECOVERY:  //회복 구역
                currentArea = AreaType.RECOVERY;
                EventManager.TriggerEvent(Global.EnterNextMap);

                RecoveryObj ro = PoolManager.GetItem<RecoveryObj>("RecoveryObjPrefObjPref1");
                //ro.transform.position = currentStage.objSpawnPos.position;

                Util.PriDelayFunc("SetRecoveryEnv", () =>
                {
                    Environment.Instance.OnEnteredOrExitRecoveryArea(true);
                    ro.ActiveRecovLight();
                    Util.PriDelayFunc("RecoveryInteract", ro.Interaction, Global.ImprAndRecoInteractDelay, this, false);
                }, Global.ImprAndRecoEffDelay, this, false);

                break;
            /*case RandomRoomType.CHARACTERISTIC:
                isSelectRandomArea = true;
                NextStage(GetStageBundleData(currentFloor).stages.FindAll(x=>x.areaType == AreaType.STAT).ToRandomElement().stageID, false);
                break;*/
        }
    }

    private NPC GetNPC(string id)
    { 
        if(npcDict.ContainsKey(id)) return npcDict[id];

        NPC npc = Instantiate(Resources.Load<GameObject>("Prefabs/NPC/" + id), transform).GetComponent<NPC>();
        npcDict.Add(id, npc);
        return npc;
    }

    private void ClearNPC()
    {
        currentMapNPCList.ForEach(x => x.gameObject.SetActive(false));
        currentMapNPCList.Clear();
    }

    private void ChefStage()
    {
        NPC npc = GetNPC(currentStageData.mapNPC.name);
        npc.gameObject.SetActive(true);
        npc.transform.position = currentStage.objSpawnPos.position;
        currentMapNPCList.Add(npc);
    }

    public void SaveStage(string stageId = "")
    {
        if (string.IsNullOrEmpty(stageId))
            stageId = currentStageData.stageID;

        GameManager.Instance.savedData.stageInfo.currentStageID = stageId;
        GameManager.Instance.savedData.stageInfo.passDoorDir = PassDir;
        startStageID = stageId;
    }

    private void SetEnemyManagerStageData(string stageId)
    {
        if (stageDataDictionary.ContainsKey(stageId))
        {
            EnemyManager.SetCurrentStageData(stageDataDictionary[stageId]);
        }
        else
        {
            EnemyManager.SetCurrentStageData(null);
        }
    }
}
