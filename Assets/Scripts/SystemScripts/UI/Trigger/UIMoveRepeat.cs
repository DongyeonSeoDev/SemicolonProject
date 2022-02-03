using DG.Tweening;
using UnityEngine;

public class UIMoveRepeat : UITransition
{
    private Tween tween;

    public Vector3 targetAnchoredPos;
    public Ease _ease = Ease.Linear;
    public float time = 0.8f;

    protected override void Awake()
    {
        tween = GetComponent<RectTransform>().DOAnchorPos(targetAnchoredPos, time).SetEase(_ease);
        tween.SetUpdate(true);
        tween.SetLoops(-1, LoopType.Yoyo);
    }

    private void OnEnable()
    {
        tween.Play();
    }

    private void OnDisable()
    {
        tween.Pause();
    }

    public override void Transition(bool on)
    {
    }
}
