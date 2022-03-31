using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy // ����° ��
    {
        private EnemyCommand enemyAttackCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isUseDelay = true;
            enemyData.isUseKnockBack = true;
            enemyData.isAttackPlayerDistance = 7f;
            enemyData.attackDelay = 2f;
            enemyData.chaseSpeed = 2f;
            enemyData.attackPower = 20;
            enemyData.maxHP = 50;
            enemyData.hp = 50;
            enemyData.playerAnimationTime = 1f;

            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, false);
            enemyAttackCommand = new EnemyAddForceCommand(rb, this, enemyData.rushForce, positionCheckData);
            enemyData.enemySpriteRotateCommand = new EnemySpriteFlipCommand(enemyData);
        }

        public void ReadyEnemyAttack() // �ִϸ��̼ǿ��� ���� - �� ���� �غ�
        {
            enemyAttackCheck.AttackObjectReset();

            if (positionCheckData.isWall)
            {
                rb.velocity = positionCheckData.oppositeDirectionWall;
            }

            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                positionCheckData.position = (playerInput.MousePosition - (Vector2)transform.position).normalized;
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                positionCheckData.position = (EnemyManager.Player.transform.position - enemyData.enemyObject.transform.position).normalized;
            }
        }

        public void EnemyAttack() // �ִϸ��̼ǿ��� ���� - �� ����
        {
            enemyAttackCommand.Execute();
        }
    }
}