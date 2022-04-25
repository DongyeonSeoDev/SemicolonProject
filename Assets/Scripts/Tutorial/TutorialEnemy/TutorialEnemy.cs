using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class TutorialEnemy : MonoBehaviour, ICanGetDamagableEnemy
    {
        public Image hpBarFillImage; // 적 HP 바 채워진것중 체력 확인용 ( 없으면 UI 만들어야 함 ( Assets > Prefabs > EnemyPrefabs > EnemyUI 참고 ) )
        public Image hpBarDamageFillImage; // 적 HP 바 채워진것중 데미지 확인용

        public int maxHP = 10;
        public int hp = 10;

        void Start()
        {
            hp = maxHP;
        }

        void Update()
        {

        }
        public void GetDamage(int damage, bool critical = false, bool isKnockBack = false, float knockBackPower = 20, float stunTime = 1, Vector2? direction = null)
        {
            hp -= damage;

            if (hp < 0)
            {
                hp = 0;
            }

            EffectManager.Instance.OnDamaged(damage, critical, true, transform.position);
        }
        //public void SetHP()
        //{

        //}
        public float EnemyHpPercent() => ((float)hp / maxHP) * 100f;
    }
}
