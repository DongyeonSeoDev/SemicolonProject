using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // �� ���� Ŭ����
    {
        public EnemyMoveSO enemyMoveSO;

        private State currentState;
        private EnemyData enemyData;

        private void Awake()
        {
            enemyData = new EnemyData()
            {
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyAnimator = GetComponent<Animator>()
            };

            currentState = new Move(enemyData);
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
            currentState = currentState.Process();
        }

        public void GetDamage(int damage)
        {
            enemyData.isDamaged = true;
            enemyData.damagedValue = damage;
        }
    }
}