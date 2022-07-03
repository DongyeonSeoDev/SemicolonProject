using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy // 세번째 적
    {
        public SpriteRenderer anotherImage;

        public float downSpeed = 0f;

        private float moveSpeed = 0f;
        private float currentDownSpeed = 0f;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isUseKnockBack = true;
            enemyData.isAttackPlayerDistance = 7f;
            enemyData.attackDelay = 2f;
            enemyData.chaseSpeed = 2f;
            enemyData.attackPower = 20;
            enemyData.maxHP = 30;
            enemyData.hp = 30;
            enemyData.playerAnimationTime = 1f;

            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed);
            enemyData.enemySpriteRotateCommand = new EnemySpriteFlipCommand(enemyData);
            enemyData.deadEvent = DeadEvent;

            moveSpeed = 0f;
        }

        protected override void Update()
        {
            base.Update();

            if (isStop)
            {
                return;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (moveSpeed > 0f)
            {
                rb.velocity = positionCheckData.position * moveSpeed;

                moveSpeed = Mathf.Lerp(moveSpeed, 0f, currentDownSpeed);

                if (moveSpeed < 1f)
                {
                    enemyData.isMovePositionReset = true;
                    moveSpeed = 0f;
                }

                currentDownSpeed += 0.005f;
            }
        }

        public void ReadyEnemyAttack() // 애니메이션에서 실행 - 적 공격 준비
        {
            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }

            if (positionCheckData.isWall)
            {
                rb.velocity = positionCheckData.oppositeDirectionWall;
            }

            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                positionCheckData.position = (playerInput.MousePosition - (Vector2)transform.position).normalized;
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                positionCheckData.position = (EnemyManager.Player.transform.position - enemyData.enemyObject.transform.position).normalized;
            }
        }

        private void AttackStart() // 애니메이션에서 실행
        {
            moveSpeed = 30f;
            currentDownSpeed = downSpeed;
        }

        private void AttackEnd() // 애니메이션에서 실행
        {
            moveSpeed = 0f;
            rb.velocity = Vector2.zero;
        }

        private void DeadEvent()
        {
            moveSpeed = 0f;
        }

        public override void ChangeColor(Color color)
        {
            anotherImage.color = color;

            base.ChangeColor(color);
        }
    }
}