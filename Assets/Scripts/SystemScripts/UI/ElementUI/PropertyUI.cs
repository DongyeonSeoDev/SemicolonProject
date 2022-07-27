using UnityEngine;
using TMPro;

public class PropertyUI : MonoBehaviour  //���UI�� ��� ȭ�鿡 ���̴� Ư�� (���� ����) UI ���
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
