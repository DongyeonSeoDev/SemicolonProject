using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EternalStatInfoData", menuName = "Scriptable Object/EternalStat Info Data", order = int.MaxValue)]
public class EternalStatSO : StatSO
{
    public bool isPercent;

    public float minStatValue;  //�ش� ������ �ּڰ�
    public float initStatAfterOpen; //������ ������ �Ŀ� �ʱⰪ
    //public List<Pair<float, string>> emotionRangeList = new List<Pair<float, string>>();  //���� ��ġ�� ���� ����
}
