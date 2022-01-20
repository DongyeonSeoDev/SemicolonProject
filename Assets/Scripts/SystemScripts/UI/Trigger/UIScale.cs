using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Water;

[AddComponentMenu("Custom/UIPointerScale")]
[RequireComponent(typeof(EventTrigger))]
public class UIScale : UITransition
{
    public float transitionTime = 0.5f;
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Transition(bool on)
    {
        transform.DOScale(on && transitionEnable ? targetScale : Vector3.one, transitionTime).SetUpdate(true);
    }
}
