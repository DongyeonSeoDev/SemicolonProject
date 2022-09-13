using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Bundle Data", menuName = "Scriptable Object/Stage Bundle Data", order = int.MaxValue)]
public class StageBundleDataSO : ScriptableObject
{
    public string id => name;

    public int floor; 
    public string stageBundleName;
    public List<StageDataSO> stages;  //여기에는 랜덤맵 전용 몬스터맵을 집어넣으면 안됨.
    public List<StageDataSO> monsterStages;  //여기에는 랜덤맵 전용 몬스터맵들까지 다 넣어줌
    public List<StageFork> randomStageList;
    public StageDataSO nextStageSO;

    public int LastStageNumber => randomStageList.Count;

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
        Debug.Log("해당 스테이지를 현재 층에서 못찾음 : " + id);
        return null;
    }
}
