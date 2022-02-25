using UnityEngine;

public class StageSelect : MonoBehaviour
{
    public StageDataSO startStage;

    private GameObject currentStage;

    private void Start()
    {
        currentStage = Instantiate(startStage.stage, transform.position, Quaternion.identity);
    }

    public void NextStage(int nextStage)
    {
        currentStage.SetActive(false);
        startStage = startStage.nextStageList[nextStage];

        currentStage = Instantiate(startStage.stage, transform.position, Quaternion.identity);
    }
}
