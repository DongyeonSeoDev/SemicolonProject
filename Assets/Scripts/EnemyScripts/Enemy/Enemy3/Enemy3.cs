using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy
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
                normalColor = Color.white,
                damagedColor = Color.red,
                enemyId = "Slime_03",
                isAnimation = false,
                isSeePlayerDistance = 20f,
                chaseSpeed = 2.5f,
                maxHP = 50,
                hp = 50
            };

            base.Awake();
        }
    }
}