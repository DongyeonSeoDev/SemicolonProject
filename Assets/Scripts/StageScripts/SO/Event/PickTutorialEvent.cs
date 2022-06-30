using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Pick Tutorial Event", menuName = "Scriptable Object/Map Events/Pick Tutorial Event", order = int.MaxValue)]
public class PickTutorialEvent : MapEventSO
{
    public SubtitleData enterSubData;

    public override void OnEnterEvent()
    {
        TalkManager.Instance.SetSubtitle(enterSubData);
        EventManager.StartListening(Global.PickupPlant, (Action<bool>)Pickup);
    }

    private void Pickup(bool suc)
    {
        

        EventManager.StopListening(Global.PickupPlant, (Action<bool>)Pickup);
    }
}
