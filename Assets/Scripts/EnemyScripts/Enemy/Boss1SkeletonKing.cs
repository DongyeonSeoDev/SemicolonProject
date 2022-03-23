using UnityEngine;

namespace Enemy
{
    public class Boss1SkeletonKing : EnemyPoolData
    {
        private Rigidbody2D rigid;
        private Animator animator;
        private SpriteRenderer sr;
        private Transform playerTransform;
        private EnemyCommand command;

        private float lastPositionX = 0f;
        private bool isMove = false;

        private readonly int hashMove = Animator.StringToHash("move");
        private readonly int hashIsAttack = Animator.StringToHash("isAttack");

        private void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();

            playerTransform = SlimeGameManager.Instance.CurrentPlayerBody.transform;
            command = new EnemyFollowPlayerCommand(transform, playerTransform, rigid, 7f, 0f, false);
        }

        private void Update()
        {
            if (isMove)
            {
                if (Vector2.Distance(transform.position, playerTransform.position) < 4f)
                {
                    animator.SetBool(hashIsAttack, true);
                }
                else
                {
                    animator.SetBool(hashIsAttack, false);
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
    }
}