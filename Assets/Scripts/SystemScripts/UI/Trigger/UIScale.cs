using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Water;

[AddComponentMenu("Custom/UIPointerScale")]
[RequireComponent(typeof(EventTrigger))]
public class UIScale : MonoBehaviour
{
    private EventTrigger eventTrigger;

    public bool transitionEnable = true;

    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener(eventData => OnPointer(true));

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener(eventData => OnPointer(false));

        eventTrigger.triggers.Add(entry1);
        eventTrigger.triggers.Add(entry2);
    }

    public void OnPointer(bool on)
    {
        transform.DOScale(on && transitionEnable ? Global.onePointThree : Vector3.one, Global.fullScaleTransitionTime05).SetUpdate(true);
    }
}
