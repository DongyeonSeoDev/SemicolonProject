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

    public partial class EnemyChaseState : EnemyState // 추격 상태
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

    public partial class EnemyAttackState : EnemyState // 공격 상태
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

            base.Update();

            if (currentTime >= enemyData.attackDelay)
            {
                currentTime = 0f;

                SpriteFlipCheck();
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

    public partial class EnemyGetDamagedState : EnemyState // 공격을 받은 상태
    {
        private EnemyCommand enemyGetDamagedCommand;

        private float currentTime;

        public EnemyGetDamagedState(EnemyData enemyData) : base(eState.GETDAMAGED, enemyData)
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyGetDamagedCommand = new EnemyGetDamagedAIControllerCommand(enemyData);
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyGetDamagedCommand = new EnemyGetDamagedPlayerControllerCommand();
            }
        }

        protected override void Start()
        {
            enemyData.hp -= enemyData.damagedValue;
            enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;

            currentTime = 0f;

            if (enemyData.isHitAnimation)
            {
                enemyData.enemyAnimator.SetTrigger(enemyData.hashHit);
            }

            enemyGetDamagedCommand.Execute();

            base.Start();
        }

        protected override void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime > enemyData.damageDelay)
            {
                enemyGetDamagedCommand.Execute();

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