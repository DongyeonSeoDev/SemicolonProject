using UnityEngine;

namespace Enemy
{
    public class EnemyBullet : EnemyPoolData
    {
        public float speed;
        private float angle;

        public Vector2 limitMaxPosition;
        public Vector2 limitMinPosition;

        private EnemyController eEnemyController;

        private Vector3 targetDirection;

        private int attackDamage;

        private Enemy enemy;

        private void Start()
        {
            EventManager.StartListening("AfterPlayerRespawn", () =>
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
                SlimeGameManager.Instance.Player.GetDamage(Random.Range(attackDamage - 5, attackDamage + 6));

                gameObject.SetActive(false);
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null && enemy != this.enemy)
                {
                    (int, bool) damage;
                    damage.Item1 = Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);

                    enemy.GetDamage(damage.Item1, damage.Item2);

                    gameObject.SetActive(false);
                }
            }
            else if (collision.CompareTag("Wall"))
            {
                gameObject.SetActive(false);
            }
        }

        public void Init(EnemyController controller, int damage, Vector3 direction, Enemy enemy = null)
        {
            eEnemyController = controller;
            attackDamage = damage;
            targetDirection = direction;

            angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            if (controller == EnemyController.AI)
            {
                gameObject.layer = LayerMask.NameToLayer("ENEMYPROJECTILE");
            }
            else if (controller == EnemyController.PLAYER)
            {
                gameObject.layer = LayerMask.NameToLayer("PLAYERPROJECTILE");
            }
        }
    }
}