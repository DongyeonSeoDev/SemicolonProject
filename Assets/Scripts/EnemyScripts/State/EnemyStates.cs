using UnityEngine;

namespace Enemy
{
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
            enemyData.enemyAnimator.SetTrigger(enemyData.hashMove);

            base.Start();
        }

        protected override void Update()
        {
            enemyMoveCommand.Execute();

            base.Update();
        }

        protected override void End() => enemyData.enemyAnimator.ResetTrigger(enemyData.hashMove);
    }

    public partial class EnemyChaseState : EnemyState // 추격 상태
    {
        private EnemyCommand enemyFollowPlayerCommand;

        public EnemyChaseState(EnemyData enemyData) : base(eState.CHASE, enemyData) =>
            enemyFollowPlayerCommand = new EnemyFollowPlayerCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.chaseSpeed);

        protected override void Start()
        {
            enemyData.enemyAnimator.SetTrigger(enemyData.hashMove);

            base.Start();
        }

        protected override void Update()
        {
            enemyFollowPlayerCommand.Execute();

            base.Update();
        }

        protected override void End() => enemyData.enemyAnimator.ResetTrigger(enemyData.hashMove);
    }

    public partial class EnemyAttackState : EnemyState // 공격 상태
    {
        public EnemyAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);

            base.Start();
        }

        protected override void Update() => base.Update();
        protected override void End() => enemyData.enemyAnimator.ResetTrigger(enemyData.hashAttack);
    }

    public partial class EnemyGetDamagedState : EnemyState // 공격을 받은 상태
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

            if (!enemyData.isHitAnimation)
            {
                enemyGetDamaged.Execute();

                currentTime = 0f;
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
                if (!enemyData.isHitAnimation)
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

    public partial class EnemyDeadState : EnemyState // 죽었을때
    {
        private EnemyCommand deadCommand;

        private float currentTime = 0f;
        private float deadTime = 1f;

        public EnemyDeadState(EnemyData enemyData) : base(eState.DEAD, enemyData)
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                deadCommand = new EnemyDeadAIControllerCommand(enemyData.enemyObject);
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                deadCommand = new EnemyDeadPlayerControllerCommand();
            }
        }

        protected override void Start()
        {
            enemyData.enemyAnimator.SetTrigger(enemyData.hashIsDie);

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