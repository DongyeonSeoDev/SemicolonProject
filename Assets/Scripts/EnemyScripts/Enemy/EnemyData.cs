using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public enum EnemyController
    { 
        AI,
        PLAYER
    }

    public enum EnemyType
    {
        Slime_01,
        Rat_02,
        Slime_03
    }

    public class EnemyData
    {
        public EnemyData(EnemyDataSO enemyDataSO)
        {
            enemyType = enemyDataSO.enemyType;
            normalColor = enemyDataSO.normalColor;
            damagedColor = enemyDataSO.damagedColor;
            enemyDeadEffectColor = enemyDataSO.enemyDeadEffectColor;
            playerNormalColor = enemyDataSO.playerNormalColor;
            playerDamagedColor = enemyDataSO.playerDamagedColor;
            playerDeadEffectColor = enemyDataSO.playerDeadEffectColor;
        }

        public EnemyController eEnemyController = EnemyController.AI;
        public EnemyType enemyType;

        public GameObject enemyObject;
        public EnemyLootListSO enemyLootList;
        public Animator enemyAnimator;
        public SpriteRenderer enemySpriteRenderer;
        public Rigidbody2D enemyRigidbody2D;
        public Image hpBarFillImage;
        public EnemyCommand enemyMoveCommand;

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

        public Vector2? knockBackDirection;

        public Color normalColor;
        public Color damagedColor;
        public Color enemyDeadEffectColor;

        public Color playerNormalColor;
        public Color playerDamagedColor;
        public Color playerDeadEffectColor;

        public float chaseSpeed = 5f;
        public float isSeePlayerDistance = 5f;
        public float isAttackPlayerDistance = 2f;
        public float isMinAttackPlayerDistance = 5f;
        public float isMaxAttackPlayerDistance = 8f;
        public float attackDelay = 1f;
        public float damageDelay = 0.1f;
        public float rushForce = 100f;
        public float knockBackPower;
        public float stunTime;

        public bool isDamaged = false;
        public bool isAttack = false;
        public bool isAnimation = true;
        public bool isHitAnimation = false;
        public bool isEndAttackAnimation = false;
        public bool isAttackCommand = false;
        public bool isLongDistanceAttack = false;
        public bool isEnemyMove = false;
        public bool isRotate = false;
        public bool isKnockBack = false;
        public bool isCurrentAttackTime = false;
        public bool isUseDelay = false;

        public int attackDamage = 10;
        public int damagedValue;
        public int maxHP = 30;
        public int hp = 30;

        public readonly int hashIsDie = Animator.StringToHash("isDie");
        public readonly int hashIsDead = Animator.StringToHash("isDead");
        public readonly int hashMove = Animator.StringToHash("Move");
        public readonly int hashAttack = Animator.StringToHash("Attack");
        public readonly int hashEndAttack = Animator.StringToHash("EndAttack");
        public readonly int hashHit = Animator.StringToHash("Hit");
        public readonly int hashReset = Animator.StringToHash("Reset");

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

        public bool IsRunAway()
        {
            if (isLongDistanceAttack)
            {
                float distance = Vector3.Distance(enemyObject.transform.position, PlayerObject.transform.position);

                return isMinAttackPlayerDistance > distance;
            }

            return false;
        }

        public bool IsAttackDelay(float currentAttackDelay = 0)
        {
            if (currentAttackDelay <= 0 || isCurrentAttackTime)
            {
                return isCurrentAttackTime;
            }

            isCurrentAttackTime = true;
            Util.DelayFunc(() => isCurrentAttackTime = false, currentAttackDelay);

            return isCurrentAttackTime;
        }
    }
}