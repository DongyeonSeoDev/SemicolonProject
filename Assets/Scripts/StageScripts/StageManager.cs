using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoSingleton<StageManager>
{
    private Dictionary<string, StageGround> idToStageObjDict = new Dictionary<string, StageGround>();
    private Dictionary<string, StageDataSO> idToStageDataDict = new Dictionary<string, StageDataSO>();
    private Dictionary<string, StageBundleDataSO> idToStageFloorDict = new Dictionary<string, StageBundleDataSO>();

    private StageGround currentStage = null;
    private StageDataSO currentStageData = null;

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
        foreach(StageDataSO data in Resources.LoadAll<StageDataSO>("Stage/SO/"))
        {
            idToStageDataDict.Add(data.stageID, data);
        }
        foreach(StageBundleDataSO data in Resources.LoadAll<StageBundleDataSO>("Stage/SO/BundleSO"))
        {
            idToStageFloorDict.Add(data.id, data);
            data.SetStageDic();
        }

        int cnt = Global.EnumCount<DoorDirType>();
        for(int i=0; i<cnt; i++)
        {
            doorSprDic.Add(((DoorDirType)i).ToString() + "Close", doorSprites[i]);
            doorSprDic.Add(((DoorDirType)i).ToString() + "Open", doorSprites[i + cnt]);
        }
    }

    private void Start()
    {
        Util.DelayFunc(() => NextStage(startStageID), 0.2f);
        respawnPos = idToStageDataDict[startStageID].stage.GetComponent<StageGround>().playerSpawnPoint.position;
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("StartNextStage", stageName => StartNextStage(stageName));
    }

    public void NextStage(string id)
    {
        if (currentStage) currentStage.gameObject.SetActive(false);

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

        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = currentStage.playerSpawnPoint.position; //Player Pos
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);  //Cam Set
        GameManager.Instance.ResetDroppedItems(); //Inactive Items

        switch (currentStageData.areaType)
        {
            case AreaType.START:
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
                List<StageDataSO> sDatas = sbData.stages.FindAll(stage=>stage.areaType==AreaType.MONSTER); //해당 층의 몬스터 스테이지들만 가져옴
                NextStage(sDatas[Random.Range(0, sDatas.Count)].stageID);
                break;
            case RandomRoomType.RECOVERY:
                //회복 구역
                break;
        }
    }
}
