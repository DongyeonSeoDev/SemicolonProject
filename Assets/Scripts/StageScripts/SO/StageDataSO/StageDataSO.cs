using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public partial class StageDataSO : ScriptableObject
{
    public bool endStage; //������ ������������

    public int stageBigNumber;  //�������� ��ȣ(ū ������ �������� ��)    stageBigNumber - n
    public int stageNumber;  //�������� ��ȣ(������)    n - stageNumber
    public string stageName;  //�������� �̸� (���� �̸�)

    public GameObject stage;  //�������� ������

    public AreaType areaType;  //���� Ÿ��

    public string stageID => name;  //�������� ���̵�

    public void SetPrefab()
    {
        GameObject go = Resources.Load<GameObject>("Stage/StagePrefab/Stage1/" + stageID + "Pref");
        if (go != null) stage = go;
    }
}