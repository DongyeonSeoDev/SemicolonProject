using UnityEngine;

namespace Enemy
{
    public class EnemyData
    {
        public GameObject enemyObject;
        public EnemyMoveSO enemyMoveSO;
        public Animator enemyAnimator;

        private GameObject playerObject;
        public GameObject PlayerObject
        {
            get
            {
                if (playerObject == null)
                {
                    playerObject = GameObject.FindGameObjectWithTag("Player");

                    if (playerObject == null)
                    {
                        Debug.LogError("Player에 Player 테그를 붙이지 않았습니다.");
                    }
                }

                return playerObject;
            }
        }

        public float chaseSpeed = 5f;
        public float isSeePlayerDistance = 5f;
        public float isAttackPlayerDistance = 2f;

        public bool isDamaged = false;
        public int attackDamage = 10;
        public int damagedValue;

        public readonly int hashIsDie = Animator.StringToHash("isDie");
        public readonly int hashIsDead = Animator.StringToHash("isDead");
        public readonly int hashMove = Animator.StringToHash("Move");
        public readonly int hashAttack = Animator.StringToHash("Attack");

        public bool IsSeePlayer() => Vector3.Distance(enemyObject.transform.position, PlayerObject.transform.position) <= isSeePlayerDistance;
        public bool IsAttackPlayer() => Vector3.Distance(enemyObject.transform.position, PlayerObject.transform.position) <= isAttackPlayerDistance;
    }
}