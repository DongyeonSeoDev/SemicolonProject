using UnityEngine;

namespace Enemy
{
    public partial class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyData enemyData) : base(eState.IDLE, enemyData) { }
    }

    public partial class EnemyMoveState : EnemyState // 움직임 상태
    {
        private EnemyCommand enemyMoveCommand;

        public EnemyMoveState(EnemyData enemyData) : base(eState.MOVE, enemyData)
        {
            enemyMoveCommand = new EnemyMovePlayerControllerCommand(enemyData.enemyRigidbody2D);
        }

        protected override void Start()
        {
            if (enemyData.isAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashMove);
            }

            base.Start();
        }

        protected override void Update()
        {
            enemyMoveCommand.Execute();

            base.Update();
        }

        protected override void End()
        {
            if (enemyData.isAnimation)
            {
                enemyData.enemyAnimator.ResetTrigger(enemyData.hashMove);
            }
        }
    }

    public partial class EnemyChaseState : EnemyState // 추격 상태
    {
        public EnemyChaseState(EnemyData enemyData) : base(eState.CHASE, enemyData)
        {

        }

        protected override void Start()
        {
            if (enemyData.isAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashMove);
            }

            base.Start();
        }

        protected override void Update()
        {
            enemyData.enemyMoveCommand.Execute();

            base.Update();
        }

        protected override void End()
        {
            if (enemyData.isAnimation)
            {
                enemyData.enemyAnimator.ResetTrigger(enemyData.hashMove);
            }
        }
    }

    public partial class EnemyAttackState : EnemyState // 공격 상태
    {
        private float currentTime;
        private bool isDelay = false;
        private bool isNoAttack = false;

        public EnemyAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            if (enemyData.isEndAttackAnimation)
            {
                enemyData.enemyAnimator.ResetTrigger(enemyData.hashEndAttack);
            }

            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyData.isAttacking = true;

                if (enemyData.isUseDelay)
                {
                    isDelay = enemyData.IsAttackDelay();
                }

                if (!isDelay)
                {
                    enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);
                }

                currentTime = 0f;
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                if (SlimeGameManager.Instance.CurrentSkillDelayTimer[0] <= 0)
                {
                    SlimeGameManager.Instance.SetSkillDelay(0, enemyData.playerAnimationDelay);
                    SlimeGameManager.Instance.CurrentSkillDelayTimer[0] = SlimeGameManager.Instance.SkillDelays[0] + enemyData.playerAnimationTime;

                    enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);
                    enemyData.enemyAnimator.speed = 1.2f;
                }
                else
                {
                    isNoAttack = true;
                }
            }

            SpriteFlipCheck();
            base.Start();
        }

        protected override void Update()
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                if (!isDelay)
                {
                    currentTime += Time.deltaTime;
                }
                else
                {
                    isDelay = enemyData.IsAttackDelay();

                    if (!isDelay)
                    {
                        enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);
                        SpriteFlipCheck();
                    }
                }

                if (currentTime >= enemyData.attackDelay)
                {
                    currentTime = 0f;

                    SpriteFlipCheck();

                    base.Update();
                }
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                if (isNoAttack)
                {
                    base.Update();
                }

                if (SlimeGameManager.Instance.CurrentSkillDelayTimer[0] <= enemyData.playerAnimationDelay)
                {
                    enemyData.enemyAnimator.speed = 1.0f;
                    
                    base.Update();
                }
            }

            if (enemyData.isDamaged)
            {
                if (enemyData.eEnemyController == EnemyController.AI)
                {
                    enemyData.IsAttackDelay(enemyData.attackDelay - currentTime);
                }
                else if (enemyData.eEnemyController == EnemyController.PLAYER)
                {
                    enemyData.isAttack = false;
                    enemyData.enemyAnimator.speed = 1.0f;
                }

                ChangeState(new EnemyGetDamagedState(enemyData));
            }
        }

        protected override void End()
        {
            if (enemyData.isEndAttackAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashEndAttack);
            }

            enemyData.enemyAnimator.ResetTrigger(enemyData.hashAttack);

            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyData.isAttacking = false;
            }
        }

        private void SpriteFlipCheck()
        {
            enemyData.enemyRigidbody2D.velocity = Vector2.zero;
            enemyData.enemyRigidbody2D.angularVelocity = 0f;

            if (enemyData.eEnemyController == EnemyController.AI)
            {
                if (enemyData.isRotate)
                {
                    if (enemyData.enemyObject.transform.position.x > enemyData.PlayerObject.transform.position.x)
                    {
                        enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    }
                    else if (enemyData.enemyObject.transform.position.x < enemyData.PlayerObject.transform.position.x)
                    {
                        enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }
                }
                else
                {
                    if (enemyData.enemyObject.transform.position.x > enemyData.PlayerObject.transform.position.x)
                    {
                        enemyData.enemySpriteRenderer.flipX = true;
                    }
                    else if (enemyData.enemyObject.transform.position.x < enemyData.PlayerObject.transform.position.x)
                    {
                        enemyData.enemySpriteRenderer.flipX = false;
                    }
                }
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                float mousePositionX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

                if (enemyData.isRotate)
                {
                    if (enemyData.enemyObject.transform.position.x > mousePositionX)
                    {
                        enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    }
                    else if (enemyData.enemyObject.transform.position.x < mousePositionX)
                    {
                        enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }
                }
                else
                {
                    if (enemyData.enemyObject.transform.position.x > mousePositionX)
                    {
                        enemyData.enemySpriteRenderer.flipX = true;
                    }
                    else if (enemyData.enemyObject.transform.position.x < mousePositionX)
                    {
                        enemyData.enemySpriteRenderer.flipX = false;
                    }
                }
            }
        }
    }

    public partial class EnemyGetDamagedState : EnemyState // 공격을 받은 상태
    {
        private EnemyCommand[] enemyCommand = new EnemyCommand[2];

        private float currentTime;

        public EnemyGetDamagedState(EnemyData enemyData) : base(eState.GETDAMAGED, enemyData)
        {
            enemyCommand[0] = new EnemyGetDamagedCommand(enemyData);

            if (enemyData.isKnockBack)
            {
                if (enemyData.knockBackDirection != null)
                {
                    enemyCommand[1] = new EnemyAddForceCommand(enemyData.enemyRigidbody2D, enemyData.knockBackPower, null, enemyData.knockBackDirection.Value.normalized);
                }
                else
                {
                    enemyCommand[1] = new EnemyAddForceCommand(enemyData.enemyRigidbody2D, enemyData.knockBackPower, null, (enemyData.enemyObject.transform.position - enemyData.PlayerObject.transform.position).normalized);
                }
            }
        }

        protected override void Start()
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyData.hp -= enemyData.damagedValue;
                enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;
            }

            currentTime = 0f;

            if (enemyData.isHitAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashHit);
            }

            enemyCommand[0].Execute();

            if (enemyData.isKnockBack)
            {
                enemyCommand[1].Execute();
                currentTime -= enemyData.stunTime;

                enemyData.isKnockBack = false;
            }

            base.Start();
        }

        protected override void Update()
        {
            currentTime += Time.deltaTime;

            if (enemyData.hp <= 0)
            {
                currentTime = enemyData.damageDelay;
            }

            if (currentTime >= enemyData.damageDelay)
            {
                enemyCommand[0].Execute();

                if (enemyData.isHitAnimation)
                {
                    enemyData.enemyAnimator.ResetTrigger(enemyData.hashHit);
                }

                enemyData.isDamaged = false;

                base.Update();
            }
        }
    }

    public partial class EnemyDeadState : EnemyState // 죽었을때
    {
        private EnemyCommand deadCommand;

        private float currentTime = 0f;
        private float deadTime = 1f;

        public EnemyDeadState(EnemyData enemyData) : base(eState.DEAD, enemyData)
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                deadCommand = new EnemyDeadAIControllerCommand(enemyData.enemyObject, enemyData.enemyLootList, enemyData.enemyDeadEffectColor);
            }
        }

        protected override void Start()
        {
            enemyData.enemyAnimator.ResetTrigger(enemyData.hashReset);
            enemyData.enemyAnimator.SetTrigger(enemyData.hashIsDie);
            currentTime = 0f;

            base.Start();
        }

        protected override void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime > deadTime)
            {
                base.Update();
            }
        }

        protected override void End()
        {
            enemyData.enemyAnimator.ResetTrigger(enemyData.hashIsDie);
            enemyData.enemyAnimator.SetBool(enemyData.hashIsDead, true);

            deadCommand.Execute();
        }
    }
}