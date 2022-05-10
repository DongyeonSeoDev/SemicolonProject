using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ButtonHoldEvent : MonoBehaviour
{   
    public event System.Action OnPressing = null;
    public event System.Func<bool> CanContinue = null;

    public bool transitionEnable = true;

    public Pair<float, float> delayTimePair;

    private Pair<float, float> checkTimePair = new Pair<float, float>(0f,0f);
    private bool isPress = false;

    //public EventTriggerType TriggerType;

    private void Awake()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerDown;
        entry1.callback.AddListener(eventData => Transition(true));

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener(eventData => Transition(false));

        eventTrigger.triggers.Add(entry1);
        eventTrigger.triggers.Add(entry2);
    }

    private void Update()
    {
        if(isPress)
        {
            if(checkTimePair.first < Time.unscaledTime)
            {
                if(checkTimePair.second < Time.unscaledTime)
                {
                    if(CanContinue !=null && !CanContinue())
                    {
                        isPress = false;
                    }
                    else
                    {
                        OnPressing?.Invoke();
                        checkTimePair.second = Time.unscaledTime + delayTimePair.second;
                    }
                }
            }
        }
    }

    public void Transition(bool on)
    {
        if (on && transitionEnable)
        {
            isPress = true;
            checkTimePair.first = Time.unscaledTime + delayTimePair.first;
        }
        else
        {
            isPress = false;

            if (checkTimePair.first >= Time.unscaledTime)
            {
                if (CanContinue == null || CanContinue())
                {
                    OnPressing?.Invoke();
                }
            }
        }
    }
}
