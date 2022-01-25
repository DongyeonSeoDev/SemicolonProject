using UnityEngine;

namespace Enemy
{
    public partial class EnemyMoveState : EnemyState // ������ ����
    {
        private EnemyCommand enemyMoveCommand;

        public EnemyMoveState(EnemyData enemyData) : base(eState.MOVE, enemyData)
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyMoveCommand = new EnemyMoveAIControllerCommand(enemyData.enemyMoveSO, enemyData.enemyObject.transform);
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyMoveCommand = new EnemyMovePlayerControllerCommand();
            }
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
        private EnemyCommand enemyFollowPlayerCommand;

        public EnemyChaseState(EnemyData enemyData) : base(eState.CHASE, enemyData) =>
            enemyFollowPlayerCommand = new EnemyFollowPlayerCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, enemyData.isLongDistanceAttack);

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
            enemyFollowPlayerCommand.Execute();

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

        public EnemyAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);

            currentTime = 0f;

            SpriteFlipCheck();

            base.Start();
        }

        protected override void Update()
        {
            currentTime += Time.deltaTime;

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

    public partial class EnemyGetDamagedState : EnemyState // ������ ���� ����
    {
        private EnemyCommand enemyGetDamaged;

        private float currentTime;

        public EnemyGetDamagedState(EnemyData enemyData) : base(eState.GETDAMAGED, enemyData)
        {
            if (!enemyData.isHitAnimation)
            {
                if (enemyData.eEnemyController == EnemyController.AI)
                {
                    enemyGetDamaged = new EnemyGetDamagedAIControllerCommand(enemyData);
                }
                else if (enemyData.eEnemyController == EnemyController.PLAYER)
                {
                    enemyGetDamaged = new EnemyGetDamagedPlayerControllerCommand();
                }
            } 
        }

        protected override void Start()
        {
            enemyData.hp -= enemyData.damagedValue;
            enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;

            currentTime = 0f;

            if (enemyGetDamaged != null)
            {
                enemyGetDamaged.Execute();
            }
            else
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashHit);
            }

            base.Start();
        }

        protected override void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime > enemyData.damageDelay)
            {
                if (enemyGetDamaged != null)
                {
                    enemyGetDamaged.Execute();
                }
                else
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
                deadCommand = new EnemyDeadAIControllerCommand(enemyData.enemyObject, enemyData.enemyLootList);
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                deadCommand = new EnemyDeadPlayerControllerCommand();
            }
        }

        protected override void Start()
        {
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