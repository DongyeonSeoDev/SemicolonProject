using UnityEngine;

[CreateAssetMenu(fileName = "Get Default Skill Event", menuName = "Scriptable Object/Map Events/Get Default Skill Event", order = int.MaxValue)]
public class GetDefaultSkillEvent : MapEventSO  //튜토리얼 7번방 입장
{

    public string enterSubDataId;

    public override void OnEnterEvent()
    {
        TalkUtil.ShowSubtitle(enterSubDataId);
        BattleUIManager.Instance.StartMission(MissionType.ABSORPTIONTUTORIAL2);
        EventManager.TriggerEvent("SpawnEnemy", "Stage0-07");
    }
}
