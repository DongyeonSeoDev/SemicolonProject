using UnityEngine;
using UnityEngine.UI;

public class StageSelect : MonoSingleton<StageSelect>
{
    public StageDataSO startStage;

    [SerializeField]
    private GameObject uiObject;
    [SerializeField]
    private Button[] stageButton;

    private GameObject currentStage;
    private Text stageButtonText1;

    private void Start()
    {
        currentStage = Instantiate(startStage.stage, transform.position, Quaternion.identity);
        SlimeGameManager.Instance.CurrentPlayerBody.transform.position = startStage.playerStartPosition;

        stageButtonText1 = stageButton[1].GetComponentInChildren<Text>();

        for (int i = 0; i < stageButton.Length; i++)
        {
            int number = i;

            if (number == 1)
            {
                stageButton[number].onClick.AddListener(() =>
                {
                    if (startStage.nextStageList.Count == 1)
                    {
                        NextStage(0);
                    }
                    else
                    {
                        NextStage(1);
                    }
                });
            }
            else
            {
                stageButton[number].onClick.AddListener(() => NextStage(number));
            }
        }
    }

    public void ShowUI()
    {
        if (startStage.nextStageList.Count == 1)
        {
            for (int i = 0; i < stageButton.Length; i++)
            {
                stageButton[i].gameObject.SetActive(false);
            }

            stageButton[1].gameObject.SetActive(true);
            stageButtonText1.text = "1";
        }
        else
        {
            stageButtonText1.text = "2";

            for (int i = 0; i < stageButton.Length; i++)
            {
                stageButton[i].gameObject.SetActive(true);
            }
        }
        
        uiObject.SetActive(true);
    }

    public void NextStage(int nextStage)
    {
        uiObject.SetActive(false);

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
