using System.Collections.Generic;
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

        public List<EnemyLootData> enemyLootList;

        public GameObject enemyObject;
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
        public float isAttackPlayerDistance = 2f;
        public float isMinAttackPlayerDistance = 5f;
        public float isRunAwayDistance = 9f;
        public float isMaxAttackPlayerDistance = 10f;
        public float attackDelay = 1f;
        public float damageDelay = 0.1f;
        public float rushForce = 50f;
        public float knockBackPower;
        public float stunTime;
        public float minRunAwayTime;
        public float maxRunAwayTime;
        public float currentRunAwayTime = 1f;
        public float playerAnimationTime;
        public float playerAnimationDelay = 1f;

        public bool isDamaged = false;
        public bool isAttack = false;
        public bool isAnimation = true;
        public bool isHitAnimation = false;
        public bool isEndAttackAnimation = false;
        public bool isAttackCommand = false;
        public bool isLongDistanceAttack = false;
        public bool isRunAway = false;
        public bool isEnemyMove = false;
        public bool isRotate = false;
        public bool isKnockBack = false;
        public bool isCurrentAttackTime = false;
        public bool isUseDelay = false;
        public bool isAttacking = false;
        public bool isUseAttacking = false;
        public bool isPlayerAttacking = false;

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

        public bool IsAttackPlayer()
        {
            if (currentRunAwayTime <= 0)
            {
                isRunAway = false;
                currentRunAwayTime = 1f;

                return true;
            }

            if (isLongDistanceAttack)
            {
                float distance = Vector3.Distance(enemyObject.transform.position, PlayerObject.transform.position);

                if (isRunAway)
                {
                    return isRunAwayDistance <= distance && distance <= isMaxAttackPlayerDistance;
                }

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

                if (isRunAway)
                {
                    if (isRunAwayDistance <= distance)
                    {
                        isRunAway = false;
                    }
                    else
                    {
                        currentRunAwayTime -= Time.deltaTime;
                    }

                    return isRunAway;
                }
                else if (isMinAttackPlayerDistance > distance)
                {
                    isRunAway = true;
                    currentRunAwayTime = Random.Range(minRunAwayTime, maxRunAwayTime);

                    return true;
                }
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