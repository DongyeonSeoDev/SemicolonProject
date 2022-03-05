using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoSingleton<StageManager>
{
    private Dictionary<string, StageGround> idToStageObjDict = new Dictionary<string, StageGround>();
    private Dictionary<string, StageDataSO> idToStageDataDict = new Dictionary<string, StageDataSO>();

    private StageGround currentStage = null;

    [SerializeField] private bool startStageClearState;
    [SerializeField] private string startStageID;
    [HideInInspector] public Vector2 respawnPos;

    public Sprite openDoorSpr, closeDoorSpr;
    public Transform stageParent;

    public bool IsStageClear { get; set; }
    public bool IsLastStage { get; set; }


    private void Awake()
    {
        foreach(StageDataSO data in Resources.LoadAll<StageDataSO>("Stage/SO/"))
        {
            idToStageDataDict.Add(data.stageID, data);
        }
    }

    private void Start()
    {
        //IsStageClear = startStageClearState;
        Util.DelayFunc(() => NextStage(startStageID), 0.2f);
        respawnPos = idToStageDataDict[startStageID].playerStartPosition;
        EventManager.StartListening("PlayerRespawn", Respawn);
    }

    public void NextStage(string id)
    {
        if (currentStage) currentStage.gameObject.SetActive(false);

        StageDataSO data = idToStageDataDict[id];
        IsLastStage = data.endStage;
        currentStage = null;

        if (!idToStageObjDict.TryGetValue(id, out currentStage))
        {
            currentStage = Instantiate(data.stage, transform.position, Quaternion.identity, stageParent).GetComponent<StageGround>();
            idToStageObjDict.Add(id, currentStage);
        }
        else
        {
            currentStage.gameObject.SetActive(true);
        }

        IsStageClear = currentStage.EnemyCnt == 0;

        if (IsStageClear) currentStage.OpenDoors();
        else currentStage.CloseDoor();
        
        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = data.playerStartPosition;
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);
    }

    public void StageClear()
    {
        IsStageClear = true;
        currentStage.OpenDoors();

        EventManager.TriggerEvent("StageClear");

        CinemachineCameraScript.Instance.SetCinemachineConfiner(CinemachineCameraScript.Instance.boundingCollider);
    }

    private void Respawn(Vector2 unusedValue)
    {
        NextStage(startStageID);
    }
}
