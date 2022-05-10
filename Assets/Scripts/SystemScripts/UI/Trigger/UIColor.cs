using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class UIColor : UITransition
{
    public Color transitionColor;

    private Color originColor;
    private Image img;

    public string soundId;

    protected override void Awake()
    {
        base.Awake();
        img = GetComponent<Image>();
        originColor = img.color;
    }

    public override void Transition(bool on)
    {
        img.DOColor(transitionEnable && on ? transitionColor : originColor, 0.3f).SetUpdate(true);

        if (on && transitionEnable && !string.IsNullOrEmpty(soundId))
        {
            SoundManager.Instance.PlaySoundBox(soundId);
        }
    }
}
