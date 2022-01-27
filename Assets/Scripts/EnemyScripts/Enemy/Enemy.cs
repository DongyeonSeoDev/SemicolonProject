using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Enemy : PoolManager // 적 관리 클래스
    {
        public EnemyMoveSO enemyMoveSO;
        public EnemyLootListSO enemyLootListSO;
        public Image hpBarFillImage;

        protected EnemyData enemyData;
        protected SpriteRenderer sr;

        private EnemyState currentState;

        private float lastPositionX;

        protected virtual void OnEnable()
        {
            sr = enemyData.enemySpriteRenderer;
            currentState = new EnemyIdleState(enemyData);
            lastPositionX = transform.position.x + Mathf.Infinity;

            enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;
            enemyData.enemyAnimator.SetTrigger(enemyData.hashReset);
            enemyData.enemyAnimator.SetBool(enemyData.hashIsDead, false);
            enemyData.enemySpriteRenderer.enabled = true;
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState = currentState.Process();
            }

            if (enemyData.isRotate)
            {
                if (lastPositionX > transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (lastPositionX < transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else
            {
                if (lastPositionX > transform.position.x)
                {
                    sr.flipX = true;
                }
                else if (lastPositionX < transform.position.x)
                {
                    sr.flipX = false;
                }
            }

            lastPositionX = transform.position.x;
        }

        public void GetDamage(int damage)
        {
            if (!enemyData.isDamaged)
            {
                enemyData.isDamaged = true;
                enemyData.damagedValue = damage;
            }
        }

        public void EnemyDestroy()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();
        }

        public string GetEnemyId()
        {
            return enemyData.enemyId;
        }

        public void MoveEnemy()
        {
            enemyData.isEnemyMove = true;
        }
    }
}