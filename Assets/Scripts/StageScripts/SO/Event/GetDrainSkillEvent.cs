using UnityEngine;

[CreateAssetMenu(fileName = "Get Drain Skill Event", menuName = "Scriptable Object/Map Events/Get Drain Skill Event", order = int.MaxValue)]
public class GetDrainSkillEvent : MapEventSO
{
    public string enterDialogKey;

    public override void OnEnterEvent()
    {
        TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle(enterDialogKey));
    }
}
