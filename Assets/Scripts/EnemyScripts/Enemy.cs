using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // 적 관리 클래스
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

        private void Update()
        {
            currentState = currentState.Process();
        }
    }
}