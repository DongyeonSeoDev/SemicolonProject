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
        public Attack(EnemyData enemyData) : base(eState.ATTACK, enemyData)
        {

        }

        protected override void Start()
        {
            enemyData.enemyAnimator.SetTrigger(enemyData.hashAttack);

            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void End()
        {
            enemyData.enemyAnimator.ResetTrigger(enemyData.hashAttack);
        }
    }

    public partial class GetDamaged : State // ������ ���� ����
    {
        public GetDamaged(EnemyData enemyData) : base(eState.GETDAMAGED, enemyData)
        {

        }

        protected override void Start()
        {
            Debug.Log("(�������� ����) ������ : " + enemyData.damagedValue);

            enemyData.isDamaged = false;

            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void End()
        {

        }
    }
}