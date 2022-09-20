using UnityEngine.UI;
using UnityEngine;
using System.Text;

public class ChoiceStatInfoElement : MonoBehaviour
{
    private ushort id;
    private StatElement choice;
    private ChoiceStatSO statData;

    public Text BGStatNameTxt;
    public Text BGStatExplanationTxt;
    public Text BGStatLvTxt;

    public Image statImg;

    public Image expFillImg;

    [SerializeField] private Button btn;

    public void InitSet(StatElement stat)
    {
        id = stat.id;
        choice = stat;
        statData = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);

        statImg.sprite = stat.StatSprite;
        statImg.color = UtilValues.Gray100;

        //BGStatNameTxt.text = statData.statName;
        //BGStatExplanationTxt.text = statData.simpleAbilExplanation;

        btn.onClick.AddListener(() => NGlobal.playerStatUI.DetailViewChoiceStatInfo(id));

        gameObject.SetActive(stat.isUnlock);
    }

    public void UpdateUI()
    {
        if (choice.isUnlock)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=#");
            sb.Append(choice.statLv < choice.maxStatLv ? "E9FF34>" : "747474>");
            sb.Append(choice.statLv.ToString());
            sb.Append("</color>  ");
            sb.Append(statData.statName);

            BGStatLvTxt.text = sb.ToString();

            expFillImg.fillAmount = Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().GetGrowthRate(id);
        }
    }
}
