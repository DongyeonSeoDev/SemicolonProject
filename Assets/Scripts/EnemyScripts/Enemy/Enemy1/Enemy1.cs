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
                hpBarFillImage = hpBarFillImage,
                normalColor = Color.magenta,
                damagedColor = Color.green,
                enemyId = "Slime_01"
            };

            base.Awake();
        }

        private void Start()
        {
            EnemyAttackCheck enemyAttackCheck = transform.GetComponentInChildren<EnemyAttackCheck>();

            if (enemyAttackCheck != null)
            {
                enemyAttackCheck.Init(enemyData.eEnemyController, enemyData.attackDamage);
                enemyAttackCheck.gameObject.SetActive(false);
            }
        }
    }
}