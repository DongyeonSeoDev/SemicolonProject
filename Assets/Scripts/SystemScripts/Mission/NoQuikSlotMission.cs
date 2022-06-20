using UnityEngine;

public class NoQuikSlotMission : Mission
{
    public NoQuikSlotMission(string title) : base(title)
    {
        missionType = MissionType.NOQUIKSLOT;
    }
    public override void End(bool breakDoor = false)
    {
        InteractionHandler.canUseQuikSlot = true;
    }

    public override void Start()
    {
        isEnd = false;
        InteractionHandler.canUseQuikSlot = false;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.ITEM_QUIKSLOT]) && !TimeManager.IsTimePaused && !string.IsNullOrEmpty(KeyActionManager.Instance.QuikItemId))
        {
            BattleUIManager.Instance.ShakeMissionPanel();
        }
    }
}
