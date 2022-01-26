using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy
    {
        private EnemyCommand enemyAttackCommand;
        private Rigidbody2D rb2d;

        protected override void OnEnable()
        {
            enemyData = new EnemyData()
            {
                eEnemyController = EnemyController.AI,
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyLootList = enemyLootListSO,
                enemyAnimator = GetComponent<Animator>(),
                enemySpriteRenderer = GetComponent<SpriteRenderer>(),
                enemyRigidbody2D = GetComponent<Rigidbody2D>(),
                hpBarFillImage = hpBarFillImage,
                normalColor = Color.white,
                damagedColor = Color.red,
                enemyId = "Slime_03",
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

            rb2d = GetComponent<Rigidbody2D>();

            enemyAttackCommand = new EnemyRushAttackCommand(rb2d, transform, enemyData.PlayerObject.transform, enemyData.rushForce);

            base.OnEnable();
        }

        public void InitData(out EnemyController controller, out int damage)
        {
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }

        public void EnemyAttack()
        {
            enemyAttackCommand.Execute();
        }
    }
}