using UnityEngine;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class UIInfoDelay : UITransition
{
    public float needWaitTime = 1.5f;

    private float checkTime;
    private bool isMouseOver = false;

    public event System.Action mouseOverEvent = null;

    private void Update()
    {
        if (isMouseOver)
        {
            if(checkTime < Time.unscaledTime)
            {
                isMouseOver = false;
                UIManager.Instance.OffUIDelayImg();
                mouseOverEvent?.Invoke();
            }
        }
    }

    public override void Transition(bool on)
    {
        if(on && transitionEnable)
        {
            isMouseOver = true;
            checkTime = Time.unscaledTime + needWaitTime;
            UIManager.Instance.SetUIDelayImg(needWaitTime);
        }
        else
        {
            isMouseOver = false;
            UIManager.Instance.OffUIDelayImg();
        }
    }
}
