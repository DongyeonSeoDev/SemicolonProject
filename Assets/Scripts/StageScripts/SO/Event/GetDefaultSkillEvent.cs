using UnityEngine;

[CreateAssetMenu(fileName = "Get Default Skill Event", menuName = "Scriptable Object/Map Events/Get Default Skill Event", order = int.MaxValue)]
public class GetDefaultSkillEvent : MapEventSO  //Ʃ�丮�� 7���� ����
{

    public SubtitleData enterSubData;

    public override void OnEnterEvent()
    {
        TalkManager.Instance.SetSubtitle(enterSubData);
        BattleUIManager.Instance.StartMission(MissionType.ABSORPTIONTUTORIAL2);
        EventManager.TriggerEvent("SpawnEnemy", "Stage0-07");
    }
}
