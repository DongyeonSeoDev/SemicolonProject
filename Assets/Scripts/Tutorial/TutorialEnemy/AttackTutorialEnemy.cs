using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Enemy
{
    public class AttackTutorialEnemy : TutorialEnemy
    {
        public Image hpBarFillImage; // 적 HP 바 채워진것중 체력 확인용
        public Image hpBarDamageFillImage; // 적 HP 바 채워진것중 데미지 확인용
        public GameObject hpBar; // 적 HP 바 오브젝트 ( hpBarFillImage의 부모 캔버스 오브젝트 )

        public float hpTweenTime = 0.1f;
        public float hpTweenDelayTime = 0.3f;
        public float damageHPTweenTime = 0.2f;

        private Tween hpTween = null;
        private Tween damageHPTween = null;

        public override void Start()
        {
            base.Start();

            SetHP(false);
        }
        public override void Update()
        {
            base.Update();
        }

        public override void GetDamage(int damage, bool critical = false, bool isKnockBack = false, float knockBackPower = 20, float stunTime = 1, Vector2? direction = null)
        {
            base.GetDamage(damage, critical, isKnockBack, knockBackPower, stunTime, direction);

            hp -= damage;

            if (hp < 0)
            {
                hp = 0;
            }

            EffectManager.Instance.OnDamaged(damage, critical, true, transform.position);

            SetHP(true);
        }

        private void SetHP(bool isUseTween)
        {
            float fillValue = (float)hp / maxHP;

            if (isUseTween)
            {
                if (hpBarFillImage != null)
                {
                    if (hpTween.IsActive())
                    {
                        hpTween.Kill();
                    }

                    hpTween = hpBarFillImage.DOFillAmount(fillValue, hpTweenTime);
                }

                if (hpBarDamageFillImage != null)
                {
                    Util.DelayFunc(() =>
                    {
                        if (damageHPTween.IsActive())
                        {
                            damageHPTween.Kill();
                        }

                        damageHPTween = hpBarDamageFillImage.DOFillAmount(fillValue, damageHPTweenTime);
                    }, hpTweenDelayTime);
                }
            }
            else
            {
                hpBarFillImage.fillAmount = fillValue;
                hpBarDamageFillImage.fillAmount = fillValue;
            }
        }
    }
}
