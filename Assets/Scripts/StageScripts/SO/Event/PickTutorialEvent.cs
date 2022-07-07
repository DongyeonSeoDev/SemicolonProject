using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Pick Tutorial Event", menuName = "Scriptable Object/Map Events/Pick Tutorial Event", order = int.MaxValue)]
public class PickTutorialEvent : MapEventSO
{
    public string enterSubDataID;
    public string successId, failId;

    public override void OnEnterEvent()
    {
        TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle(enterSubDataID));
        InteractionHandler.canUseQuikSlot = false;
        EventManager.StartListening(Global.PickupPlant, Pickup);
    }

    private void Pickup(string id, bool suc)
    {
        if(!suc)
        {
            Inventory.Instance.GetItem(new ItemInfo(id, 1));
        }
        
        string tId = suc ? successId : failId;
        TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle(tId));
        EventManager.StopListening(Global.PickupPlant, Pickup);
    }
}
