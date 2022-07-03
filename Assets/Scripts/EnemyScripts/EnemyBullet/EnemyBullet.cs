using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Enemy
{
    public class EnemyBullet : EnemyPoolData
    {
        public float speed;
        public float multipleSpeed;
        public float addAngle;
        public float removeBulletTime;
        public float fadeTime;
        public bool isBulletEffect;
        public bool isRemainBullet;

        private float angle;
        private bool isStop = false;
        public bool isDelete = false;
        public bool isDamage = false;
        public bool isComplete = false;
        public bool isReflection = false;

        public Vector2 limitMaxPosition;
        public Vector2 limitMinPosition;

        private SpriteRenderer sr;
        private BoxCollider2D col;
        private Animator anim;
        private WaitForSeconds ws;

        private Color currentColor;

        private EnemyController eEnemyController;

        public Vector3 targetDirection;

        public float minAttack;
        public float maxAttack;
        public float critical;
        public float criticalPower;

        public Type bulletEffectType;

        private Enemy enemy;

        private int wallLayer = 0;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<BoxCollider2D>();
            anim = GetComponent<Animator>();

            wallLayer = LayerMask.NameToLayer("WALL");
        }

        private void Start()
        {
            ws = new WaitForSeconds(removeBulletTime);
        }

        private void OnEnable()
        {
            EventManager.StartListening("AfterPlayerRespawn", RemoveBullet);
            EventManager.StartListening("ExitCurrentMap", RemoveBullet);
            EventManager.StartListening("EnemyStart", IsStopFalse);
            EventManager.StartListening("EnemyStop", IsStopTrue);

            IsStopFalse();
        }

        private void OnDisable()
        {
            EventManager.StopListening("AfterPlayerRespawn", RemoveBullet);
            EventManager.StopListening("ExitCurrentMap", RemoveBullet);
            EventManager.StopListening("EnemyStart", IsStopFalse);
            EventManager.StopListening("EnemyStop", IsStopTrue);
        }

        private void Update()
        {
            if (isStop || isDelete)
            {
                return;
            }

            transform.position += targetDirection * speed * multipleSpeed * Time.deltaTime;

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
                switch (bulletEffectType)
                {
                    case Type.BulletEffect:
                        EnemyPoolManager.Instance.GetPoolObject(bulletEffectType, transform.position).GetComponent<BulletEffect>().Play(angle);
                        break;
                    case Type.ReflectionBulletEffect:
                        EnemyPoolManager.Instance.GetPoolObject(bulletEffectType, transform.position).transform.rotation = Quaternion.Euler(0f, 0f, angle);
                        break;
                }
            }

            isDamage = true;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isDelete || isDamage || isComplete)
            {
                return;
            }

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                float damage = Random.Range(minAttack, maxAttack + 1);

                if (critical > Random.Range(0, 100))
                {
                    damage = damage + (damage * (criticalPower / 100));
                }

                SlimeGameManager.Instance.Player.GetDamage(gameObject, damage, transform.position, targetDirection);

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

            if (collision.CompareLayer(wallLayer))
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

        public void Init(EnemyController controller, Vector3 direction, float minAttack, float maxAttack, float critical, float criticalPower, Color color, Enemy enemy = null, float multipleSpeed = 1f)
        {
            this.minAttack = minAttack;
            this.maxAttack = maxAttack;
            this.critical = critical;
            this.criticalPower = criticalPower;

            eEnemyController = controller;
            targetDirection = direction;
            this.enemy = enemy;
            this.multipleSpeed = multipleSpeed;

            isDamage = false;
            isComplete = false;
            isReflection = false;

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

        private void IsStopTrue()
        {
            isStop = true;

            if (anim != null)
            {
                anim.speed = 0f;
            }
        }

        private void IsStopFalse()
        {
            isStop = false;

            if (anim != null)
            {
                anim.speed = 1f;
            }
        }
    }
}