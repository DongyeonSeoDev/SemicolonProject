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

    public bool Init()
    {
        SaveFileStream.LoadGameSaveData(saveFileName);

        changableBodyDataScript = FindObjectOfType<ChangableBodyDataScript>();
        saveData = SaveFileStream.GetSaveData(saveFileName, true);

        continueBtn.onClick.AddListener(() =>
        {
            OnStart();
            LoadSceneManager.Instance.LoadScene("StageScene");  //임시 코드
        });

        //여러가지 정보 UI 띄워줌. (저장 정보)
        UpdateTMPs();

        if(saveData.userInfo.currentBodyID == null)
        {
            return false;
        }

        return true;
    }

    public void OnStart()
    {
        SaveFileStream.currentSaveFileName = saveFileName;
    }
    public void UpdateTMPs()
    {
        Debug.Log(saveData.userInfo.currentBodyID);
   
        if (saveData.userInfo.currentBodyID == null)
        {
            currentStageTMP.text = "??";
            currentBodyNameTMP.text = "??";

            statTMPs.HPText.text = "HP: ??";
            statTMPs.DamageText.text = "Damage: ??";
            statTMPs.DPText.text = "Defense: ??";
            statTMPs.SpeedText.text = "Speed: ??";
        }
        else
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

            if (changableBodyDataScript.ChangableBodyNameDict.ContainsKey(saveData.userInfo.currentBodyID))
            {
                currentBodyNameTMP.text = changableBodyDataScript.ChangableBodyNameDict[saveData.userInfo.currentBodyID];
            }
            else
            {
                currentBodyNameTMP.text = "기본 슬라임";
            }
        }
    }
    public void OnDelete()
    {
        SaveFileStream.DeleteGameSaveData(saveFileName);

        //UI 갱신
    }
}
