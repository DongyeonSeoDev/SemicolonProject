using UnityEngine;

namespace Enemy
{
    public class Enemy1 : Enemy // 첫번째 적
    {
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
                normalColor = Color.magenta,
                damagedColor = Color.green,
                enemyDeadEffectColor = Util.Change255To1Color(96f, 0f, 106f),
                playerNormalColor = Color.white,
                playerDamagedColor = Color.red,
                playerDeadEffectColor = Color.white,
                enemyType = EnemyType.Slime_01
            };

            base.OnEnable();
        }

        public void InitData(out EnemyController controller, out int damage)
        {
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }
    }
}