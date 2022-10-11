using UnityEngine;

[CreateAssetMenu(fileName = "Mission Tutorial Event", menuName = "Scriptable Object/Map Events/Mission Tutorial Event", order = int.MaxValue)]
public class MissionTutorialEvent : MapEventSO
{
    //public SubtitleData startMapSubData;
    //public SubtitleData damagedSubData;

    public string startMapSubDataId;
    public string damagedSubDataId;

    private bool isHit;

    public override void OnEnterEvent()
    {
        isHit = false;
        BattleUIManager.Instance.StartMission(MissionType.SURVIVAL1, DifficultyLevel.EASY);
        TalkUtil.ShowSubtitle(startMapSubDataId);

        EventManager.StartListening("PlayerGetDamaged", PlayerDamaged);
        EventManager.StartListening("ExitCurrentMap", StartNextStage);
        EventManager.TriggerEvent("SpawnEnemy","Stage0-03");
    }

    public void PlayerDamaged()
    {
        isHit = true;
        TalkUtil.ShowSubtitle(damagedSubDataId);
        EventManager.StopListening("PlayerGetDamaged", PlayerDamaged);
    }

    void StartNextStage()
    {
        if (!isHit)
        {
            EventManager.StopListening("PlayerGetDamaged", PlayerDamaged);
        }

        EventManager.StopListening("ExitCurrentMap", StartNextStage);
    }
}
