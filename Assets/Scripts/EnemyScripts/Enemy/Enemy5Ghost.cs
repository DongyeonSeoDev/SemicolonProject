using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Enemy5Ghost : Enemy
    {
        [SerializeField]
        private Collider2D damageCollider;

        private EnemyCommand moveCommand = null;
        private WaitForSeconds ws = new WaitForSeconds(0.2f);
        private WaitWhile ww = new WaitWhile(() => SlimeGameManager.Instance.Player.PlayerState.IsDrain);

        private float currentTime = 0f;
        private float moveTime = 2f;
        private float minMoveTime = 2f;
        private float maxMoveTime = 5f;
        private float randomTeleportPosition = 5f;
        private bool isTeleport = false;
        private bool isAttackMove = false;

        private readonly int hashTeleport = Animator.StringToHash("Teleport");
        private readonly int hashTeleportEnd = Animator.StringToHash("TeleportEnd");

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 1f;
            enemyData.isAttackPlayerDistance = 1.5f;

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyMoveCommand(enemyData, transform, enemyData.chaseSpeed);
            moveCommand = new EnemyMoveCommand(enemyData, transform, 1f);
            enemyData.moveEvent = MoveEvent;

            moveTime = Random.Range(minMoveTime, maxMoveTime);
        }

        protected override void Update()
        {
            base.Update();

            if (isStop)
            {
                return;
            }

            if (isAttackMove)
            {
                moveCommand.Execute();
            }
        }

        public void ReadyAttack() // 애니메이션에서 실행
        {
            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }

            isAttackMove = true;
        }

        public void AttackEnd() // 애니메이션에서 실행
        {
            isAttackMove = false;
        }

        public void TeleportEnd() // 애니메이션에서 실행
        {
            enemyData.isAttack = true;
        }

        private void MoveEvent()
        {
            if (isTeleport)
                return;

            currentTime += Time.deltaTime;

            if (currentTime > moveTime)
            {
                StartCoroutine(Teleport());
            }
        }

        private IEnumerator Teleport()
        {
            isTeleport = true;

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Move, enemyData.enemyAnimator, TriggerType.SetTrigger);
            enemyData.enemyAnimator.SetTrigger(hashTeleport);

            damageCollider.enabled = false;

            int count = Random.Range(10, 15);

            for (int i = 0; i < count; i++)
            {
                yield return ws;
                yield return ww;
            }

            Vector2 teleportPosition = SlimeGameManager.Instance.CurrentPlayerBody.transform.position;

            teleportPosition.x += Random.Range(-randomTeleportPosition, randomTeleportPosition);
            teleportPosition.y += Random.Range(-randomTeleportPosition, randomTeleportPosition);

            transform.position = teleportPosition;

            enemyData.enemyAnimator.SetTrigger(hashTeleportEnd);

            damageCollider.enabled = true;

            currentTime = 0f;
            moveTime = Random.Range(minMoveTime, maxMoveTime);

            isTeleport = false;
        }
    }
}