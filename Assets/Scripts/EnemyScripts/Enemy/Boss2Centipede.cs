using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Boss2Centipede : Enemy
    {
        public Transform movePivot;

        private Vector2 dashTargetPosition = Vector2.zero;

        private float originMinAttackPower = 0f;
        private float originMaxAttackPower = 0f;

        private float bulletMinAttackPower = 0f;
        private float bulletMaxAttackPower = 0f;

        private float originCurrentSpeed = 0f;

        private float stopAnimTimer = 0f;
        private float moveTimer = 0f;
        private float dashTimer = 0f;
        private float meleeAttackDis = 12f;

        #region meleeAttack1(지면파괴)관련 변수
        [Header("meleeAttack1(지면파괴)의 원통형 공격 범위")]
        [SerializeField]
        private float meleeAttack1CircleRadius = 2;
        [Header("meleeAttack1(지면파괴)의 원통형 데미지 변동값")]
        [SerializeField]
        private float meleeAttack1CircleDamageValue = 3;
        [Header("meleeAttack1(지면파괴)의 구체 데미지 변동 값")]
        [SerializeField]
        private float meleeAttack1BulletDamageValue = -10;
        #endregion

        #region meleeAttack2(물어뜯기)관련 변수
        [Header("meleeAttack2(물어뜯기)의 돌진 거리")]
        [SerializeField]
        private float dashAttackDis = 5f;
        [Header("meleeAttack2(물어뜯기)의 돌진 데미지 변동 값")]
        [SerializeField]
        private float meleeAttack2DamageValue = -5f;
        #endregion

        #region sneer(침뱉기)관련 변수
        [Header("Sneer(침뱉기)의 구체 데미지 변동 값")]
        [SerializeField]
        private float sneerDamageValue = -10f;
        #endregion

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
        private EnemyCommand dashCommand;
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

            enemyMeleeAttack1Collider.GetComponent<CircleCollider2D>().radius = meleeAttack1CircleRadius;
            currentSpeed = enemyDataSO.speed;
            
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
            originMinAttackPower = enemyData.minAttackPower;
            originMaxAttackPower = enemyData.maxAttackPower;

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

            StopAnimTimerCheck();
            DashCheck();
            MoveCheck();
        }

        private void MoveCheck()
        {
            Vector3 playerPosition = EnemyManager.Player.transform.position;
            float dis = Vector3.Distance(playerPosition, transform.position);

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

        private void DashCheck()
        {
            if (isDashToPlayer)
            {
                dashTimer -= Time.deltaTime;

                if (dashCommand == null)
                {
                    DashToPlayerEnd();
                }

                dashCommand.Execute();

                if (dashTimer <= 0)
                {
                    DashToPlayerEnd();
                }
            }
        }
        public void DashToPlayer(float speed) // 대쉬 시작
        {
            Vector3 playerPosition = EnemyManager.Player.transform.position;

            dashTargetPosition = playerPosition;

            isDashToPlayer = true;
            dashTimer = dashAttackDis / speed;
            currentSpeed = speed;

            dashCommand = new CentipedeBossDashCommand(enemyData, playerPosition, rb, this);

            isMove = false;
            moveTimer = 0f;
        }
        private void DashToPlayerEnd()// 대쉬 끝났을 때
        {
            currentSpeed = originCurrentSpeed;
            dashCommand = null;
            isDashToPlayer = false;
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
                    if (value == 0)
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
        public void StopAnim(float stopTime)
        {
            if(stopTime <= 0)
            {
                return;
            }

            stopAnimTimer = stopTime;

            SetAnimSpeed(0f);
        }
        private void StopAnimTimerCheck()
        {
            if(stopAnimTimer > 0f)
            {
                stopAnimTimer -= Time.deltaTime;

                if(stopAnimTimer <= 0f)
                {
                    SetAnimSpeed(1f);
                }
            }
        }
        public void SetAnimSpeed(float speed)
        {
            anim.speed = speed;
        }
        // sneer(침뱉기)에 쓰이는 총알 발사 함수
        public void ShootBulletToPlayer()
        {
            enemySneerCommand = new CentipedeLongRangeAttackCommand(this, shootTrm, EnemyManager.Player.transform.position, Type.CentipedeBullet, shootTrm, bulletMinAttackPower, bulletMaxAttackPower, enemyData.randomCritical, enemyData.randomCritical);
            enemySneerCommand.Execute();
        }
        // meleeAttack1(지면 파괴)에 쓰이는 총알 발사 함수
        public void ShootBulletAround(int num)
        {
            Vector3 rotation = Vector3.zero;
            
            float up = 360f / num;

            for (int i = 0; i < num; i++)
            {
                rotation = Quaternion.Euler(1f, 1f, up * i) * Vector2.one;
                rotation = rotation.normalized;

                enemySneerCommand = new CentipedeLongRangeAttackCommand(this, shootTrm, EnemyManager.Player.transform.position, rotation, Type.CentipedeBullet, shootTrm, bulletMinAttackPower, bulletMaxAttackPower, enemyData.randomCritical, enemyData.randomCritical);
                enemySneerCommand.Execute();
            }
        }
        public EnemyState AttackStateChangeCondition() // 이벤트 구독에 사용됨 - 공격2 사용 가능 확인
        {
            int value = Random.Range(0, 2);
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

                        if (value == 0)
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
        public void CentipedeMeleeAttack1Start()
        {
            CentipedeAttackStart();

            enemyData.minAttackPower += meleeAttack1CircleDamageValue;
            enemyData.maxAttackPower += meleeAttack1CircleDamageValue;

            bulletMinAttackPower = originMinAttackPower + meleeAttack1BulletDamageValue;
            bulletMaxAttackPower = originMaxAttackPower + meleeAttack1BulletDamageValue;
        }
        public void CentipedeMeleeAttack2Start()
        {
            CentipedeAttackStart();

            enemyData.minAttackPower += meleeAttack2DamageValue;
            enemyData.maxAttackPower += meleeAttack2DamageValue;
        }
        public void CentipedeSneerStart()
        {
            CentipedeAttackStart();

            bulletMinAttackPower = originMinAttackPower + sneerDamageValue;
            bulletMaxAttackPower = originMaxAttackPower + sneerDamageValue;
        }
        private void CentipedeAttackStart()
        {
            isAttack = true;
        }
        public void CentipedeAttackEnd()
        {
            isAttack = false;
            anim.speed = 1f;

            enemyData.minAttackPower = originMinAttackPower;
            enemyData.maxAttackPower = originMaxAttackPower;

            bulletMinAttackPower = originMinAttackPower;
            bulletMaxAttackPower= originMaxAttackPower;

            enemyMeleeAttack1Collider.GetComponent<EnemyAttackCheck>().AttackObjectReset();
            enemyMeleeAttack2Collider.GetComponent<EnemyAttackCheck>().AttackObjectReset();

            enemyData.enemyAnimator.SetTrigger(hashAttackEnd);
        }
    }
}
