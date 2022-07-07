using UnityEngine;

[CreateAssetMenu(fileName = "Mission Tutorial Event", menuName = "Scriptable Object/Map Events/Mission Tutorial Event", order = int.MaxValue)]
public class MissionTutorialEvent : MapEventSO
{
    //public SubtitleData startMapSubData;
    //public SubtitleData damagedSubData;

    public string startMapSubDataId;
    public string damagedSubDataId;

    public override void OnEnterEvent()
    {
        BattleUIManager.Instance.StartMission(MissionType.SURVIVAL1);
        TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle(startMapSubDataId));

        EventManager.StartListening("PlayerGetDamaged", PlayerDamaged);
        EventManager.TriggerEvent("SpawnEnemy","Stage0-03");
    }

    public void PlayerDamaged()
    {
        TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle(damagedSubDataId));
        EventManager.StopListening("PlayerGetDamaged", PlayerDamaged);
    }

}
