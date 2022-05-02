using UnityEngine;
using System.Collections.Generic;
using System;
using Water;
using FkTweening;

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
    [SerializeField] private int MaxStage;
    [SerializeField] private string startStageID;
    //[HideInInspector] public Vector2 respawnPos;

    //public Sprite openDoorSpr, closeDoorSpr;
    public Sprite[] doorSprites;
    public Dictionary<string, Sprite> doorSprDic = new Dictionary<string, Sprite>();
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

    public Func<bool> canNextStage = null;  //���� ���������� �� �� �̰� null�̸� ������ ���� ���� ������ �װ��� �����ؼ� true�� ��������

    public DoorDirType PassDir { get; set; } //�� ������������ ������ �� ����
    public bool IsStageClear { get; set; }
    public Vector3 MapCenterPoint { get; private set; }

    private string CurrentMonstersOrderID => currentStageData.stageMonsterBundleCount < currentStageMonsterBundleOrder ? string.Empty : currentStageData.stageMonsterBundleID[currentStageMonsterBundleOrder - 1];

    private Dictionary<int, List<RandomRoomType>> randomZoneTypeListDic = new Dictionary<int, List<RandomRoomType>>(); //���� �������� ���� ���� Ÿ�Ե��� �̸� �־����
    private List<RandomRoomType> randomZoneRestTypes = new List<RandomRoomType>(); //������������ ���� ���� Ÿ�Ե� ���� ���� �� (�׽�Ʈ�� ����)
    //private int prevRandRoomType = -1;  // ���� ���� ������ Ÿ��

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

        EventManager.StartListening(Global.EnterNextMap, () =>  //�ش� ���� �������� ��, �������� ȣ��
        {
            currentStageNumber++;
        });

        floorInitSet = new bool[MaxStage + 1];
        for(int i=0; i<floorInitSet.Length; i++)
            floorInitSet[i] = false;
    }

    private void Start()
    {
        Init();
        DefineEvent();
    }

    private void Init()
    {
        startStageID = GameManager.Instance.savedData.stageInfo.currentStageID;
        PassDir = GameManager.Instance.savedData.stageInfo.passDoorDir;
        if (TutorialManager.Instance.IsTestMode) startStageID = "Stage1-01";
        currentFloor = GetStageData(startStageID).stageFloor.floor;
        InsertRandomMaps(currentFloor);
        SetRandomAreaRandomIncounter();
        Util.DelayFunc(() => NextStage(startStageID), 0.2f);
        //respawnPos = idToStageDataDict[startStageID].stage.GetComponent<StageGround>().playerSpawnPoint.position;

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
            DOUtil.StopCo("Next Enemys Spawn", this);
            Enemy.EnemyManager.Instance.PlayerDeadEvent();

            if (canNextStage != null) canNextStage = null;
        });
        EventManager.TriggerEvent("StartBGM", startStageID);
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("StartNextStage", StartNextStage);
        EventManager.StartListening("PickupMiniGame", (Action<bool>)(start => currentStage.StageLightActive(!start)));
        EventManager.StartListening("GameClear", () =>
        {
            Debug.Log("Game Clear : " + currentFloor + "-" + currentStageNumber);
        });
    }

    private void SetRandomAreaRandomIncounter()  //�ϴ� �����丵 ���߿� �ؾ��� ��
    {
        if (currentFloor == 0) return;

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

        //������ / ����Ÿ���� ����������� ������ ������ �������� ��������ŭ ����ִµ� ��ġ�� �ʰ� ��
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

        for(i=0; i<r; i++)
        {
            randomZoneTypeListDic[currentFloor].Insert(UnityEngine.Random.Range(0, randomZoneTypeListDic[currentFloor].Count), (RandomRoomType)UnityEngine.Random.Range(0, rrCnt));
        }

        randomZoneRestTypes = randomZoneTypeListDic[currentFloor];
    }

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

            foreach (AreaType type in Enum.GetValues(typeof(AreaType)))  //���� ���� �� �ʱ�ȭ
            {
                randomRoomDict[floor][type].Clear();
            }

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

        //���� �������� ������ ���ְ� ���� ���������� �ҷ��ͼ� ���ְ� �������� ��ȣ�� 1 ������Ŵ
        if (currentStage) currentStage.gameObject.SetActive(false);

        currentStageData = idToStageDataDict[id];
        currentArea = currentStageData.areaType;
        //IsLastStage = currentStageData.endStage;
        currentStage = null;
        currentStageMonsterBundleOrder = 1;

        if(currentStageData.stageFloor.floor != currentFloor) //���� ���������� ���� �ٸ��� ���� ���� �ؾ���
        {
            currentStageNumber = 0;
            currentFloor++;
            InsertRandomMaps(currentFloor);
            SetRandomAreaRandomIncounter();

            Debug.Log("�� UP : " + currentFloor);
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
            currentStage.stageDoors.ForEach(door =>
            {
                if (!door.gameObject.activeSelf)
                {
                    door.gameObject.SetActive(true);
                }
            });
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
                        currentStageData.mapEvent.OnEnterEvent();
                    }

                    break;
                case AreaType.MONSTER:
                    //EventManager.TriggerEvent("SpawnEnemy", currentStageData.stageID);
                    SetMonsterStage();
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
                    return;   //���� ���̸� �Լ��� ����������.
                case AreaType.BOSS:
                    SetMonsterStage();
                    //EventManager.TriggerEvent("BossSpawn", CurrentMonstersOrderID);
                    //currentStageMonsterBundleOrder++;
                    break;
            }


            EventManager.TriggerEvent(Global.EnterNextMap);  //�ش� �� ���� �Ŀ� �ʿ��� �̺�Ʈ�� ó��
        }
    }

    private void SetNextDoors(Func<StageDoor, bool> canSetDoorPos) //canSetDoorPos -> �� �ڸ��� ���� ������ �� �ִ���
    {
        int idx = 0;
        int count = currentStageData.stageFloor.randomStageList[currentStageNumber].nextStageTypes.Length;

        if (currentFloor > 0)
        {
            currentStage.stageDoors.ToRandomList(5).ForEach(door =>
            {
                if (!door.gameObject.activeSelf && idx < count && canSetDoorPos(door))
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
                    Debug.Log(door.name);
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
            DOUtil.ExecuteTweening("Next Enemys Spawn", Util.DelayFuncCo(() => EventManager.TriggerEvent("SpawnEnemy", id), 1f, false), this);
        }
        else
        {
            EventManager.TriggerEvent("SpawnEnemy", id);
        }

        currentStageMonsterBundleOrder++;
    }

    public void SetClearStage()
    {
        IsStageClear = true;
        currentStage.OpenDoors();
    }

    public void StartNextStage()
    {
        if (currentArea == AreaType.MONSTER || currentArea == AreaType.BOSS)
        {
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
        currentFloor--;
        PassDir = GameManager.Instance.savedData.stageInfo.passDoorDir;
        InsertRandomMaps(currentFloor);
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

        switch (room)
        {
            case RandomRoomType.IMPRECATION: //���� ����
                currentArea = AreaType.IMPRECATION;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitImprecationArea(true);
                SoundManager.Instance.SetBGMPitchByLerp(1, -0.7f, 1f);
                ImprecationObj io = PoolManager.GetItem<ImprecationObj>("ImprecationObjPref1");
                io.transform.position = currentStage.objSpawnPos.position;
                Util.DelayFunc(() => io.Interaction(), 4.5f);
                break;

            case RandomRoomType.MONSTER:  //���� ����
                //--currentStageNumber;
                int targetStage = Mathf.Clamp(currentStageData.stageFloor.floor + UnityEngine.Random.Range(-1, 2), 1, MaxStage); //���� ������ �� ���� ������ ����
                StageBundleDataSO sbData = idToStageFloorDict.Values.Find(x=>x.floor == targetStage); //���� ������ -1 or 0 or 1���� ���� ���� �����´�
                NextStage(sbData.stages.FindRandom(stage => stage.areaType == AreaType.MONSTER).stageID); //���� ������ ���� �������߿� �������� �����´�
                break;

            case RandomRoomType.RECOVERY:  //ȸ�� ����
                currentArea = AreaType.RECOVERY;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitRecoveryArea(true);
                RecoveryObj ro = PoolManager.GetItem<RecoveryObj>("RecoveryObjPrefObjPref1");
                ro.transform.position = currentStage.objSpawnPos.position;
                
                if (StateManager.Instance.IsPlayerFullHP && StateManager.Instance.IsPlayerNoImpr)
                {
                    SetClearStage();
                }
                else
                {
                    Util.DelayFunc(() => ro.Interaction(), 4.5f);
                }
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
}
