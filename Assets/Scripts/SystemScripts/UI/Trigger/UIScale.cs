using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

//[AddComponentMenu("Custom/UIPointerScale")]
[RequireComponent(typeof(EventTrigger))]
public class UIScale : UITransition
{
    public float transitionTime = 0.3f;
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);

    public bool useSound = true;
    public string soundId = "UIMouseEnterSFX4";

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Transition(bool on)
    {
        transform.DOScale(on && transitionEnable ? targetScale : Vector3.one, transitionTime).SetUpdate(true);
        if(on&& transitionEnable && useSound)
        {
            SoundManager.Instance.PlaySoundBox(soundId);
        }
    }
}
