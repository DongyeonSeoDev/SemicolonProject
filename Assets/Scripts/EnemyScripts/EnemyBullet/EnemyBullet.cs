using UnityEngine;

namespace Enemy
{
    public class EnemyBullet : EnemyPoolData
    {
        public float speed;
        public float addAngle;
        public bool isBulletEffect;

        private float angle;
        private bool isStop = false;

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


            EventManager.StartListening("ExitCurrentMap", () =>
            {
                gameObject.SetActive(false);
            });

            EventManager.StartListening("EnemyStart", () =>
            {
                isStop = false;
            });

            EventManager.StartListening("EnemyStop", () =>
            {
                isStop = true;
            });
        }

        private void Update()
        {
            if (isStop)
            {
                return;
            }

            transform.position += targetDirection * speed * Time.deltaTime;

            if (transform.position.x < limitMinPosition.x || transform.position.x > limitMaxPosition.x 
                || transform.position.y < limitMinPosition.y || transform.position.y > limitMaxPosition.y)
            {
                gameObject.SetActive(false);
            }
        }

        private void StartBulletEffect()
        {
            if (isBulletEffect)
            {
                EnemyPoolManager.Instance.GetPoolObject(Type.BulletEffect, transform.position).GetComponent<BulletEffect>().Play(angle);
            }

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(gameObject, Random.Range(attackDamage - 5, attackDamage + 6));

                StartBulletEffect();
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

                    StartBulletEffect();

                    EventManager.TriggerEvent("OnEnemyAttack");
                }
            }

            if (collision.CompareTag("Wall"))
            {
                if (eEnemyController == EnemyController.PLAYER)
                {
                    EventManager.TriggerEvent("OnAttackMiss");
                }

                StartBulletEffect();
            }
        }

        public void Init(EnemyController controller, Vector3 direction, int damage, Enemy enemy = null)
        {
            eEnemyController = controller;
            attackDamage = damage;
            targetDirection = direction;
            this.enemy = enemy;

            angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - addAngle);

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