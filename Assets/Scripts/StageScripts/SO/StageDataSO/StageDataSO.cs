using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public partial class StageDataSO : ScriptableObject
{
    public StageBundleDataSO stageFloor;

    //public bool endStage; //마지막 스테이지인지

    //public int stageNumber;  //스테이지 번호(구역당)    n - stageNumber
    public string stageName;  //스테이지 이름 (지역 이름)

    public GameObject stage;  //스테이지 프리팹

    public AreaType areaType;  //구역 타입

    public string stageID => name;  //스테이지 아이디

    public string StageName => stageName ?? Global.AreaTypeToString(areaType);

    public void SetPrefab()
    {
        GameObject go = Resources.Load<GameObject>("Stage/StagePrefab/Stage1/" + stageID + "Pref");
        if (go != null) stage = go;
    }

    
}