using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public enum EnemyController
    { 
        AI,
        PLAYER
    }

    public class EnemyData
    {
        public EnemyController eEnemyController;

        public GameObject enemyObject;
        public EnemyMoveSO enemyMoveSO;
        public EnemyLootListSO enemyLootList;
        public Animator enemyAnimator;
        public SpriteRenderer enemySpriteRenderer;
        public Image hpBarFillImage;

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

        public Color normalColor;
        public Color damagedColor;

        public string enemyId;

        public float chaseSpeed = 5f;
        public float isSeePlayerDistance = 5f;
        public float isAttackPlayerDistance = 2f;
        public float isMinAttackPlayerDistance = 5f;
        public float isMaxAttackPlayerDistance = 8f;
        public float damageDelay = 0.2f;

        public bool isDamaged = false;
        public bool isAttack = false;
        public bool isAnimation = true;
        public bool isHitAnimation = false;
        public bool isAttackCommand = false;
        public bool isLongDistanceAttack = false;

        public int attackDamage = 10;
        public int damagedValue;
        public int maxHP = 30;
        public int hp = 30;

        public readonly int hashIsDie = Animator.StringToHash("isDie");
        public readonly int hashIsDead = Animator.StringToHash("isDead");
        public readonly int hashMove = Animator.StringToHash("Move");
        public readonly int hashAttack = Animator.StringToHash("Attack");
        public readonly int hashHit = Animator.StringToHash("Hit");

        public bool IsSeePlayer() => Vector3.Distance(enemyObject.transform.position, PlayerObject.transform.position) <= isSeePlayerDistance;

        public bool IsAttackPlayer()
        {
            if (isLongDistanceAttack)
            {
                float distance = Vector3.Distance(enemyObject.transform.position, PlayerObject.transform.position);

                return isMinAttackPlayerDistance <= distance && distance <= isMaxAttackPlayerDistance;
            }
            else
            {
                return Vector3.Distance(enemyObject.transform.position, PlayerObject.transform.position) <= isAttackPlayerDistance;
            }
        }
    }
}