using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public partial class StageDataSO : ScriptableObject
{
    public StageBundleDataSO stageFloor;

    //public bool endStage; //������ ������������

    //public int stageNumber;  //�������� ��ȣ(������)    n - stageNumber
    public string stageName;  //�������� �̸� (���� �̸�)

    public int stageMonsterBundleCount;
    [HideInInspector] public string[] stageMonsterBundleID;

    public GameObject stage;  //�������� ������

    public AreaType areaType;  //���� Ÿ��
    public EnemySpecies enemySpeciesArea = EnemySpecies.NONE;

    public GameObject mapNPC;

    public MapEventSO mapEvent;

    public string stageID => name;  //�������� ���̵�

    public string StageName => string.IsNullOrEmpty( stageName) ? Global.AreaTypeToString(areaType) : stageName;

    public void SetPrefab()
    {
        GameObject go = Resources.Load<GameObject>("Stage/StagePrefab/Stage1/" + stageID + "Pref");
        if (go != null) stage = go;
    }

    public void SetStageMonsterBundleID()
    {
        if (stageMonsterBundleCount > 0)
        {
            stageMonsterBundleID = new string[stageMonsterBundleCount];
            for (int i = 0; i < stageMonsterBundleCount; i++)
            {
                stageMonsterBundleID[i] = string.Concat(stageID, '-', i + 1);
            }
        }
    }
}