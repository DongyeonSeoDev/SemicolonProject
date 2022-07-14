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
        TalkUtil.ShowSubtitle(startMapSubDataId);

        EventManager.StartListening("PlayerGetDamaged", PlayerDamaged);
        EventManager.TriggerEvent("SpawnEnemy","Stage0-03");
    }

    public void PlayerDamaged()
    {
        TalkUtil.ShowSubtitle(damagedSubDataId);
        EventManager.StopListening("PlayerGetDamaged", PlayerDamaged);
    }

}
