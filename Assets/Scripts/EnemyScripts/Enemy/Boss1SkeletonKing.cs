using UnityEngine;

namespace Enemy
{
    public class Boss1SkeletonKing : Enemy
    {
        public Transform movePivot;

        private EnemyCommand attackMoveCommand;

        protected override void Start()
        {
            attackMoveCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 15f, 0f, false);

            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 1.8f;
            enemyData.isAttackPlayerDistance = 3.5f;
            enemyData.attackPower = 30;
            enemyData.maxHP = 500;
            enemyData.hp = 500;
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 5f, 0f, false);
        }

        public void AttackMove() // 애니메이션에서 실행
        {
            rb.velocity = Vector2.zero;
            enemyAttackCheck.AttackObjectReset();

            attackMoveCommand.Execute();
        }
    }
}