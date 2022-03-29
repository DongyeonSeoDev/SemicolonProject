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
            enemyMoveCommand = new EnemyMovePlayerControllerCommand(enemyData, enemyData.enemyRigidbody2D);
        }

        protected override void Start()
        {
            if (enemyData.isAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(EnemyManager.hashMove);
            }

            base.Start();
        }

        protected override void Update()
        {
            enemyMoveCommand.Execute();

            if (enemyData.isRotate)
            {
                if (enemyData.moveVector.x < 0)
                {
                    enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (enemyData.moveVector.x > 0)
                {
                    enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else
            {
                if (enemyData.moveVector.x < 0)
                {
                    enemyData.enemySpriteRenderer.flipX = true;
                }
                else if (enemyData.moveVector.x > 0)
                {
                    enemyData.enemySpriteRenderer.flipX = false;
                }
            }

            base.Update();
        }

        protected override void End()
        {
            if (enemyData.isAnimation)
            {
                enemyData.enemyAnimator.ResetTrigger(EnemyManager.hashMove);
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
                enemyData.enemyAnimator.SetTrigger(EnemyManager.hashMove);
            }

            base.Start();
        }

        protected override void Update()
        {
            enemyData.enemyMoveCommand.Execute();

            if (enemyData.isRotate)
            {
                if (enemyData.moveVector.x < 0)
                {
                    enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (enemyData.moveVector.x > 0)
                {
                    enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else
            {
                if (enemyData.moveVector.x < 0)
                {
                    enemyData.enemySpriteRenderer.flipX = true;
                }
                else if (enemyData.moveVector.x > 0)
                {
                    enemyData.enemySpriteRenderer.flipX = false;
                }
            }

            base.Update();
        }

        protected override void End()
        {
            if (enemyData.isAnimation)
            {
                enemyData.enemyAnimator.ResetTrigger(EnemyManager.hashMove);
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
                enemyData.enemyAnimator.ResetTrigger(EnemyManager.hashEndAttack);
            }

            if (enemyData.eEnemyController == EnemyController.AI)
            {
                if (enemyData.isUseDelay)
                {
                    isDelay = EnemyManager.IsAttackDelay(enemyData);
                }

                if (!isDelay)
                {
                    enemyData.enemyAnimator.SetTrigger(EnemyManager.hashAttack);
                }

                currentTime = 0f;
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                if (SlimeGameManager.Instance.CurrentSkillDelayTimer[0] <= 0)
                {
                    SlimeGameManager.Instance.SetSkillDelay(0, enemyData.playerAnimationDelay + enemyData.playerAnimationTime);
                    SlimeGameManager.Instance.CurrentSkillDelayTimer[0] = SlimeGameManager.Instance.SkillDelays[0];

                    enemyData.enemyAnimator.SetTrigger(EnemyManager.hashAttack);
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
                    isDelay = EnemyManager.IsAttackDelay(enemyData);

                    if (!isDelay)
                    {
                        enemyData.enemyAnimator.SetTrigger(EnemyManager.hashAttack);
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
                    EnemyManager.IsAttackDelay(enemyData, enemyData.attackDelay - currentTime);
                }
                else if (enemyData.eEnemyController == EnemyController.PLAYER)
                {
                    enemyData.enemyAnimator.speed = 1.0f;
                }

                ChangeState(new EnemyGetDamagedState(enemyData));
            }
        }

        protected override void End()
        {
            if (enemyData.isEndAttackAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(EnemyManager.hashEndAttack);
            }

            enemyData.enemyAnimator.ResetTrigger(EnemyManager.hashAttack);
        }

        private void SpriteFlipCheck()
        {
            enemyData.enemyRigidbody2D.velocity = Vector2.zero;
            enemyData.enemyRigidbody2D.angularVelocity = 0f;

            if (enemyData.eEnemyController == EnemyController.AI)
            {
                if (enemyData.isRotate)
                {
                    if (enemyData.enemyObject.transform.position.x > EnemyManager.Player.transform.position.x)
                    {
                        enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    }
                    else if (enemyData.enemyObject.transform.position.x < EnemyManager.Player.transform.position.x)
                    {
                        enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }
                }
                else
                {
                    if (enemyData.enemyObject.transform.position.x > EnemyManager.Player.transform.position.x)
                    {
                        enemyData.enemySpriteRenderer.flipX = true;
                    }
                    else if (enemyData.enemyObject.transform.position.x < EnemyManager.Player.transform.position.x)
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
                    enemyCommand[1] = new EnemyAddForceCommand(enemyData.enemyRigidbody2D, enemyData.knockBackPower, null, (enemyData.enemyObject.transform.position - EnemyManager.Player.transform.position).normalized);
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
                enemyData.enemyAnimator.SetTrigger(EnemyManager.hashHit);
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
                    enemyData.enemyAnimator.ResetTrigger(EnemyManager.hashHit);
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
            enemyData.enemyRigidbody2D.velocity = Vector2.zero;

            enemyData.enemyAnimator.ResetTrigger(EnemyManager.hashReset);
            enemyData.enemyAnimator.SetTrigger(EnemyManager.hashIsDie);
            currentTime = 0f;

            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMYDEAD");

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
            enemyData.enemyAnimator.ResetTrigger(EnemyManager.hashIsDie);
            enemyData.enemyAnimator.SetBool(EnemyManager.hashIsDead, true);

            deadCommand.Execute();
        }
    }
}