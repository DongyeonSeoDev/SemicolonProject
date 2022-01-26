using UnityEngine;

namespace Enemy
{
    public class EnemyBullet : PoolManager
    {
        public float speed;

        public Vector2 limitMaxPosition;
        public Vector2 limitMinPosition;

        private EnemyController eEnemyController;

        private Vector3 targetDirection;

        private int attackDamage;

        private void Start()
        {
            SlimeEventManager.StartListening("AfterPlayerRespawn", () =>
            {
                gameObject.SetActive(false);
            });
        }

        private void Update()
        {
            transform.position += targetDirection * speed * Time.deltaTime;

            if (transform.position.x < limitMinPosition.x || transform.position.x > limitMaxPosition.x 
                || transform.position.y < limitMinPosition.y || transform.position.y > limitMaxPosition.y)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                Player player = collision.GetComponent<Player>();

                if (player != null)
                {
                    collision.GetComponent<Player>().GetDamage(attackDamage);
                }
                else
                {
                    collision.GetComponent<EnemyAttackTest>().EnemyAttack(attackDamage);
                }

                gameObject.SetActive(false);
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.GetDamage(attackDamage);

                    gameObject.SetActive(false);
                }
            }
            else if (collision.CompareTag("Wall"))
            {
                gameObject.SetActive(false);
            }
        }

        public void Init(EnemyController controller, int damage, Vector3 direction)
        {
            eEnemyController = controller;
            attackDamage = damage;
            targetDirection = direction;
        }
    }
}