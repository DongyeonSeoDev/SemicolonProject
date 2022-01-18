using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Water;

[AddComponentMenu("Custom/UIPointerScaleRepeat")]
[RequireComponent(typeof(EventTrigger))]
public class UIScaleRepeat : UITransition
{
    private EventTrigger eventTrigger;

    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener(eventData => Transition(true));

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener(eventData => Transition(false));

        eventTrigger.triggers.Add(entry1);
        eventTrigger.triggers.Add(entry2);
    }

    public override void Transition(bool on)
    {
        if (on)
        {
            transform.DOScale(Global.onePointTwo, Global.fullScaleTransitionTime05).SetLoops(-1,LoopType.Yoyo).SetUpdate(true);
        }
        else
        {
            transform.DOKill();
            transform.DOScale(Vector3.one, Global.fullScaleTransitionTime05).SetUpdate(true);
        }
    }
}
