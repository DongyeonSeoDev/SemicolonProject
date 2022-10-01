using UnityEngine;

namespace Enemy
{
    public class EnemyBullet2 : MonoBehaviour
    {
        public Animator anim;
        public SpriteRenderer sr;
        
        public float distance;
        public float angleSpeed;

        private Enemy enemy;
        private Enemy6Mage mage;
        private Transform centerTransform;

        private EnemyController eEnemyController;

        private Vector2 moveDirection;

        private float angle;
        private bool isStop;
        private int bulletId;
        private bool isAttack;

        public bool isConnectMage;
        public float minAttack;
        public float maxAttack;
        public float critical;
        public float criticalPower;

        public float currentAngle
        {
            get => angle;
        }

        private void Awake()
        {
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (isStop || centerTransform == null) // || isDelete
            {
                return;
            }

            if (isConnectMage)
            {
                transform.position = centerTransform.position + Quaternion.Euler(0f, 0f, angle) * Vector2.right * distance;

                angle += Time.deltaTime * angleSpeed;
            }
            else
            {
                transform.position = (Vector2)transform.position + moveDirection * 10f * Time.deltaTime;

                if (transform.position.x < -50f || transform.position.x > 50f || transform.position.y < -50f || transform.position.y > 50f)
                {
                    RemoveBullet();
                }
            }
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isAttack)
            {
                return;
            }

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                (float, bool) damage;

                damage.Item1 = Random.Range(minAttack, maxAttack + 1);
                damage.Item2 = critical > Random.Range(0, 100);

                if (damage.Item2)
                {
                    damage.Item1 = damage.Item1 + (damage.Item1 * (criticalPower / 100));
                }

                SlimeGameManager.Instance.Player.GetDamage(gameObject, damage.Item1, transform.position, (SlimeGameManager.Instance.CurrentPlayerBody.transform.position - transform.position).normalized, critical: damage.Item2);

                if (enemy != null && enemy != this.enemy)
                {
                    enemy.AttackInit(0, false, false);
                }

                // StartBulletEffect();

                RemoveBullet();
                isAttack = true;
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                if (enemy != null && enemy != this.enemy)
                {
                    (float, bool) damage;
                    damage.Item1 = Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);

                    enemy.GetDamage(damage.Item1, damage.Item2, true, false, transform.position, (enemy.transform.position - transform.position).normalized, 5f);

                    // StartBulletEffect();
                    RemoveBullet();
                    isAttack = true;

                    EventManager.TriggerEvent("OnEnemyAttack");
                }
            }
        }

        public void Init(EnemyController controller, float minAttack, float maxAttack, float critical, float criticalPower, Color color, Transform centerTransform, float startAngle, Enemy6Mage mage, int bulletId, Enemy enemy = null)
        {
            this.minAttack = minAttack;
            this.maxAttack = maxAttack;
            this.critical = critical;
            this.criticalPower = criticalPower;
            this.centerTransform = centerTransform;

            eEnemyController = controller;
            angle = startAngle;
            this.enemy = enemy;
            this.mage = mage;
            this.bulletId = bulletId;

            isAttack = false;
            isConnectMage = true;

            moveDirection = Vector2.zero;

            // 반사 구현 안됨
            // isDamage = false;
            // isComplete = false;
            // isReflection = false;

            sr.color = color;

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

        public void RemoveBullet()
        {
            if (isConnectMage)
            {
                DisconnectMage();
            }
            
            gameObject.SetActive(false);
            
            //StopAllCoroutines();
            //ResetBullet();
        }

        private void DisconnectMage()
        {
            isConnectMage = false;

            mage.isUseAngles[bulletId] = false;
            mage.bullets.Remove(this);
        }

        public void Fire(Vector2 direction)
        {
            DisconnectMage();

            moveDirection = direction;
        }

        //private void ResetBullet()
        //{
        //    col.enabled = true;
        //    isDelete = false;
        //    sr.color = currentColor;
        //}

        //private void StartBulletEffect()
        //{
        //    if (isBulletEffect)
        //    {
        //        switch (bulletEffectType)
        //        {
        //            case Type.BulletEffect:
        //                EnemyPoolManager.Instance.GetPoolObject(bulletEffectType, transform.position).GetComponent<BulletEffect>().Play(angle);
        //                break;
        //            case Type.ReflectionBulletEffect:
        //                EnemyPoolManager.Instance.GetPoolObject(bulletEffectType, transform.position).transform.rotation = Quaternion.Euler(0f, 0f, angle);
        //                break;
        //        }
        //    }

        //    isDamage = true;
        //    gameObject.SetActive(false);
        //}

        //private IEnumerator DeleteBullet()
        //{
        //    col.enabled = false;
        //    isDelete = true;

        //    yield return ws;

        //    sr.DOFade(0f, fadeTime).OnComplete(() =>
        //    {
        //        ResetBullet();
        //        StartBulletEffect();
        //    });
        //}
    }
}