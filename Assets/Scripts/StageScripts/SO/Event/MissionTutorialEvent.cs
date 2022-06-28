using UnityEngine;

[CreateAssetMenu(fileName = "Mission Tutorial Event", menuName = "Scriptable Object/Mission Tutorial Event", order = int.MaxValue)]
public class MissionTutorialEvent : MapEventSO
{
    public SubtitleData startMapSubData;
    public SubtitleData damagedSubData;

    public override void OnEnterEvent()
    {
        BattleUIManager.Instance.StartMission(MissionType.SURVIVAL1);
        TalkManager.Instance.SetSubtitle(startMapSubData);
    }

    public void PlayerDamaged()
    {
        TalkManager.Instance.SetSubtitle(damagedSubData);
    }

}
