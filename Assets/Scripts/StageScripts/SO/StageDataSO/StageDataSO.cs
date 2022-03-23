using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public partial class StageDataSO : ScriptableObject
{
    public StageBundleDataSO stageFloor;

    //public bool endStage; //������ ������������

    //public int stageNumber;  //�������� ��ȣ(������)    n - stageNumber
    public string stageName;  //�������� �̸� (���� �̸�)

    public GameObject stage;  //�������� ������

    public AreaType areaType;  //���� Ÿ��

    public string stageID => name;  //�������� ���̵�

    public string StageName => stageName ?? Global.AreaTypeToString(areaType);

    public void SetPrefab()
    {
        GameObject go = Resources.Load<GameObject>("Stage/StagePrefab/Stage1/" + stageID + "Pref");
        if (go != null) stage = go;
    }

    
}