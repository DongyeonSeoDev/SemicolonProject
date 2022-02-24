using System.Collections.Generic;
using UnityEngine;

public class StageSelect : MonoBehaviour
{
    [System.Serializable]
    public struct Stage
    {
        public GameObject stageObject;
        public bool isInstantiate;
    }

    public List<Stage> stageList = new List<Stage>();
    public int startStageId = 0;

    private int currentStageId = 0;

    private void Start()
    {
        for (int i = 0; i < stageList.Count; i++)
        {
            if (!stageList[i].isInstantiate)
            {
                Stage stage = new Stage();

                stage.stageObject = Instantiate(stageList[i].stageObject, transform.position, Quaternion.identity);
                stage.stageObject.SetActive(false);
                stage.isInstantiate = true;

                stageList[i] = stage;
            }

            if (stageList[i].stageObject.activeSelf)
            {
                stageList[i].stageObject.SetActive(false);
            }
        }

        stageList[startStageId].stageObject.SetActive(true);

        currentStageId = startStageId;
    }

    public void NextStage(int stageId)
    {
        stageList[currentStageId].stageObject.SetActive(false);
        stageList[stageId].stageObject.SetActive(true);

        currentStageId = stageId;
    }
}
