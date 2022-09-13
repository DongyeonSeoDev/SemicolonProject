using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChoiceStatInfoData", menuName = "Scriptable Object/ChoiceStat Info Data", order = int.MaxValue)]
public class ChoiceStatSO : StatSO
{
    public CharType charType;  //특성 타입

    public ushort needStatID;  //헤당 스탯을 얻기 위해서 필요한 (고정)스탯의 아이디  -> 0이면 걍 얻을 수 있음

    public int purchase;  //구매 비용
    public int sell;  //1렙일 떄의 판매비용
    public int upCost; //레벨 오를 때마다 판매비용 얼마나 상승하는지

    public string simpleAbilExplanation;
    [TextArea]
    public string detailAbilExplanation;
    [TextArea]
    public string growthWay;
    [TextArea]
    public string acquisitionWay;
    //public List<Pair<float, string>> emotionRangeList = new List<Pair<float, string>>();  //스탯 수치에 따른 감정
}
