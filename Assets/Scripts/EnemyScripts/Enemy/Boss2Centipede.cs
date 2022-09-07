using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Boss2Centipede : Enemy
    {
        public Transform movePivot;

        private float originCurrentSpeed = 0f;

        private float moveTimer = 0f;
        private float dashTimer = 0f;
        private float meleeAttackDis = 12f;

        private bool isMove = false;
        private bool isDashToPlayer = false;
        private bool isAttack = false;

        public readonly int hashMeleeAttack1 = Animator.StringToHash("meleeAttack1");
        public readonly int hashMeleeAttack2 = Animator.StringToHash("meleeAttack2");
        public readonly int hashSneer = Animator.StringToHash("sneer");

        public readonly int hashAttackEnd = Animator.StringToHash("attackEnd");
        private readonly int hashMove = Animator.StringToHash("move");

        private int sneerCount = 0;

        private EnemyCommand enemyMoveCommand;
        private EnemyCommand enemySneerCommand;

        [HideInInspector] public LayerMask whatIsWall;
        [HideInInspector] public float currentSpeed = 0f;

        [SerializeField]
        private EnemyBullet sneerBullet = null;
        [SerializeField]
        private Transform shootTrm = null;

        [SerializeField]
        private Collider2D enemyMeleeAttack1Collider = null;
        [SerializeField]
        private Collider2D enemyMeleeAttack2Collider = null;

        private BossCanvas bossHPBar;

        protected override void Awake()
        {
            base.Awake();

            currentSpeed = 10f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetBossHPBar();

            enemyData.attackDelay = 2f;
            enemyData.noAttackTime = 5f;
            enemyData.isAttackPlayerDistance = 20f;
            enemyData.isNoKnockback = true;
            enemyData.isNoStun = true;

            originCurrentSpeed = currentSpeed;

            SetCommands();
            MoveEnemy();
        }

        protected override void Update()
        {
            base.Update();

            if (isStop || !enemyData.isEnemyMove)
            {
                return;
            }

            Vector3 playerPosition = EnemyManager.Player.transform.position;
            float dis = Vector3.Distance(playerPosition, transform.position);

            if(isDashToPlayer)
            {
                dashTimer -= Time.deltaTime;

                enemyMoveCommand.Execute();

                if (dashTimer <= 0)
                {
                    currentSpeed = originCurrentSpeed; 
                    dashTimer = 0f;
                    isDashToPlayer = false;
                }
            }

            if (isMove && !isDashToPlayer)
            {
                moveTimer -= Time.deltaTime;

                enemyMoveCommand.Execute();

                if (moveTimer <= 0 || (dis < meleeAttackDis && playerPosition.y >= transform.position.y) || isAttack)
                {
                    moveTimer = 0f;
                    isMove = false;
                    AttackCheck();
                }
            }
        }

        public override void MoveEnemy()
        {
            base.MoveEnemy();

            enemyData.animationDictionary[EnemyAnimationType.Move] = hashMove;
        }
        private void SetBossHPBar()
        {
            bossHPBar = GetComponentInChildren<BossCanvas>();
            bossHPBar.Init(enemyData);

            enemyData.enemyCanvas = bossHPBar.gameObject;

            if (bossHPBar != null)
            {
                bossHPBar.SetFill();
            }
        }
        private void SetCommands()
        {
            enemyMoveCommand = new CentipedeBossMoveCommand(enemyData, movePivot, rb, this);

            enemyData.enemyMoveCommand = enemyMoveCommand;
            
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.attackTypeCheckCondition = AttackCheck;
            enemyData.addAIAttackStateChangeCondition = AttackStateChangeCondition;
            enemyData.addChangeAttackCondition = ChangeAttackCondition;
        }
        protected override void SetHP(bool useTween)
        {
            if (bossHPBar != null)
            {
                bossHPBar.SetFill();
            }

            if (enemyData.hp <= 0)
            {
                EventManager.TriggerEvent("BossDead");
            }
        }
        public void DashToPlayer(float speed) // 대쉬 시작
        {
            Vector3 playerPosition = EnemyManager.Player.transform.position;
            float dis = Vector3.Distance(playerPosition, transform.position);

            isDashToPlayer = true;
            dashTimer = dis / speed;
            currentSpeed = speed;

            isMove = false;
            moveTimer = 0f;
        }
        private void DashToPlayerEnd()// 대쉬 끝났을 때
        {
            isDashToPlayer = false;
        }
        public void AttackCheck() // 이벤트 구독에 사용됨 - 특수공격 사용 확인
        {
            int value = Random.Range(0, 2);
            Vector3 playerPosition = EnemyManager.Player.transform.position;
            float dis = Vector3.Distance(playerPosition, transform.position);

            if(isMove)
            {
                return;
            }

            if (dis < meleeAttackDis)
            {
                // MeleeAttack
                 enemyData.attackDelay = 1f;

                if (playerPosition.y >= transform.position.y)
                {
                    if (Mathf.Abs(EnemyManager.Player.transform.position.y - transform.position.y) > 1f)
                    {
                        enemyData.animationDictionary[EnemyAnimationType.Attack] = hashMeleeAttack1;
                    }
                    else
                    {
                        enemyData.animationDictionary[EnemyAnimationType.Attack] = hashMeleeAttack2;
                    }

                    return;
                }
            }

            if (value == 0)
            {
                // SneerAttack

                enemyData.attackDelay = 2f;

                sneerCount = 3;
            }
            else
            {
                isMove = true;
                moveTimer = 3f;
            }
           
        }
        public void SetAnimSpeed(float speed)
        {
            anim.speed = speed;
        }
        public void ShootBulletToPlayer()
        {
            enemySneerCommand = new CentipedeLongRangeAttackCommand(this, shootTrm, EnemyManager.Player.transform.position, Type.CentipedeBullet, shootTrm, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.randomCritical);
            enemySneerCommand.Execute();
        }
        public void ShootBulletAround(int num)
        {
            Vector3 rotation = Vector3.zero;
            
            float up = 360f / num;

            for (int i = 0; i < num; i++)
            {
                rotation = Quaternion.Euler(1f, 1f, up * i) * Vector2.one;
                rotation = rotation.normalized;

                enemySneerCommand = new CentipedeLongRangeAttackCommand(this, shootTrm, EnemyManager.Player.transform.position, rotation, Type.CentipedeBullet, shootTrm, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.randomCritical);
                enemySneerCommand.Execute();
            }
        }
        public EnemyState AttackStateChangeCondition() // 이벤트 구독에 사용됨 - 공격2 사용 가능 확인
        {
            Vector3 playerPosition = EnemyManager.Player.transform.position;
            float dis = Vector3.Distance(playerPosition, transform.position);

            if (!isMove && sneerCount > 0)
            {
                if (playerPosition.y >= transform.position.y)
                {
                    if (dis < meleeAttackDis)
                    {
                        enemyData.attackDelay = 1f;
                        sneerCount = 0;

                        if (Mathf.Abs(EnemyManager.Player.transform.position.y - transform.position.y) > 1f)
                        {
                            enemyData.animationDictionary[EnemyAnimationType.Attack] = hashMeleeAttack1;
                        }
                        else
                        {
                            enemyData.animationDictionary[EnemyAnimationType.Attack] = hashMeleeAttack2;
                        }

                        return new EnemyChaseState(enemyData);
                    }
                }

                sneerCount--;

                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashSneer;

                return new EnemyAIAttackState(enemyData);
            }

            return new EnemyChaseState(enemyData);
        }
        public EnemyState ChangeAttackCondition() // 이벤트 구독에 사용됨 - 공격을 해야하는지 확인
        {
            if (isMove)
            {
                return new EnemyChaseState(enemyData);
            }
            
            return null;
        }
        public void CentipedeAttackStart()
        {
            isAttack = true;
        }
        public void CentipedeAttackEnd()
        {
            isAttack = false;
            anim.speed = 1f;

            enemyMeleeAttack1Collider.GetComponent<EnemyAttackCheck>().AttackObjectReset();
            enemyMeleeAttack2Collider.GetComponent<EnemyAttackCheck>().AttackObjectReset();

            enemyData.enemyAnimator.SetTrigger(hashAttackEnd);
        }
    }
}
