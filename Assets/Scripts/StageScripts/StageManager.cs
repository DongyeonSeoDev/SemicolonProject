using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoSingleton<StageManager>
{
    private Dictionary<string, StageGround> idToStageObjDict = new Dictionary<string, StageGround>();
    private Dictionary<string, StageDataSO> idToStageDataDict = new Dictionary<string, StageDataSO>();
    private Dictionary<string, StageBundleDataSO> idToStageFloorDict = new Dictionary<string, StageBundleDataSO>();
    private Dictionary<int, Dictionary<AreaType, List<StageDataSO>>> randomRoomDict = new Dictionary<int, Dictionary<AreaType, List<StageDataSO>>>();

    private StageGround currentStage = null;
    private StageDataSO currentStageData = null;

    private int currentFloor = 1;
    private int currentStageNumber = 0;

    public DoorDirType PassDir { get; set; } //전 스테이지에서 지나간 문 방향

    [SerializeField] private int MaxStage;
    [SerializeField] private string startStageID;
    [HideInInspector] public Vector2 respawnPos;

    //public Sprite openDoorSpr, closeDoorSpr;
    public Sprite[] doorSprites;
    public Dictionary<string, Sprite> doorSprDic = new Dictionary<string, Sprite>();

    public Transform stageParent;

    public bool IsStageClear { get; set; }
    public bool IsLastStage { get; set; }

    #region ValuableForEditor
    [Header("Test")]
    public string stageSOFolderName;
    #endregion


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
        foreach(StageBundleDataSO data in idToStageFloorDict.Values)
        {
            randomRoomDict.Add(data.floor, new Dictionary<AreaType, List<StageDataSO>>());
            for(int i=0; i<Global.EnumCount<AreaType>(); i++)
            {
                randomRoomDict[data.floor].Add((AreaType)i, new List<StageDataSO>());
            }
        }

        int cnt = Global.EnumCount<DoorDirType>();
        for(int i=0; i<cnt; i++)
        {
            doorSprDic.Add(((DoorDirType)i).ToString() + "Close", doorSprites[i]);
            doorSprDic.Add(((DoorDirType)i).ToString() + "Open", doorSprites[i + cnt]);
            doorSprDic.Add(((DoorDirType)i).ToString() + "Exit", doorSprites[i + cnt * 2]);
        }
    }

    private void Start()
    {
        InsertRandomMaps(currentFloor);
        Util.DelayFunc(() => NextStage(startStageID), 0.2f);
        respawnPos = idToStageDataDict[startStageID].stage.GetComponent<StageGround>().playerSpawnPoint.position;
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("StartNextStage", stageName => StartNextStage(stageName));
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
        List<string> stageIDList = new List<string>();
        StageBundleDataSO bundle = idToStageFloorDict[FloorToFloorID(floor)];

        foreach (StageDataSO data in bundle.stages)
        {
            stageIDList.Add(data.stageID);
        }
        foreach(AreaType type in System.Enum.GetValues(typeof(AreaType)))
        {
            randomRoomDict[floor][type].Clear();
        }

        for (int i = 0; i < bundle.randomStageList.Count; i++)
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
        }
    }

    public void NextStage(string id)
    {
        if (currentStage) currentStage.gameObject.SetActive(false);

        currentStageNumber++;
        currentStageData = idToStageDataDict[id];
        IsLastStage = currentStageData.endStage;
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

        currentStage.stageDoors.ForEach(x => 
        { 
            x.gameObject.SetActive(false);
            x.IsExitDoor = false;
        });

        //Player Position
        if(currentStage.playerSpawnPoint)
        {
            SlimeGameManager.Instance.CurrentPlayerBody.transform.position = currentStage.playerSpawnPoint.position;
        }
        else
        {
            SlimeGameManager.Instance.CurrentPlayerBody.transform.position = currentStage.GetOpposeDoor(PassDir).playerSpawnPos.position;
        }
        
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);  //Camera Move Range Set
        GameManager.Instance.ResetDroppedItems(); //Inactive Items

        int idx = 0;
        for (int i = 0; i< currentStageData.stageFloor.randomStageList[currentStageNumber - 1].nextStageTypes.Length; i++)
        {
            if (currentStage.stageDoors[i].gameObject.activeSelf) continue;

            try
            {
                currentStage.stageDoors[i].nextStageData = randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber - 1].nextStageTypes[idx]][0];
                randomRoomDict[currentFloor][currentStageData.stageFloor.randomStageList[currentStageNumber - 1].nextStageTypes[idx]].RemoveAt(0);
                ++idx;
                currentStage.stageDoors[i].gameObject.SetActive(true);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                Debug.Log($"스테이지를 불러오지 못함 {currentFloor} - {currentStageNumber} : i : {i}, idx: {idx}");
            }
        }

        switch (currentStageData.areaType)
        {
            case AreaType.START:
                currentStage.stageDoors.ForEach(x => x.gameObject.SetActive(true));
                SetClearStage();
                break;
            case AreaType.MONSTER:
                EventManager.TriggerEvent("SpawnEnemy", currentStageData.stageID);
                break;
            case AreaType.CHEF:
                SetClearStage();
                break;
            case AreaType.PLANTS:
                //채집구역이면 무엇을 할까
                break;
            case AreaType.RANDOM:
                --currentStageNumber;
                EnterRandomArea();
                break;
            case AreaType.BOSS:
                //보스구역이면 무엇을 할까
                break;
        }
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

    public void StartNextStage(string stageName = "")
    {
        if (!IsStageClear)
            EventManager.TriggerEvent("EnemyMove", currentStageData.stageID);
        EventManager.TriggerEvent("StartBGM", currentStageData.stageID);
    }

    public void StageClear()
    {
        IsStageClear = true;
        currentStage.OpenDoors();

        EventManager.TriggerEvent("StageClear");

        //CinemachineCameraScript.Instance.SetCinemachineConfiner(CinemachineCameraScript.Instance.boundingCollider);

        if (IsLastStage)
        {
            EventManager.TriggerEvent("GameClear");
        }
    }

    private void Respawn()
    {
        currentStageNumber = 0;
        InsertRandomMaps(currentFloor);
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
        RandomRoomType room = (RandomRoomType)Random.Range(0, Global.EnumCount<RandomRoomType>());

        switch(room)
        {
            case RandomRoomType.IMPRECATION:
                //저주 구역
                break;
            case RandomRoomType.MONSTER:  //몬스터 구역
                int targetStage = Mathf.Clamp(currentStageData.stageFloor.floor + Random.Range(-1, 2), 1, MaxStage); //현재 층에서 몇 층을 더할지 정함
                StageBundleDataSO sbData = idToStageFloorDict.Values.Find(x=>x.floor == targetStage); //현재 층에서 -1 or 0 or 1층을 더한 층을 가져온다
                NextStage(sbData.stages.FindRandom(stage => stage.areaType == AreaType.MONSTER).stageID); //뽑은 층에서 몬스터 지역들중에 랜덤으로 가져온다
                break;
            case RandomRoomType.RECOVERY:
                //회복 구역
                break;
        }
    }
}
