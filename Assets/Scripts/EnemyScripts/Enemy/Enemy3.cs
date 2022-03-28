using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy // ����° ��
    {
        private EnemyCommand enemyAttackCommand;

        private EnemyPositionCheckData positionCheckData = new EnemyPositionCheckData();

        protected override void OnEnable()
        {
            enemyData = new EnemyData(enemyDataSO)
            {
                enemyObject = gameObject,
                enemyLootList = enemyLootListSO,
                enemyAnimator = anim,
                enemySpriteRenderer = sr,
                enemyRigidbody2D = rb,
                hpBarFillImage = hpBarFillImage,
                isAnimation = false,
                isEndAttackAnimation = true,
                isUseDelay = true,
                isAttackPlayerDistance = 7f,
                attackDelay = 2f,
                chaseSpeed = 2f,
                attackDamage = 20,
                maxHP = 50,
                hp = 50,
                playerAnimationTime = 1f
            };

            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, false);
            enemyAttackCommand = new EnemyAddForceCommand(rb, enemyData.rushForce, positionCheckData);
            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMY");

            base.OnEnable();
        }

        public void InitData(out EnemyPositionCheckData positionData, out EnemyController controller, out int damage)
        {
            positionData = positionCheckData;
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }

        public void ReadyEnemyAttack()
        {
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

            EventManager.TriggerEvent("AttackStart");
        }

        public void EnemyAttack()
        {
            enemyAttackCommand.Execute();
        }
    }
}