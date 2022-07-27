using UnityEngine;
using TMPro;

public class PropertyUI : MonoBehaviour  //상시UI로 평소 화면에 보이는 특성 (선택 스탯) UI 요소
{
    public TextMeshProUGUI lvTmp;
    public TextMeshProUGUI nameTmp;

    public ushort ID { get; private set; }

    public void Set(StatElement stat)
    {
        ID = stat.id;
        lvTmp.SetText(stat.statLv.ToString());
        nameTmp.SetText(NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(ID).statName);
    }
}
