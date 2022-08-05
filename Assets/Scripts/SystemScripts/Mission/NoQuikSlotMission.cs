
public class NoQuikSlotMission : Mission 
{
    public NoQuikSlotMission(string title) : base(title)
    {
        missionType = MissionType.NOQUIKSLOT;
    }
    public override void End(bool breakDoor = false)
    {
        //InteractionHandler.canUseQuikSlot = true;

        EventManager.StopListening("UseQuikSlot", MissionFailure);
        EventManager.StopListening("StageClear", MissionSuccess);
    }

    public override void Start()
    {
        base.Start();
        //InteractionHandler.canUseQuikSlot = false;

        EventManager.StartListening("UseQuikSlot", MissionFailure);
        EventManager.StartListening("StageClear", MissionSuccess);
    }

    public override void Update()
    {
        /*if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.ITEM_QUIKSLOT]) && !TimeManager.IsTimePaused && !string.IsNullOrEmpty(KeyActionManager.Instance.QuikItemId))
        {
            BattleUIManager.Instance.ShakeMissionPanel();
        }*/
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = DifficultyLevel.NORMAL;
    }

    public override void MissionFailure()
    {
        base.MissionFailure();
    }

    public override void MissionSuccess()
    {
        base.MissionSuccess();
    }
}
