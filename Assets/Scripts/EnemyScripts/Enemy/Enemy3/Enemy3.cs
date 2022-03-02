using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy
    {
        private EnemyCommand enemyAttackCommand;

        private EnemyRushAttackCommand.RushAttackPosition rushAttackPosition = new EnemyRushAttackCommand.RushAttackPosition();

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
                isSeePlayerDistance = 20f,
                isAttackPlayerDistance = 7f,
                attackDelay = 2f,
                chaseSpeed = 2f,
                attackDamage = 20,
                maxHP = 50,
                hp = 50
            };

            enemyAttackCommand = new EnemyRushAttackCommand(rb, rushAttackPosition, enemyData.rushForce);

            base.OnEnable();
        }

        public void InitData(out EnemyController controller, out int damage)
        {
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }

        public void ReadyEnemyAttack()
        {
            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                rushAttackPosition.position = (playerInput.MousePosition - (Vector2)transform.position).normalized;
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                rushAttackPosition.position = (enemyData.PlayerObject.transform.position - enemyData.enemyObject.transform.position).normalized;
            }
        }

        public void EnemyAttack()
        {
            enemyAttackCommand.Execute();
        }
    }
}