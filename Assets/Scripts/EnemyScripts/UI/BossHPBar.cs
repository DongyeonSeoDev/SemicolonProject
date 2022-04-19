using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHPBar : EnemyHPBar
{
    public RectTransform hpBarRectTransform = null;
    public Image fillHpBar = null;

    public Vector2 setActiveMoveValue = new Vector2(0f, 50f);
    public float setActiveMoveTimeValue = 0.5f;

    private Sequence activeTrueSequence = null;
    private Sequence activeFalseSequence = null;

    private CanvasGroup hpBarCanvasGroup = null;

    private void Start()
    {
        hpBarCanvasGroup = hpBarRectTransform.GetComponent<CanvasGroup>();

        activeTrueSequence = SetActiveSequence(true);
        activeFalseSequence = SetActiveSequence(false);
    }

    public void SetActiveHPBar(bool value)
    {
        ResetSetActiveTween();

        if (value)
        {
            activeTrueSequence.Restart();
        }
        else
        {
            activeFalseSequence.Restart();
        }
    }

    private void ResetSetActiveTween()
    {
        if (activeTrueSequence.IsActive())
        {
            activeTrueSequence.Complete();
        }
        else if (activeFalseSequence.IsActive())
        {
            activeFalseSequence.Complete();
        }
    }

    private Sequence SetActiveSequence(bool isActive)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.SetAutoKill(false);
        sequence.Pause();

        if (isActive)
        {
            activeTrueSequence.OnStart(() =>
            {
                hpBarRectTransform.anchoredPosition -= setActiveMoveValue;
            });

            sequence.Append(hpBarRectTransform.DOAnchorPos(setActiveMoveValue, setActiveMoveTimeValue).SetRelative());
            sequence.Join(hpBarCanvasGroup.DOFade(1, setActiveMoveTimeValue));
        }
        else
        {
            sequence.Append(hpBarRectTransform.DOAnchorPos(-setActiveMoveValue, setActiveMoveTimeValue).SetRelative());
            sequence.Join(hpBarCanvasGroup.DOFade(0, setActiveMoveTimeValue));

            activeFalseSequence.OnComplete(() =>
            {
                hpBarRectTransform.anchoredPosition += setActiveMoveValue;
            });
        }

        return sequence;
    }

    public void SetFill(float value)
    { 
        fillHpBar.fillAmount = value;
    }
}
