using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // 적 관리 클래스
    {
        public EnemyMoveSO enemyMoveSO;

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
    }
}