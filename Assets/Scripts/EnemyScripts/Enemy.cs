using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // 적 관리 클래스
    {
        public EnemyMoveSO enemyMoveSO;

        private EnemyState currentState;
        private EnemyData enemyData;

        private SpriteRenderer sr;

        private float lastPositionX;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            enemyData = new EnemyData()
            {
                eEnemyController = EnemyController.AI,
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyAnimator = GetComponent<Animator>(),
                enemySpriteRenderer = sr
            };

            currentState = new EnemyMoveState(enemyData);

            lastPositionX = transform.position.x;
        }

        private void Start()
        {
            EnemyAttackCheck enemyAttackCheck = transform.GetComponentInChildren<EnemyAttackCheck>();

            if (enemyAttackCheck != null)
            {
                enemyAttackCheck.SetAttackDamage(enemyData.attackDamage);
                enemyAttackCheck.gameObject.SetActive(false);
            }
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