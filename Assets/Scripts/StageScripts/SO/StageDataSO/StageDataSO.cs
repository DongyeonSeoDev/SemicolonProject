using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = 0)]
public partial class StageDataSO : ScriptableObject
{
    //public List<StageDataSO> nextStageList = new List<StageDataSO>();
      
    public bool endStage;

    public int stageNumber;
    public string stageName;

    public string stageID;

    public GameObject stage;
    public Vector3 playerStartPosition;
}