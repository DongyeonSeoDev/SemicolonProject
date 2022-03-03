using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoSingleton<StageManager>
{
    private Dictionary<string, StageGround> idToStageObjDict = new Dictionary<string, StageGround>();
    private Dictionary<string, StageDataSO> idToStageDataDict = new Dictionary<string, StageDataSO>();

    private StageGround currentStage = null;

    [SerializeField] private bool startStageClearState;

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
        IsStageClear = startStageClearState;
    }

    public void NextStage(string id)
    {
        if (currentStage) currentStage.gameObject.SetActive(false);

        StageDataSO data = idToStageDataDict[id];
        IsLastStage = data.endStage;

        if (!idToStageObjDict.TryGetValue(id, out currentStage))
        {
            currentStage = Instantiate(data.stage, transform.position, Quaternion.identity, stageParent).GetComponent<StageGround>();
            idToStageObjDict.Add(id, currentStage);
        }
        else
        {
            currentStage = idToStageObjDict[id];
            currentStage.gameObject.SetActive(true);
        }

        IsStageClear = currentStage.EnemyCnt == 0;
        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = data.playerStartPosition;
        CinemachineCameraScript.Instance.SetCinemachineConfiner(currentStage.camStageCollider);
    }

    public void StageClear()
    {
        IsStageClear = true;
        currentStage.OpenDoor();

        EventManager.TriggerEvent("StageClear");

        CinemachineCameraScript.Instance.SetCinemachineConfiner(CinemachineCameraScript.Instance.boundingCollider);

        
    }
}
