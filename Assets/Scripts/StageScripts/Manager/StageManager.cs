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

    //private bool completeLoadNextMap; //���� ���� ������ �ҷ��Դ���

    public DoorDirType PassDir { get; set; } //�� ������������ ������ �� ����

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
            StageBundleDataSO bundle = idToStageFloorDict[FloorToFloorID(floor)]; //�� ���� ����


            /*foreach (StageDataSO data in bundle.stages)
            {
                stageIDList.Add(data.stageID);
            }*/
            foreach (AreaType type in Enum.GetValues(typeof(AreaType)))  //���� ���� �� �ʱ�ȭ
            {
                randomRoomDict[floor][type].Clear();
            }

            /*for (int i = 0; i < bundle.randomStageList.Count; i++) //���� �ٲܰ�
            {
                for (int j = 0; j < bundle.randomStageList[i].nextStageTypes.Length; j++) //���� �� ��Ĵ�ζ�� ���� �̷��� �� �ʿ� ������ ���߿� ���� �ٲ� ���� ������ �ϴ� ���� ��
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
        //���� �������� ������ ���ְ� ���� ���������� �ҷ��ͼ� ���ְ� �������� ��ȣ�� 1 ������Ŵ
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
            SlimeGameManager.Instance.CurrentPlayerBody.transform.position = MapCenterPoint;
        }
        else
        {
            //���� �������� ���� �ش� ���⿡ �°� �� ���������� �� ��ó���� ������ �Ǹ� �� ���� ������ ������ ��������Ʈ�� �ٲ�
            MapCenterPoint = currentStage.GetOpposeDoor(PassDir).playerSpawnPos.position;
            SlimeGameManager.Instance.CurrentPlayerBody.transform.position = MapCenterPoint;
        }
        
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);  //Camera Move Range Set
        GameManager.Instance.ResetDroppedItems(); //Inactive Items

        //���� �������� ����� ����ŭ ���� ���ְ� ���� ���� �������� Ÿ�Կ� �°� �������� ���� �������� ����
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
                    Debug.Log($"���������� �ҷ����� ���� {currentFloor} - {currentStageNumber} : idx: {idx}");
                }
            }
        });

        //�ش� ���������� Ÿ�Կ� ���� ���𰡸� ��
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
                return;   //���� ���̸� �Լ��� ����������.
            case AreaType.BOSS:
                //���������̸� ������ �ұ�
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
            Debug.Log("�������� �ʴ� �������� ���̵� : " + id);
            return null;
        }
    }

    private void EnterRandomArea()
    {
        RandomRoomType room = (RandomRoomType)UnityEngine.Random.Range(0, Global.EnumCount<RandomRoomType>());
        //�������� ���� EventManager.TriggerEvent(Global.EnterNextMap)�� ����ȵǹǷ� ���ֳ� ȸ���� �� ���� �θ�. �� ������ �� ������ NextStage�� ȣ����
        switch (room)
        {
            case RandomRoomType.IMPRECATION: //���� ����
                currentArea = AreaType.IMPRECATION;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitImprecationArea(true);
                PoolManager.GetItem("ImprecationObjPref1").transform.position = currentStage.objSpawnPos.position;
                break;
            case RandomRoomType.MONSTER:  //���� ����
                --currentStageNumber;
                int targetStage = Mathf.Clamp(currentStageData.stageFloor.floor + UnityEngine.Random.Range(-1, 2), 1, MaxStage); //���� ������ �� ���� ������ ����
                StageBundleDataSO sbData = idToStageFloorDict.Values.Find(x=>x.floor == targetStage); //���� ������ -1 or 0 or 1���� ���� ���� �����´�
                NextStage(sbData.stages.FindRandom(stage => stage.areaType == AreaType.MONSTER).stageID); //���� ������ ���� �������߿� �������� �����´�
                break;
            case RandomRoomType.RECOVERY:  //ȸ�� ����
                currentArea = AreaType.RECOVERY;
                EventManager.TriggerEvent(Global.EnterNextMap);
                Environment.Instance.OnEnteredOrExitRecoveryArea(true);
                PoolManager.GetItem("RecoveryObjPrefObjPref1").transform.position = currentStage.objSpawnPos.position;
                PoolManager.GetItem("NormalPointLight2D").transform.position = currentStage.objSpawnPos.position;
                break;
        }
    }
}
