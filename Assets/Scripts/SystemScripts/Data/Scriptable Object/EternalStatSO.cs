using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EternalStatInfoData", menuName = "Scriptable Object/EternalStat Info Data", order = int.MaxValue)]
public class EternalStatSO : StatSO
{
    public float minStatValue;  //해당 스탯의 최솟값
    //public List<Pair<float, string>> emotionRangeList = new List<Pair<float, string>>();  //스탯 수치에 따른 감정
}
