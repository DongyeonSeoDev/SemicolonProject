using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Pick Tutorial Event", menuName = "Scriptable Object/Map Events/Pick Tutorial Event", order = int.MaxValue)]
public class PickTutorialEvent : MapEventSO
{
    public SubtitleData enterSubData;
    public SubtitleData success, fail;

    public override void OnEnterEvent()
    {
        TalkManager.Instance.SetSubtitle(enterSubData);
        InteractionHandler.canUseQuikSlot = false;
        EventManager.StartListening(Global.PickupPlant, Pickup);
    }

    private void Pickup(string id, bool suc)
    {
        if(!suc)
        {
            Inventory.Instance.GetItem(new ItemInfo(id, 1));
        }
        
        TalkManager.Instance.SetSubtitle(suc ? success : fail);
        EventManager.StopListening(Global.PickupPlant, Pickup);
    }
}
