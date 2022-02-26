using UnityEngine;

public class StageSelect : MonoBehaviour
{
    public StageDataSO startStage;

    private GameObject currentStage;

    private void Start()
    {
        currentStage = Instantiate(startStage.stage, transform.position, Quaternion.identity);
        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = startStage.playerStartPosition;
    }

    public void NextStage(int nextStage)
    {
        if (startStage.nextStageList.Count <= nextStage)
        {
            return;
        }

        currentStage.SetActive(false);
        startStage = startStage.nextStageList[nextStage];

        currentStage = Instantiate(startStage.stage, transform.position, Quaternion.identity);
        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = startStage.playerStartPosition;
    }
}
