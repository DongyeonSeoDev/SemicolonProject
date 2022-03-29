using UnityEngine;

namespace Enemy
{
    public class Enemy1 : Enemy // 첫번째 적
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.playerAnimationTime = 0.85f;
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, false); // MoveCommand 추가
        }

        public void ReadyAttack() // 애니메이션에서 실행
        {
            enemyAttackCheck.AttackObjectReset();
        }
    }
}