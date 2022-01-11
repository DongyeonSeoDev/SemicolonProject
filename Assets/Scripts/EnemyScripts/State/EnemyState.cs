using UnityEngine;

namespace Enemy
{
    public partial class Move : State // ������ ����
    {
        private Command enemyMoveCommand;

        public Move(EnemyData enemyData) : base(eState.MOVE, enemyData)
        {
            enemyMoveCommand = new EnemyMove(enemyData.enemyMoveSO, enemyData.enemyObject.transform);
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

        protected override void End()
        {
            enemyData.enemyAnimator.ResetTrigger(enemyData.hashMove);
        }
    }

    public partial class Chase : State // �߰� ����
    {
        private Command enemyFollowPlayerCommand;

        public Chase(EnemyData enemyData) : base(eState.CHASE, enemyData)
        {
            enemyFollowPlayerCommand = new EnemyFollowPlayer(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.chaseSpeed);
        }

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

        protected override void End()
        {
            enemyData.enemyAnimator.ResetTrigger(enemyData.hashMove);
        }
    }

    public partial class Attack : State // ���� ����
    {
        private Command enemyAttackCommand;

        public Attack(EnemyData enemyData) : base(eState.ATTACK, enemyData)
        {
            enemyAttackCommand = new EnemyAttack();
        }

        protected override void Start()
        {
            enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);

            base.Start();
        }

        protected override void Update()
        {
            enemyAttackCommand.Execute();

            base.Update();
        }

        protected override void End()
        {
            enemyData.enemyAnimator.ResetTrigger(enemyData.hashAttack);
        }
    }
}