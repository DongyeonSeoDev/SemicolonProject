using UnityEngine.UI;
using UnityEngine;

public class ChoiceStatInfoElement : MonoBehaviour
{
    private ushort id;
    private ChoiceStatSO statData;

    public Text BGStatNameTxt;
    public Text BGStatExplanationTxt;
    public Text BGStatLvTxt;

    [SerializeField] private Button btn;

    public void InitSet(StatElement stat)
    {
        id = stat.id;
        statData = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);
        BGStatNameTxt.text = statData.statName;

        btn.onClick.AddListener(() =>
        {
            NGlobal.playerStatUI.DetailViewChoiceStatInfo(id);
        });

        gameObject.SetActive(stat.isUnlock);
    }

    public void UpdateUI()
    {
        BGStatLvTxt.text = NGlobal.playerStatUI.choiceStatDic[id].statLv.ToString();
        BGStatExplanationTxt.text = NGlobal.GetChoiceStatAbilExplanation(id);
    }
}
