using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Enemy
{
    public class BossCanvas : EnemyCanvas
    {
        public RectTransform hpBarRectTransform = null;
        public Image fillHpBar = null;
        public Image damageHpBar = null;

        public Vector2 setActiveMoveValue = new Vector2(0f, 50f);
        public float setActiveMoveTimeValue = 0.5f;
        public float fillAmountDelayTime = 0.5f;
        public float fillAmountTime = 0.5f;
        public float fillTweenTime = 0.1f;
        public float damageFillDelayTime = 0.3f;
        public float damageFillTime = 0.2f;

        private EnemyData enemyData = null;

        private Sequence activeTrueSequence = null;
        private Sequence activeFalseSequence = null;

        private Tween hpBarTween = null;
        private Tween damageBarTween = null;

        private CanvasGroup hpBarCanvasGroup = null;
        public CanvasGroup bossNameText = null;

        private bool isSequencePlay = false;

        protected override void Start()
        {
            base.Start();

            hpBarCanvasGroup = hpBarRectTransform.GetComponent<CanvasGroup>();

            activeTrueSequence = SetActiveSequence(true);
            activeFalseSequence = SetActiveSequence(false);
        }

        public void Init(EnemyData data)
        {
            enemyData = data;
        }

        public void SetActiveHPBar(bool value)
        {
            if (isSequencePlay)
            {
                Debug.LogError("Sequence가 이미 실행중입니다.");
                return;
            }

            if (value)
            {
                activeTrueSequence.Restart();
            }
            else
            {
                activeFalseSequence.Restart();
            }
        }

        public void SetActiveBossName(bool value)
        {
            if (value)
            {
                bossNameText.DOFade(1, 0.5f);
            }
            else
            {
                bossNameText.DOFade(0, 0.5f);
            }
        }

        private Sequence SetActiveSequence(bool isActive)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.SetAutoKill(false);
            sequence.Pause();

            if (isActive)
            {
                sequence.OnStart(() =>
                {
                    hpBarRectTransform.anchoredPosition -= setActiveMoveValue;
                    fillHpBar.fillAmount = 0;
                    isSequencePlay = true;
                });

                sequence.Append(hpBarRectTransform.DOAnchorPos(setActiveMoveValue, setActiveMoveTimeValue).SetRelative());
                sequence.Join(hpBarCanvasGroup.DOFade(1, setActiveMoveTimeValue));
                sequence.AppendInterval(fillAmountDelayTime);
                sequence.Append(fillHpBar.DOFillAmount(1, fillAmountTime));

                sequence.OnComplete(() =>
                {
                    isSequencePlay = false;

                    SetFill();
                });
            }
            else
            {
                sequence.OnStart(() =>
                {
                    isSequencePlay = true;
                });

                sequence.Append(hpBarRectTransform.DOAnchorPos(-setActiveMoveValue, setActiveMoveTimeValue).SetRelative());
                sequence.Join(hpBarCanvasGroup.DOFade(0, setActiveMoveTimeValue));

                sequence.OnComplete(() =>
                {
                    hpBarRectTransform.anchoredPosition += setActiveMoveValue;
                    isSequencePlay = false;
                });
            }

            return sequence;
        }

        public void SetFill()
        {
            if (isSequencePlay)
            {
                return;
            }

            if (hpBarTween.IsActive())
            {
                hpBarTween.Kill();
            }

            float fillValue = (float)enemyData.hp / enemyData.maxHP;

            hpBarTween = fillHpBar.DOFillAmount(fillValue, fillTweenTime);

            Util.DelayFunc(() =>
            {
                if (damageBarTween.IsActive())
                {
                    damageBarTween.Kill();
                }

                damageBarTween = damageHpBar.DOFillAmount(fillValue, damageFillTime);
            }, damageFillDelayTime);
        }
    }
}