using UnityEngine;

namespace Enemy
{
    public class Boss1Clone : EnemyPoolData
    {
        private Rigidbody2D rigid;
        private SpriteRenderer spriteRenderer;

        private Vector2 targetPosition = Vector2.zero;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            EventManager.StartListening("PlayerSetActiveFalse", PlayerDeadEvent);
        }

        private void Update()
        {
            if (targetPosition.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (targetPosition.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }

        public void PlayerDeadEvent()
        {
            gameObject.SetActive(false);
        }

        public void MovePosition(Vector2 target)
        {
            targetPosition = target;
            rigid.velocity = targetPosition;
        }
    }
}