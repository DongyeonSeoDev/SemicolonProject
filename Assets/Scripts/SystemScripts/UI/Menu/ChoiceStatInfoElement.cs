using UnityEngine.UI;
using UnityEngine;

public class ChoiceStatInfoElement : MonoBehaviour
{
    private ushort id;
    private StatElement choice;
    private ChoiceStatSO statData;

    public Text BGStatNameTxt;
    public Text BGStatExplanationTxt;
    public Text BGStatLvTxt;

    [SerializeField] private Button btn;

    public void InitSet(StatElement stat)
    {
        id = stat.id;
        choice = stat;
        statData = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);

        BGStatNameTxt.text = statData.statName;
        BGStatExplanationTxt.text = statData.simpleAbilExplanation;

        btn.onClick.AddListener(() =>
        {
            NGlobal.playerStatUI.DetailViewChoiceStatInfo(id);
        });

        gameObject.SetActive(stat.isUnlock);
    }

    public void UpdateUI()
    {
        BGStatLvTxt.text = choice.statLv.ToString();
    }
}
