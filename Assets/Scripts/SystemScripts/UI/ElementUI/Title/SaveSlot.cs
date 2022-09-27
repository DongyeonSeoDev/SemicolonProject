using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    //private ChangableBodyDataScript changableBodyDataScript = null;
    [SerializeField] private int saveFileIndex;
    private string saveFileName => Global.GetSaveFileName(saveFileIndex);
    private SaveData saveData = null;

    public Button gameStartBtn;
    private TextMeshProUGUI gameStartBtnText = null;

    public Button deleteSaveDataBtn = null;

    [Serializable]
    private struct StatTMPs
    {
        public TextMeshProUGUI HPText;
        public TextMeshProUGUI DamageText;
        public TextMeshProUGUI DPText;
        public TextMeshProUGUI SpeedText;
    }

    [SerializeField]
    private GameObject userInfoTextsParent;

    [SerializeField]
    private TextMeshProUGUI newSlotTMP;

    [SerializeField]
    private TextMeshProUGUI currentStageTMP = null;
    [SerializeField]
    private TextMeshProUGUI currentStatPointTMP = null;
    [SerializeField]
    private TextMeshProUGUI lastConnectionDateTMP = null;
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
        gameStartBtn.onClick.AddListener(() =>
        {
            loadingWindowObj.SetActive(true);

            OnStart();
            LoadSceneManager.Instance.LoadScene(progressBar, progressText, "StageScene");
        });
    }
    public bool Init()
    {
        SaveFileStream.LoadGameSaveData(saveFileName);

        gameStartBtnText = gameStartBtn.transform.GetComponentInChildren<TextMeshProUGUI>();
        //changableBodyDataScript = FindObjectOfType<ChangableBodyDataScript>();
        saveData = SaveFileStream.GetSaveData(saveFileName, true);

        //�������� ���� UI �����. (���� ����)
        UpdateTMPs();

        return SaveFileStream.HasSaveData(saveFileName);
    }

    public void OnStart()
    {
        SaveFileStream.currentSaveFileName = saveFileName;
    }
    public void UpdateTMPs()
    {
        /*if(saveData.userInfo.currentBodyID == null || saveData.userInfo.currentBodyID == "")
        {
            saveData.userInfo.currentBodyID = "origin";
        }*/
   
        if (SaveFileStream.HasSaveData(saveFileName))
        {
            gameStartBtnText.text = "�̾��ϱ�";
            userInfoTextsParent.SetActive(true);
            deleteSaveDataBtn.gameObject.SetActive(true);
            newSlotTMP.gameObject.SetActive(false);

            currentStageTMP.text = StageInfoTextAsset.Instance.GetValue(saveData.stageInfo.currentStageID);

            string[] dateStrs = saveData.option.lastPlayDate.Split(':');
            lastConnectionDateTMP.text = string.Format("{0}/{1}/{2} ({3}:{4})", dateStrs[0], dateStrs[1], dateStrs[2], dateStrs[3], dateStrs[4]);

            if (saveData.tutorialInfo.isEnded)
            {
                Stat stat = saveData.userInfo.playerStat;

                currentStatPointTMP.text = "<color=#E8EC75>" + (stat.currentStatPoint + stat.accumulateStatPoint).ToString() + "</color> POINT";

                /*statTMPs.HPText.text = "HP: " + stat.MaxHp;
                statTMPs.DamageText.text = "Damage: " + stat.MinDamage + " ~ " + stat.MaxDamage;
                statTMPs.DPText.text = "Defense: " + stat.Defense;
                statTMPs.SpeedText.text = "Speed: " + stat.Speed;*/
            }
            else
            {
                currentStatPointTMP.text = "<color=#E8EC75>0</color> POINT";

                /*statTMPs.HPText.text = "HP: ??";
                statTMPs.DamageText.text = "Damage: ??";
                statTMPs.DPText.text = "Defense: ??";
                statTMPs.SpeedText.text = "Speed: ??";*/
            }

            /*if (saveData.userInfo.currentBodyID == "origin") // origin ���� �۾�
            {
                currentBodyNameTMP.text = "�⺻ ������";
            }
            else if (changableBodyDataScript.ChangableBodyNameDict.ContainsKey(saveData.userInfo.currentBodyID))
            {
                
                currentBodyNameTMP.text = changableBodyDataScript.ChangableBodyNameDict[saveData.userInfo.currentBodyID].name;
            }*/
        }
        else
        {
            gameStartBtnText.text = "���ν���";
            userInfoTextsParent.SetActive(false);
            deleteSaveDataBtn.gameObject.SetActive(false);
            newSlotTMP.gameObject.SetActive(true);

            /*currentStageTMP.text = "??";
            currentStatPointTMP.text = "??";
            lastConnectionDateTMP.text = "????/??/?? (??:??)";*/
            //currentBodyNameTMP.text = "??";

            /*statTMPs.HPText.text = "HP: ??";
            statTMPs.DamageText.text = "Damage: ??";
            statTMPs.DPText.text = "Defense: ??";
            statTMPs.SpeedText.text = "Speed: ??";*/
        }
    }
    public void OnDelete()
    {
        SaveFileStream.DeleteGameSaveData(saveFileName);

        //UI ����
    }
}
