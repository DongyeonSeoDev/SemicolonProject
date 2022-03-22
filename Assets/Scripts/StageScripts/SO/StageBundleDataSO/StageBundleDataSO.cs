using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Bundle Data", menuName = "Scriptable Object/Stage Bundle Data", order = int.MaxValue)]
public class StageBundleDataSO : ScriptableObject
{
    public string id => name;

    public int floor; 
    public string stageBundleName;
    public List<StageDataSO> stages;

    private Dictionary<string, StageDataSO> stageDic;
    public void SetStageDic()
    {
        stageDic = new Dictionary<string, StageDataSO>();
        for(int i = 0; i < stages.Count; i++)
        {
            stageDic.Add(stages[i].stageID, stages[i]);
        }
    }
    public StageDataSO GetStageData(string id)
    {
        if(stageDic.ContainsKey(id))
        {
            return stageDic[id];
        }
        return null;
    }
}
