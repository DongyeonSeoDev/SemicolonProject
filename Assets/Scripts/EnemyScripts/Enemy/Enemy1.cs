using UnityEngine;

namespace Enemy
{
    public class Enemy1 : Enemy // ù��° ��
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.playerAnimationTime = 0.85f;
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, false); // MoveCommand �߰�
        }

        public void ReadyAttack() // �ִϸ��̼ǿ��� ����
        {
            enemyAttackCheck.AttackObjectReset();
        }
    }
}