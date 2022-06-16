using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public partial class StageDataSO : ScriptableObject
{
    public StageBundleDataSO stageFloor;

    //public bool endStage; //마지막 스테이지인지

    //public int stageNumber;  //스테이지 번호(구역당)    n - stageNumber
    public string stageName;  //스테이지 이름 (지역 이름)

    public bool isSaveStage; //이 스테이지 도착하면 저장할까
    public bool useOpenDoorSound;  //문 사운드 사용할까

    public int stageMonsterBundleCount;  //총 몇 세트의 몹들이 나오는지
    [HideInInspector] public string[] stageMonsterBundleID;

    public GameObject stage;  //스테이지 프리팹

    public AreaType areaType;  //구역 타입
    public EnemySpecies enemySpeciesArea = EnemySpecies.NONE;
    public List<MissionType> missionTypes; //나타날 수 있는 미션들 (몬스터 구역 한정)

    public GameObject mapNPC;

    public MapEventSO mapEvent;

    public string stageID => name;  //스테이지 아이디

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