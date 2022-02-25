using System.Collections.Generic;
using UnityEngine;

public partial class StageDataSO : ScriptableObject
{
    public List<StageDataSO> nextStageList = new List<StageDataSO>();
    public GameObject stage;
}