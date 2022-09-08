using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChoiceStatInfoData", menuName = "Scriptable Object/ChoiceStat Info Data", order = int.MaxValue)]
public class ChoiceStatSO : StatSO
{
    public CharType charType;  //Ư�� Ÿ��

    public int purchase;  //���� ���
    public int sell;  //1���� �Ǹ� ���

    public string simpleAbilExplanation;
    [TextArea]
    public string detailAbilExplanation;
    [TextArea]
    public string growthWay;
    [TextArea]
    public string acquisitionWay;
    //public List<Pair<float, string>> emotionRangeList = new List<Pair<float, string>>();  //���� ��ġ�� ���� ����
}
