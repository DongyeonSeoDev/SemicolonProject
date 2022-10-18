using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ButtonHoldEvent : MonoBehaviour
{   
    public event System.Action OnPressing = null;
    public event System.Func<bool> CanContinue = null;

    public bool isButtonOutDisable = true;  //��ư �� �����ٰ� ������ ���콺 ������ ��� �۵� �ȵǰ�
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
                        //Ȧ�� Ŭ��
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

            if (checkTimePair.first >= Time.unscaledTime)  //Ȧ�尡 �ƴ϶� �׳� Ŭ����
            {
                if (CanContinue == null || CanContinue())
                {
                    OnPressing?.Invoke();
                }
            }
        }
    }
}
