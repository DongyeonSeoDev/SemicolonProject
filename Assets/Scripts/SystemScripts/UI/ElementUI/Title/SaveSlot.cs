using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    private ChangableBodyDataScript changableBodyDataScript = null;
    [SerializeField] private string saveFileName;
    private SaveData saveData = null;

    public Button continueBtn;

    [Serializable]
    private struct StatTMPs
    {
        public TextMeshProUGUI HPText;
        public TextMeshProUGUI DamageText;
        public TextMeshProUGUI DPText;
        public TextMeshProUGUI SpeedText;
    }

    [SerializeField]
    private TextMeshProUGUI currentStageTMP = null;
    [SerializeField]
    private TextMeshProUGUI currentBodyNameTMP = null;
    [SerializeField]
    private StatTMPs statTMPs = new StatTMPs();

    public bool IsEmptySlot => !SaveFileStream.HasSaveData(saveFileName);

    #region LoadingWindowData
    [SerializeField]
    private GameObject loadingWindowObj = null;

    public Image progressBar = null;
    public TextMeshProUGUI progressText = null;
    #endregion

    private void Awake()
    {
        continueBtn.onClick.AddListener(() =>
        {
            loadingWindowObj.SetActive(true);

            OnStart();
            LoadSceneManager.Instance.LoadScene(progressBar, progressText, "StageScene");
        });
    }

    public bool Init()
    {
        SaveFileStream.LoadGameSaveData(saveFileName);

        changableBodyDataScript = FindObjectOfType<ChangableBodyDataScript>();
        saveData = SaveFileStream.GetSaveData(saveFileName, true);

        Debug.Log(SaveFileStream.HasSaveData(saveFileName));

        //여러가지 정보 UI 띄워줌. (저장 정보)
        UpdateTMPs();

        return SaveFileStream.HasSaveData(saveFileName);
    }

    public void OnStart()
    {
        SaveFileStream.currentSaveFileName = saveFileName;
    }
    public void UpdateTMPs()
    {
        if(saveData.userInfo.currentBodyID == null || saveData.userInfo.currentBodyID == "")
        {
            saveData.userInfo.currentBodyID = "origin";
        }
   
        if (SaveFileStream.HasSaveData(saveFileName))
        {
            if (saveData.tutorialInfo.isEnded)
            {
                currentStageTMP.text = saveData.stageInfo.currentStageID;

                Stat stat = saveData.userInfo.playerStat;

                statTMPs.HPText.text = "HP: " + stat.MaxHp;
                statTMPs.DamageText.text = "Damage: " + stat.MinDamage + " ~ " + stat.MaxDamage;
                statTMPs.DPText.text = "Defense: " + stat.Defense;
                statTMPs.SpeedText.text = "Speed: " + stat.Speed;
            }
            else
            {
                currentStageTMP.text = "TutorialStage";

                statTMPs.HPText.text = "HP: ??";
                statTMPs.DamageText.text = "Damage: ??";
                statTMPs.DPText.text = "Defense: ??";
                statTMPs.SpeedText.text = "Speed: ??";
            }

            if (saveData.userInfo.currentBodyID == "origin") // origin 관련 작업
            {
                currentBodyNameTMP.text = "기본 슬라임";
            }
            else if (changableBodyDataScript.ChangableBodyNameDict.ContainsKey(saveData.userInfo.currentBodyID))
            {
                
                currentBodyNameTMP.text = changableBodyDataScript.ChangableBodyNameDict[saveData.userInfo.currentBodyID].name;
            }
        }
        else
        {
            currentStageTMP.text = "??";
            currentBodyNameTMP.text = "??";

            statTMPs.HPText.text = "HP: ??";
            statTMPs.DamageText.text = "Damage: ??";
            statTMPs.DPText.text = "Defense: ??";
            statTMPs.SpeedText.text = "Speed: ??";
        }
    }
    public void OnDelete()
    {
        SaveFileStream.DeleteGameSaveData(saveFileName);

        //UI 갱신
    }
}
