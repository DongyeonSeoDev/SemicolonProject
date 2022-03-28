using UnityEngine;

namespace Enemy
{
    public class Boss1SkeletonKing : EnemyPoolData
    {
        public Transform movePivot;

        private Rigidbody2D rigid;
        private Animator animator;
        private SpriteRenderer sr;
        private Transform playerTransform;
        private EnemyCommand command;
        private EnemyCommand command2;

        private float lastPositionX = 0f;
        private bool isMove = false;
        private bool isAttack = false;

        private readonly int hashMove = Animator.StringToHash("move");
        private readonly int hashIsAttack = Animator.StringToHash("attack");

        private void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();

            playerTransform = SlimeGameManager.Instance.CurrentPlayerBody.transform;
            // command = new EnemyFollowPlayerCommand(movePivot, playerTransform, rigid, 5f, 0f, false);
            // command2 = new EnemyFollowPlayerCommand(movePivot, playerTransform, rigid, 15f, 0f, false);
        }

        private void Update()
        {
            if (isAttack)
            {
                return;
            }

            if (isMove)
            {
                if (Vector2.Distance(transform.position, playerTransform.position) < 3.5f)
                {
                    animator.ResetTrigger(hashMove);
                    animator.SetTrigger(hashIsAttack);

                    rigid.velocity = Vector2.zero;
                    isAttack = true;
                }
                else
                {
                    command.Execute();
                }
            }

            if (lastPositionX > transform.position.x)
            {
                sr.flipX = true;
            }
            else if (lastPositionX < transform.position.x)
            {
                sr.flipX = false;
            }

            lastPositionX = transform.position.x;
        }

        public void Move()
        {
            isMove = true;
            animator.SetTrigger(hashMove);
        }

        public void AttackEnd()
        {
            animator.ResetTrigger(hashIsAttack);
            animator.SetTrigger(hashMove);

            isAttack = false;
        }

        public void AttackMove()
        {
            command2.Execute();
        }
    }
}