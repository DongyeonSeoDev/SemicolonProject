using UnityEngine;

[CreateAssetMenu(fileName = "Mission Tutorial Event", menuName = "Scriptable Object/Map Events/Mission Tutorial Event", order = int.MaxValue)]
public class MissionTutorialEvent : MapEventSO
{
    public SubtitleData startMapSubData;
    public SubtitleData damagedSubData;

    public override void OnEnterEvent()
    {
        BattleUIManager.Instance.StartMission(MissionType.SURVIVAL1);
        TalkManager.Instance.SetSubtitle(startMapSubData);

        EventManager.StartListening("PlayerGetDamaged", PlayerDamaged);
        // 몬스터 두 마리 스폰
    }

    public void PlayerDamaged()
    {
        TalkManager.Instance.SetSubtitle(damagedSubData);
        EventManager.StopListening("PlayerGetDamaged", PlayerDamaged);
    }

}
