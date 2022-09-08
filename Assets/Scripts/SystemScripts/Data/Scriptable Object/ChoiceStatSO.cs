using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChoiceStatInfoData", menuName = "Scriptable Object/ChoiceStat Info Data", order = int.MaxValue)]
public class ChoiceStatSO : StatSO
{
    public CharType charType;  //특성 타입

    public int purchase;  //구매 비용
    public int sell;  //1렙당 판매 비용

    public string simpleAbilExplanation;
    [TextArea]
    public string detailAbilExplanation;
    [TextArea]
    public string growthWay;
    [TextArea]
    public string acquisitionWay;
    //public List<Pair<float, string>> emotionRangeList = new List<Pair<float, string>>();  //스탯 수치에 따른 감정
}
