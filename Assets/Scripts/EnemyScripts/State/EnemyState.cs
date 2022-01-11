using UnityEngine;

namespace Enemy
{
    public partial class Move : State // 움직임 상태
    {
        private Command enemyMoveCommand;

        public Move(EnemyData enemyData) : base(eState.MOVE, enemyData) => 
            enemyMoveCommand = new EnemyMove(enemyData.enemyMoveSO, enemyData.enemyObject.transform);

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

    public partial class Chase : State // 추격 상태
    {
        private Command enemyFollowPlayerCommand;

        public Chase(EnemyData enemyData) : base(eState.CHASE, enemyData) =>
            enemyFollowPlayerCommand = new EnemyFollowPlayer(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.chaseSpeed);

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

    public partial class Attack : State // 공격 상태
    {
        public Attack(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);

            base.Start();
        }

        protected override void Update() => base.Update();
        protected override void End() => enemyData.enemyAnimator.ResetTrigger(enemyData.hashAttack);
    }

    public partial class GetDamaged : State // 공격을 받은 상태
    {
        private Command enemyGetDamaged;

        private float currentTime;

        public GetDamaged(EnemyData enemyData) : base(eState.GETDAMAGED, enemyData)
        {
            enemyGetDamaged = new EnemyGetDamaged(enemyData);
        }

        protected override void Start()
        {
            enemyGetDamaged.Execute();
            currentTime = 0f;

            base.Start();
        }

        protected override void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime > enemyData.damageDelay)
            {
                enemyGetDamaged.Execute();

                base.Update();
            }
        }
    }

    public partial class Dead : State // 죽었을때
    {
        private Command deadCommand;

        private float currentTime = 0f;
        private float deadTime = 1f;

        public Dead(EnemyData enemyData) : base(eState.DEAD, enemyData) => deadCommand = new EnemyDead(enemyData.enemyObject);

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