using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public abstract class TutorialEnemy : MonoBehaviour, ICanGetDamagableEnemy
    {
        public Image hpBarFillImage; // �� HP �� ä�������� ü�� Ȯ�ο� ( ������ UI ������ �� ( Assets > Prefabs > EnemyPrefabs > EnemyUI ���� ) )
        public Image hpBarDamageFillImage; // �� HP �� ä�������� ������ Ȯ�ο�

        public int maxHP = 10;
        public int hp = 10;

        public virtual void Start()
        {
            hp = maxHP;
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
        public Transform GetTransform() => transform;
        public GameObject GetGameObject() => gameObject;
        public float EnemyHpPercent() => ((float)hp / maxHP) * 100f;
    }
}
