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

    private int currentStageMonsterBundleOrder = 1;

    private int currentFloor = 1;
    private int currentStageNumber = 0;
    private AreaType currentArea = AreaType.NONE;
    public AreaType CurrentAreaType => currentArea;

    private List<NPC> currentMapNPCList = new List<NPC>();

    //private bool completeLoadNextMap; //���� ���� ������ �ҷ��Դ���
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
    private Dictionary<int, Dictionary<AreaType, int>> areaWeightDic = new Dictionary<int, Dictionary<AreaType, int>>();
    private Dictionary<int, Dictionary<EnemyType, int>> mobAreaWeightDic = new Dictionary<int, Dictionary<EnemyType, int>>();
    [SerializeField] private List<Pair<int, List<EnemyType>>> floorSpecies;  //������ � ���� ��������
    [SerializeField] private List<Pair<AreaType, int>> areaWeight;  //�ʸ��� �⺻ ����ġ
    [SerializeField] private int mobWeight = 15; //���ʹ� �⺻ ����ġ 
    [SerializeField] private int plusWeight = 10, minusWeight = 5;
    #endregion

    public Func<bool> canNextStage = null;  //���� ���������� �� �� �̰� null�̸� ������ ���� ���� ������ �װ��� �����ؼ� true�� ��������

    public DoorDirType PassDir { get; set; } //�� ������������ ������ �� ����
    public bool IsBattleArea => currentArea == AreaType.MONSTER || currentArea == AreaType.BOSS;
    public bool IsFighting => IsBattleArea && !IsStageClear;
    public bool IsStageClear { get; set; }
    public Vector3 MapCenterPoint { get; private set; }

    public event Action NextStagePreEvent;

    private string CurrentMonstersOrderID => currentStageData.stageMonsterBundleCount < currentStageMonsterBundleOrder ? string.Empty : currentStageData.stageMonsterBundleID[currentStageMonsterBundleOrder - 1];

    private Dictionary<int, List<RandomRoomType>> randomZoneTypeListDic = new Dictionary<int, List<RandomRoomType>>(); //���� �������� ���� ���� Ÿ�Ե��� �̸� �־����

    private Dictionary<string, StageData> stageDataDictionary = new Dictionary<string, StageData>();
    private string stageDataPath = Path.Combine("Enemy", "StageData", "StageData");

    public LinkedListNode<string> s;
    public LinkedList<string> ss;

    private void Awake()
    {
        //�� ������
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

        //�������� ������
        string jsonData = Resources.Load(stageDataPath).ToString();
        var stageData = JsonUtility.FromJson<JsonParse<StageData>>(jsonData);

        for (int i = 0; i < stageData.jsonData.Count; i++)
        {
            stageDataDictionary.Add(stageData.jsonData[i].stageName, stageData.jsonData[i]);
        }

        //�� ��ī���� �غ� + �������� ���� (��)
        int[] mobStageWeight = new int[maxStage];
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
            areaWeightDic.Add(i + 1 , new Dictionary<AreaType, int>());
            mobAreaWeightDic.Add(i + 1, new Dictionary<EnemyType, int>());

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

        //�������� ���� �غ� (��) + �������� ������ ����
        int cnt = Global.EnumCount<AreaType>();
        foreach (StageBundleDataSO data in idToStageFloorDict.Values)
        {
            
            randomRoomDict.Add(data.floor, new Dictionary<AreaType, List<StageDataSO>>());
            for(int i=0; i<cnt; i++)
            {
                randomRoomDict[data.floor].Add((AreaType)i, new List<StageDataSO>());
            }

            randomZoneTypeListDic.Add(data.floor, new List<RandomRoomType>());  //�������� Ű �� ����
        }

        //�� ��������Ʈ ����
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

        EventManager.StartListening(Global.EnterNextMap, () =>  //�ش� ���� �������� ��, �������� ȣ��
        {
            UpdateAreaWeights();
            currentStageNumber++;
        });

        floorInitSet = new bool[maxStage + 1];  //Ʃ�丮�� (0��������) �����ϹǷ� +1
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
                UIManager.Instance.RequestSystemMsg("�̹� �� �����ִ�");
                return;
            }

            SetClearStage();
            UIManager.Instance.RequestSystemMsg("������ ���� ����");
            Enemy.EnemyManager.Instance.PlayerDeadEvent();
        });

        GameManager.Instance.testKeyInputActionDict.Add(KeyCode.F11, () =>
        {
            foreach(AreaType key in areaWeightDic[currentFloor].Keys)
            {
                Debug.Log($"{key} : {areaWeightDic[currentFloor][key]}");
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
        EventManager.StartListening("ExitCurrentMap", () =>   //���� ���� �� ������ ���� ó��
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
                    currentMapNPCList.ForEach(x => x.gameObject.SetActive(false));
                    currentMapNPCList.Clear();
                    break;
                case AreaType.BOSS:
                    SkillUIManager.Instance.SetEnableSlot(SkillType.SPECIALATTACK2, true);
                    break;

            }

            SoundManager.Instance.SetBGMPitch(1);
            DOUtil.StopCo("Next Enemys Spawn", this);
            Enemy.EnemyManager.Instance.PlayerDeadEvent();

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

    private void SetRandomAreaRandomIncounter()  //�ϴ� �����丵 ���߿� �ؾ��� ��
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

        //������ / ����Ÿ���� ����������� ������ ������ �������� ��������ŭ ����ִµ� ��ġ�� �ʰ� ��
        List<RandomRoomType> rList = new List<RandomRoomType>();
        if (r > 0)
        {
            RandomRoomType rr;
            for (i = 0; i < r; i++)
            {
                rr = (RandomRoomType)UnityEngine.Random.Range(0, rrCnt);

                if (rList.Contains(rr))
                    i--;
                else
                    rList.Add(rr);
            }
        }

        //���� ���� �� �� ������ rrCnt - 1�̸����� �ƴ��� üũ
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
            return (true, RandomRoomType.MONSTER); //�ι�°���ڴ� false�� ���� ���߱� ���ؼ� �׳� �� ��
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

        //������ ���� ����� ������ ��ġ�� �������
        for(i=0; i<r; i++)
        {
            pre = UnityEngine.Random.Range(0, randomZoneTypeListDic[currentFloor].Count);

            if(pre > 1 && randomZoneTypeListDic[currentFloor][pre-1] == rList[i])
            {
                if(randomZoneTypeListDic[currentFloor][pre - 2] == rList[i]) //���� ��ġ�κ��� �ڿ� �� ���� ���� ���̰� ���� �ʵ� ���� ���̸�
                {
                    i--;
                    continue;
                }
            }

            // 0 1 2 3 4 5 ����Ʈ�� insert(4, 100)�� �ϸ� 0 1 2 3 100 4 5�� ��
            if(pre < randomZoneTypeListDic[currentFloor].Count - 1 && randomZoneTypeListDic[currentFloor][pre] == rList[i])
            {
                if(randomZoneTypeListDic[currentFloor][pre + 1] == rList[i]) //�ְ��� �տ� �� ���� ���� ���̰� ���� �ʵ� ���� ���̸�
                {
                    i--;
                    continue;
                }
            }
            
            if(pre > 0 && randomZoneTypeListDic[currentFloor][pre - 1] == rList[i])
            {
                if(randomZoneTypeListDic[currentFloor][pre] == rList[i]) //���� ��ġ���� �հ� �ڰ� ���� ���̰� ���� �ʵ� ���� ���̸�
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

    private void InsertRandomMaps(int floor)
    {
        if (currentFloor == 0) return;

        if (!floorInitSet[floor])
        {
            floorInitSet[floor] = true; 
            StageBundleDataSO bundle = idToStageFloorDict[FloorToFloorID(floor)]; //�� ���� ����

            foreach (AreaType type in Enum.GetValues(typeof(AreaType)))  //���� ���� �� �ʱ�ȭ  (���ص� �Ǳ������� �ϴ� �� ��)
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

    //���� ���������� ��. (������ �ε� ȭ�� �߿� �̷������ �Լ�)
    public void NextStage(string id, bool teleport)
    {
        Debug.Log("Next Stage : " + id);

        EventManager.TriggerEvent("ExitCurrentMap");

        //���� �������� ������ ���ְ� ���� ���������� �ҷ��ͼ� ���ְ� �������� ��ȣ�� 1 ������Ŵ
        if (currentStage) currentStage.gameObject.SetActive(false);

        currentStageData = idToStageDataDict[id];
        currentArea = currentStageData.areaType;
        currentStage = null;
        currentStageMonsterBundleOrder = 1;

        if(currentStageData.stageFloor.floor != currentFloor) //���� ���������� ���� �ٸ��� ���� ���� �ؾ���
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

        //���������� ���� �ʱ�ȭ
        currentStage.stageDoors.ForEach(x => 
        { 
            x.gameObject.SetActive(false);
            x.IsExitDoor = false;
        });

        //Player Position
        if(currentStage.playerSpawnPoint)
        {
            //�ַ� ó�� ���� ��ŸƮ ����
            MapCenterPoint = currentStage.playerSpawnPoint.position;
        }
        else if (currentStage.lobbySpawnPoint && teleport)  //�ַ� �κ񿡼� ��Ȱ�ϰų� �������� ��
        {
            MapCenterPoint = currentStage.lobbySpawnPoint.position;
            currentStage.GetOpposeDoor(PassDir);
        }
        else
        {
            //���� �������� ���� �ش� ���⿡ �°� �� ���������� �� ��ó���� ������ �Ǹ� �� ���� ������ ������ ��������Ʈ�� �ٲ�
            MapCenterPoint = currentStage.GetOpposeDoor(PassDir).playerSpawnPos.position;
            
        }

        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = MapCenterPoint;
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);  //Camera Move Range Set
        GameManager.Instance.ResetDroppedItems(); //Inactive Items

        //���� �������� ����� ����ŭ ���� ���ְ� ���� ���� �������� Ÿ�Կ� �°� �������� ���� �������� ����

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
            
            //������ 1�������������� �����Ƿ� �ӽ÷� �̷��� ��
            if (currentFloor > 0)
            {
                currentStage.stageDoors.ForEach(door =>
                {
                    if (!door.IsExitDoor)
                    {
                        door.gameObject.SetActive(false);
                    }
                });
            }
            else
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

        if (currentStageData.isSaveStage)
        {
            SaveStage();
        }

        {
            //�ش� ���������� Ÿ�Կ� ���� ���𰡸� ��  (�ش� ���� �������� ��)
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
                    StatStore.Instance.EnteredStatArea();
                    break;
                case AreaType.RANDOM:
                    SetMonsterStage();
                    IsStageClear = false;
                    EnterRandomArea();
                    return;   //���� ���̸� �Լ��� ����������.
                case AreaType.BOSS:
                    SetMonsterStage();
                    SkillUIManager.Instance.SetEnableSlot(SkillType.SPECIALATTACK2, false);
                    break;
            }

            SetEnemyManagerStageData(id);

            EventManager.TriggerEvent(Global.EnterNextMap);  //�ش� �� ���� �Ŀ� �ʿ��� �̺�Ʈ�� ó��
        }
    }

    private void ResetMapWeight(int floor)
    {
        int mobStageWeight = 0, i;
        for (i = 0; i < floorSpecies[floor].second.Count; i++)
        {
            mobStageWeight += mobWeight;
        }

        for (i = 0; i < areaWeight.Count; i++)
        {
            areaWeightDic[floor][areaWeight[i].first] = areaWeight[i].second;
        }
        areaWeightDic[floor][AreaType.MONSTER] = mobStageWeight;
        for (i = 0; i < floorSpecies[floor].second.Count; i++)
        {
            mobAreaWeightDic[floor][floorSpecies[floor].second[i]] = mobWeight;
        }
    }

    private AreaType GetRandomArea(List<AreaType> list)
    {
        List<AreaType> li = areaWeightDic[currentFloor].Keys.ToList();
        if (currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes.Length == 1)
            return currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes[0];
        int total = 0, weight = 0, i;
        for (i = 0; i < li.Count; i++)
            total += areaWeightDic[currentFloor][li[i]];

        for(i=0; i<list.Count; i++)
        {
            li.Remove(list[i]);
            total -= areaWeightDic[currentFloor][list[i]];
        }

        int sel = Mathf.RoundToInt(total * UnityEngine.Random.Range(0f, 1f));

        for (i = 0; i < li.Count; i++)
        {
            weight += areaWeightDic[currentFloor][li[i]];
            if (sel < weight)
            {
                if (!list.Contains(li[i])) return li[i];
            }
        }

        return GetRandomArea(list); 
    }

    private void UpdateAreaWeights()
    {
        AreaType type = currentStageData.areaType;
        if (areaWeightDic[currentFloor].ContainsKey(type))
        {
            areaWeightDic[currentFloor][type] -= minusWeight;

            if (areaWeightDic[currentFloor][type] <= 0) areaWeightDic[currentFloor][type] = 0;

            foreach (AreaType key in Global.GetEnumArr<AreaType>())  //���� : Dictionary�� foreach�� ��ȸ�߿� ������ ���� �ٲٸ� ��ȸ�� �ߴܵǰ� ������ ����� �ڵ嵵 ������ �ȵ�.
            {
                if (areaWeightDic[currentFloor].ContainsKey(key) && key != type)
                {
                    areaWeightDic[currentFloor][key] += plusWeight;
                }
            }
        }
    }

    private void SetNextDoors(Func<StageDoor, bool> canSetDoorPos) //canSetDoorPos -> �� �ڸ��� ���� ������ �� �ִ���
    {
        int idx = 0;
        int count = currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes.Length;

        if (currentFloor > 0)
        {
            List<AreaType> list = new List<AreaType>();

            currentStage.stageDoors.ToRandomList(5).ForEach(door =>
            {
                if (!door.gameObject.activeSelf && idx < count && canSetDoorPos(door))  //�κ񿡼��� ������ ���� �� ��. �Ա�/�ⱸ. �ɾ ���� ����� �������
                {
                    try
                    {
                        AreaType type = GetRandomArea(list);
                        list.Add(type);
                        door.nextStageData = randomRoomDict[currentFloor][type][0];
                        randomRoomDict[currentFloor][type].RemoveAt(0);
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
                        Debug.Log($"���������� �ҷ����� ���� {currentFloor} - {currentStageNumber} : idx: {idx}");
                    }
                }
            });
        }
        else  //Ʃ�丮�� ���̶��
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

    public void SetMonsterStage()
    {
        IsStageClear = false;
        currentStage.CloseDoor();
    }

    public void NextEnemy()
    {
        string id = CurrentMonstersOrderID;

        if (string.IsNullOrEmpty(id))
        {
            StageClear();
            return;
        }

        if (currentArea != AreaType.BOSS)
        {
            DOUtil.ExecuteTweening("Next Enemys Spawn", Util.DelayFuncCo(() => EventManager.TriggerEvent("SpawnEnemy", id), 1f, false, false), this);
        }
        else
        {
            EventManager.TriggerEvent("SpawnEnemy", id);
        }

        currentStageMonsterBundleOrder++;
    }

    public int GetCurStageEnemyCount()  //���� ���������� �� �� ��ü ��. => �� ��Ʈ�� ����� ������ �͵� ���� ���ļ�
    {
        string[] strs = currentStageData.stageMonsterBundleID;
        int count = 0;

        //count += Enemy.EnemyManager.Instance.enemyDictionary[strs[i]].Count;  // ���� �������� �ε尡 �� �� ��쿡�� ���� ���ҷ��´�

        Dictionary<string,List<EnemySpawnData>> spawnData =  CSVEnemySpawn.Instance.enemySpawnDatas;
        for (int i = 0; i < strs.Length; i++)
        {
            count += spawnData[strs[i]].Count;
        }
        return count;
    }

    public void SetClearStage()
    {
        IsStageClear = true;
        currentStage.OpenDoors();
    }

    public void StartNextStage()
    {
        Debug.Log("Start Next Stage");
        NextStagePreEvent();

        if (currentArea == AreaType.MONSTER || currentArea == AreaType.BOSS) //���������̸� �� ��ȯ
        {
            NextEnemy();
        }
        
        EventManager.TriggerEvent("StartBGM", currentStageData.stageID); //BGM
       
        //���� �̸��� �����
        string str = string.IsNullOrEmpty(currentStageData.stageName) ? Global.AreaTypeToString(currentArea) : currentStageData.stageName;
        if (currentArea != AreaType.BOSS)
        {
            UIManager.Instance.InsertTopCenterNoticeQueue(str);
        }
        else
        {
            UIManager.Instance.InsertTopCenterNoticeQueue(str, UIManager.Instance.bossNoticeMsgVGrd); 
        }
        
        //�� ���� �̺�Ʈ�� ������ ����
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
            EventManager.TriggerEvent("GameClear");
        }
    }

    private void Respawn()
    {
        Debug.Log("Respawn : " + startStageID);

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
            Debug.Log("�������� �ʴ� �������� ���̵� : " + id);
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
        Debug.Log("�߸��� ���� : " + floor + "��");
        return null;
    }

    private void EnterRandomArea()
    {
        //RandomRoomType room = (RandomRoomType)UnityEngine.Random.Range(0, Global.EnumCount<RandomRoomType>());
        //room = RandomRoomType.IMPRECATION;
        //�������� ���� EventManager.TriggerEvent(Global.EnterNextMap)�� ����ȵǹǷ� ���ֳ� ȸ���� �� ���� �θ�. �� ������ �� ������ NextStage�� ȣ����

        RandomRoomType room = randomZoneTypeListDic[currentFloor][0];
        randomZoneTypeListDic[currentFloor].RemoveAt(0);
        randomZoneTypeListDic[currentFloor].Add(room); //Ȥ�� �� ���� ó��

        switch (room)
        {
            case RandomRoomType.IMPRECATION: //���� ����
                currentArea = AreaType.IMPRECATION;
                EventManager.TriggerEvent(Global.EnterNextMap);

                ImprecationObj io = PoolManager.GetItem<ImprecationObj>("ImprecationObjPref1");
                io.transform.position = currentStage.objSpawnPos.position;
                Util.PriDelayFunc("SetImprecationEnv", () =>
                {
                    Environment.Instance.OnEnteredOrExitImprecationArea(true);
                    Util.PriDelayFunc("ImprecationInteract", io.Interaction, Global.ImprAndRecoInteractDelay, this, false);
                }, Global.ImprAndRecoEffDelay, this, false);
                break;

            case RandomRoomType.MONSTER:  //���� ����
                //int targetStage = Mathf.Clamp(currentStageData.stageFloor.floor + UnityEngine.Random.Range(-1, 2), 1, MaxStage); //���� ������ �� ���� ������ ����
                //StageBundleDataSO sbData = idToStageFloorDict.Values.Find(x=>x.floor == targetStage); //���� ������ -1 or 0 or 1���� ���� ���� �����´�
                NextStage(GetStageBundleData(currentFloor).monsterStages.ToRandomElement().stageID, false); //���� ������ ���� �������߿� �������� �����´�
                break;

            case RandomRoomType.RECOVERY:  //ȸ�� ����
                currentArea = AreaType.RECOVERY;
                EventManager.TriggerEvent(Global.EnterNextMap);

                RecoveryObj ro = PoolManager.GetItem<RecoveryObj>("RecoveryObjPrefObjPref1");
                ro.transform.position = currentStage.objSpawnPos.position;

                Util.PriDelayFunc("SetRecoveryEnv", () =>
                {
                    Environment.Instance.OnEnteredOrExitRecoveryArea(true);
                    ro.ActiveRecovLight();
                    if (StateManager.Instance.IsPlayerFullHP && StateManager.Instance.IsPlayerNoImpr)
                    {
                        SetClearStage();
                    }
                    else
                    {
                        Util.PriDelayFunc("RecoveryInteract", ro.Interaction, Global.ImprAndRecoInteractDelay, this, false);
                    }
                }, Global.ImprAndRecoEffDelay, this, false);

                break;
        }
    }

    private NPC GetNPC(string id)
    { 
        if(npcDict.ContainsKey(id)) return npcDict[id];

        NPC npc = Instantiate(Resources.Load<GameObject>("Prefabs/NPC/" + id), transform).GetComponent<NPC>();
        npcDict.Add(id, npc);
        return npc;
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
            Enemy.EnemyManager.SetCurrentStageData(stageDataDictionary[stageId]);
        }
        else
        {
            Enemy.EnemyManager.SetCurrentStageData(null);
        }
    }
}
