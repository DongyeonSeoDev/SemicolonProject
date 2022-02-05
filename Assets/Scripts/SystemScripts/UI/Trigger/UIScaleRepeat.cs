using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

//[AddComponentMenu("Custom/UIPointerScaleRepeat")]
[RequireComponent(typeof(EventTrigger))]
public class UIScaleRepeat : UITransition
{
    public float transitionTime = 0.2f;
    public Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f);

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Transition(bool on)
    {
        if (on && transitionEnable)
        {
            transform.DOScale(targetScale, transitionTime).SetLoops(-1,LoopType.Yoyo).SetUpdate(true);
        }
        else
        {
            transform.DOKill();
            transform.DOScale(Vector3.one, transitionTime).SetUpdate(true);
        }
    }
}
