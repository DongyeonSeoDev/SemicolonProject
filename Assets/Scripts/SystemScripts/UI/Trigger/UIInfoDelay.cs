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
            UIManager.Instance.SetUIDelayImg(needWaitTime- (Mathf.Clamp(needWaitTime * 0.1f, 0.1f, 0.2f)) ); //저 값을 빼는 것은 UI Fill이 다 찬 것을 보여주고 UI 띄우기 위함. 안하면 약간 Fill 남은 상태에서 UI 띄워진 것처럼 보임
        }
        else
        {
            isMouseOver = false;
            UIManager.Instance.OffUIDelayImg();
        }
    }
}
