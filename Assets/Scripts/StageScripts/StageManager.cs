using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoSingleton<StageManager>
{
    private Dictionary<string, StageGround> idToStageObjDict = new Dictionary<string, StageGround>();
    private Dictionary<string, StageDataSO> idToStageDataDict = new Dictionary<string, StageDataSO>();

    private StageGround currentStage = null;
    private StageDataSO currentStageData = null;

    [SerializeField] private string startStageID;
    [HideInInspector] public Vector2 respawnPos;

    public Sprite openDoorSpr, closeDoorSpr;
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

        IsStageClear = currentStageData.enemyCount == 0;

        if (IsStageClear) currentStage.OpenDoors();
        else currentStage.CloseDoor();

        Enemy.EnemyManager.Instance.enemyCount = currentStageData.enemyCount;
        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = currentStage.playerSpawnPoint.position;
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);
        GameManager.Instance.ResetDroppedItems();

        if(!IsStageClear)
           EventManager.TriggerEvent("SpawnEnemy", currentStageData.stageID);
    }

    public void StartNextStage(string stageName = "")
    {
        if (!IsStageClear)
            EventManager.TriggerEvent("EnemyMove", currentStageData.stageID);
    }

    public void StageClear()
    {
        IsStageClear = true;
        currentStage.OpenDoors();

        EventManager.TriggerEvent("StageClear");

        CinemachineCameraScript.Instance.SetCinemachineConfiner(CinemachineCameraScript.Instance.boundingCollider);

        if (IsLastStage)
        {
            EventManager.TriggerEvent("GameClear");
        }
    }

    private void Respawn(Vector2 unusedValue)
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
}
