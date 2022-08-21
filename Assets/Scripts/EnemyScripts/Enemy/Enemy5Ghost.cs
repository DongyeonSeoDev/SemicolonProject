using UnityEngine;

namespace Enemy
{
    public class Enemy5Ghost : Enemy
    {
        private float currentTime = 0f;
        private float moveTime = 2f;
        private float minMoveTime = 2f;
        private float maxMoveTime = 5f;
        private float randomTeleportPosition = 2f;
        private bool isTeleport = false;

        private readonly int hashTeleport = Animator.StringToHash("Teleport");
        private readonly int hashTeleportEnd = Animator.StringToHash("TeleportEnd");

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 0.5f;
            enemyData.isAttackPlayerDistance = 1.5f;

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyMoveCommand(enemyData, transform, enemyData.chaseSpeed);
            enemyData.moveEvent = MoveEvent;

            moveTime = Random.Range(minMoveTime, maxMoveTime);
        }

        public void ReadyAttack() // 애니메이션에서 실행
        {
            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }
        }

        private void MoveEvent()
        {
            currentTime += Time.deltaTime;

            if (currentTime > moveTime)
            {
                StartTeleport();
            }
        }

        private void StartTeleport()
        {
            if (!isTeleport)
            {
                isTeleport = true;

                EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Move, enemyData.enemyAnimator, TriggerType.SetTrigger);
                enemyData.enemyAnimator.SetTrigger(hashTeleport);

                Invoke("EndTeleport", 3f);
            }
        }

        private void EndTeleport()
        {
            Vector2 teleportPosition = SlimeGameManager.Instance.CurrentPlayerBody.transform.position;

            teleportPosition.x += Random.Range(-randomTeleportPosition, randomTeleportPosition);
            teleportPosition.y += Random.Range(-randomTeleportPosition, randomTeleportPosition);

            transform.position = teleportPosition;

            enemyData.enemyAnimator.SetTrigger(hashTeleportEnd);

            currentTime = 0f;
            moveTime = Random.Range(minMoveTime, maxMoveTime);

            isTeleport = false;
        }
    }
}