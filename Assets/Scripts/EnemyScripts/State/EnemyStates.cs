using UnityEngine;

namespace Enemy
{
    public partial class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyData enemyData) : base(eState.IDLE, enemyData) { }
    }

    public partial class EnemyMoveState : EnemyState // ������ ����
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

    public partial class EnemyChaseState : EnemyState // �߰� ����
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

    public partial class EnemyAttackState : EnemyState // ���� ����
    {
        private float currentTime;
        private bool isDelay = false;

        public EnemyAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            if (enemyData.isUseDelay)
            {
                isDelay = enemyData.IsAttackDelay();
            }

            if (enemyData.isEndAttackAnimation)
            {
                enemyData.enemyAnimator.ResetTrigger(enemyData.hashEndAttack);
            }

            if (!isDelay)
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);
            }

            currentTime = 0f;
            SpriteFlipCheck();

            base.Start();
        }

        protected override void Update()
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

            if (enemyData.isDamaged)
            {
                enemyData.IsAttackDelay(enemyData.attackDelay - currentTime);
                ChangeState(new EnemyGetDamagedState(enemyData));
            }

            if (currentTime >= enemyData.attackDelay)
            {
                currentTime = 0f;

                SpriteFlipCheck();

                base.Update();
            }
        }

        protected override void End()
        {
            if (enemyData.isEndAttackAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashEndAttack);
            }

            enemyData.enemyAnimator.ResetTrigger(enemyData.hashAttack);
        }

        private void SpriteFlipCheck()
        {
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
        }
    }

    public partial class EnemyGetDamagedState : EnemyState // ������ ���� ����
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

            if (currentTime > enemyData.damageDelay)
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

    public partial class EnemyDeadState : EnemyState // �׾�����
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