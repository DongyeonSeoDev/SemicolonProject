using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy
    {
        private EnemyCommand enemyAttackCommand;

        private EnemyRushAttackCommand.RushAttackPosition rushAttackPosition = new EnemyRushAttackCommand.RushAttackPosition();

        protected override void OnEnable()
        {
            enemyData = new EnemyData()
            {
                eEnemyController = EnemyController.AI,
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyLootList = enemyLootListSO,
                enemyAnimator = anim,
                enemySpriteRenderer = sr,
                enemyRigidbody2D = rb,
                hpBarFillImage = hpBarFillImage,
                normalColor = Color.white,
                damagedColor = Color.red,
                enemyDeadEffectColor = Util.Change255To1Color(99f, 187f, 57f),
                playerNormalColor = Color.green,
                playerDamagedColor = Color.red,
                playerDeadEffectColor = Color.green,
                enemyType = EnemyType.Slime_03,
                isAnimation = false,
                isEndAttackAnimation = true,
                isSeePlayerDistance = 20f,
                isAttackPlayerDistance = 7f,
                attackDelay = 3f,
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