using UnityEngine;

namespace Enemy
{
    public class Enemy1 : Enemy // 첫번째 적
    {
        protected override void Awake()
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
                normalColor = Color.magenta,
                damagedColor = Color.green,
                enemyId = "Slime_01"
            };

            base.Awake();
        }

        public void InitData(out EnemyController controller, out int damage)
        {
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }
    }
}