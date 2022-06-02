using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Enemy
{
    public class EnemyBullet : EnemyPoolData
    {
        public float speed;
        public float addAngle;
        public float removeBulletTime;
        public float fadeTime;
        public bool isBulletEffect;
        public bool isRemainBullet;

        private float angle;
        private bool isStop = false;
        public bool isDelete = false;
        public bool isDamage = false;

        public Vector2 limitMaxPosition;
        public Vector2 limitMinPosition;

        private SpriteRenderer sr;
        private BoxCollider2D col;
        private WaitForSeconds ws;

        private Color currentColor;

        private EnemyController eEnemyController;

        public Vector3 targetDirection;

        public float attackDamage;

        private Enemy enemy;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            EventManager.StartListening("AfterPlayerRespawn", () =>
            {
                RemoveBullet();
            });


            EventManager.StartListening("ExitCurrentMap", () =>
            {
                RemoveBullet();
            });

            EventManager.StartListening("EnemyStart", () =>
            {
                isStop = false;
            });

            EventManager.StartListening("EnemyStop", () =>
            {
                isStop = true;
            });

            ws = new WaitForSeconds(removeBulletTime);
        }

        private void Update()
        {
            if (isStop || isDelete)
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

            isDamage = true;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isDelete || isDamage)
            {
                return;
            }

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(gameObject, Random.Range(attackDamage - 5, attackDamage + 6), transform.position, targetDirection);

                if (enemy != null && enemy != this.enemy)
                {
                    enemy.AttackInit(0, false, false);
                }

                StartBulletEffect();
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                if (enemy != null && enemy != this.enemy)
                {
                    (float, bool) damage;
                    damage.Item1 = Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);

                    enemy.GetDamage(damage.Item1, damage.Item2, false, false, transform.position, targetDirection);

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

                if (isRemainBullet)
                {
                    StartCoroutine(DeleteBullet());
                }
                else
                {
                    StartBulletEffect();
                }
            }
        }

        private IEnumerator DeleteBullet()
        {
            col.enabled = false;
            isDelete = true;

            yield return ws;

            sr.DOFade(0f, fadeTime).OnComplete(() =>
            {
                ResetBullet();
                StartBulletEffect();
            });
        }

        private void ResetBullet()
        {
            col.enabled = true;
            isDelete = false;
            sr.color = currentColor;
        }

        public void RemoveBullet()
        {
            gameObject.SetActive(false);
            StopAllCoroutines();
            ResetBullet();
        }

        public void Init(EnemyController controller, Vector3 direction, float damage, Color color, Enemy enemy = null)
        {
            eEnemyController = controller;
            attackDamage = damage;
            targetDirection = direction;
            this.enemy = enemy;

            isDamage = false;

            angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + addAngle);

            sr.color = color;
            currentColor = color;

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