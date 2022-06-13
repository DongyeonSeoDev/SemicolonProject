using UnityEngine;

namespace Enemy
{
    public class Boss1Clone : EnemyPoolData
    {
        private Rigidbody2D rigid;
        public SpriteRenderer sr;

        private Vector2 targetPosition = Vector2.zero;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            EventManager.StartListening("PlayerRespawn", PlayerDeadEvent);
            EventManager.StartListening("BossDead", PlayerDeadEvent);
        }

        private void Update()
        {
            if (targetPosition.x < 0)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (targetPosition.x > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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