using UnityEngine;

namespace Enemy
{
    public class Enemy5Ghost : Enemy
    {
        private readonly int hashTeleport = Animator.StringToHash("Teleport");
        private readonly int hashTeleportEnd = Animator.StringToHash("TeleportEnd");

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 0.5f;
            enemyData.isAttackPlayerDistance = 1.5f;

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyMoveCommand(enemyData, transform, enemyData.chaseSpeed);
        }

        public void ReadyAttack() // 애니메이션에서 실행
        {
            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }
        }

        public void StartTeleport()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Move, enemyData.enemyAnimator, TriggerType.SetTrigger);
            enemyData.enemyAnimator.SetTrigger(hashTeleport);

            Invoke("EndTeleport", 3f);
        }

        public void EndTeleport()
        {
            enemyData.enemyAnimator.SetTrigger(hashTeleportEnd);
        }
    }
}