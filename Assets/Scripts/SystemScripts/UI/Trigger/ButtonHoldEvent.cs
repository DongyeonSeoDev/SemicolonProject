using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ButtonHoldEvent : MonoBehaviour
{   
    public event System.Action OnPressing = null;
    public event System.Func<bool> CanContinue = null;

    public bool isButtonOutDisable = true;  //버튼 꾹 누르다가 밖으로 마우스 나가면 기능 작동 안되게
    public bool transitionEnable = true;

    public Pair<float, float> delayTimePair;

    private Pair<float, float> checkTimePair = new Pair<float, float>(0f,0f);
    private bool isPress = false;

    //public EventTriggerType TriggerType;

    private void Awake()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();

        eventTrigger.AddTrigger(EventTriggerType.PointerDown, eventData => Transition(true));
        eventTrigger.AddTrigger(EventTriggerType.PointerUp, eventData => Transition(false));
        eventTrigger.AddTrigger(EventTriggerType.PointerExit, eventData =>
        {
            if (isPress && isButtonOutDisable)
            {
                isPress = false;
            }
        });
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
                        //홀드 클릭
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

            if (checkTimePair.first >= Time.unscaledTime)  //홀드가 아니라 그냥 클릭함
            {
                if (CanContinue == null || CanContinue())
                {
                    OnPressing?.Invoke();
                }
            }
        }
    }
}
