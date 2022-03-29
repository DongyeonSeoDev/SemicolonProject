using UnityEngine;

namespace Enemy
{
    public class Enemy1 : Enemy // 첫번째 적
    {
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
                isEndAttackAnimation = true,
                playerAnimationTime = 0.85f
            };

            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, false);
            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMY");

            base.OnEnable();
        }

        public void InitData(out EnemyController controller, out int damage)
        {
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }

        public void AttackReset()
        {
            EventManager.TriggerEvent("AttackStart");
        }
    }
}