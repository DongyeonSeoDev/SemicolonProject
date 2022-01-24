using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Enemy : PoolManager // �� ���� Ŭ����
    {
        public EnemyMoveSO enemyMoveSO;
        public EnemyLootListSO enemyLootListSO;
        public Image hpBarFillImage;

        protected EnemyData enemyData;
        protected SpriteRenderer sr;

        private EnemyState currentState;

        private float lastPositionX;

        protected virtual void Awake()
        {
            sr = enemyData.enemySpriteRenderer;
            currentState = new EnemyMoveState(enemyData);
            lastPositionX = transform.position.x;
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState = currentState.Process();
            }

            if (lastPositionX > transform.position.x)
            {
                sr.flipX = true;
            }
            else if (lastPositionX < transform.position.x)
            {
                sr.flipX = false;
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
        }

        public string GetEnemyId()
        {
            return enemyData.enemyId;
        }
    }
}