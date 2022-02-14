using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Enemy : PoolManager // 적 관리 클래스
    {
        public EnemyMoveSO enemyMoveSO;
        public EnemyLootListSO enemyLootListSO;
        public Image hpBarFillImage;
        public GameObject hpBar;

        protected EnemyData enemyData;
        protected SpriteRenderer sr;
        protected Animator anim;
        protected Rigidbody2D rb;

        private EnemyState currentState;
        private PlayerInput playerInput;

        public EnemyAttackCheck enemyAttackCheck;

        private float lastPositionX;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            EventManager.StartListening("PlayerDead", () =>
            {
                currentState = null;
                enemyData.enemyAnimator.enabled = false;
            });

            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
        }

        protected virtual void OnEnable()
        {
            enemyData.enemyAnimator.enabled = true;
            currentState = new EnemyIdleState(enemyData);
            lastPositionX = transform.position.x + Mathf.Infinity;

            enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;
            enemyData.enemyAnimator.SetTrigger(enemyData.hashReset);
            enemyData.enemyAnimator.SetBool(enemyData.hashIsDead, false);
        }

        protected virtual void Update()
        {
            if (currentState != null)
            {
                currentState = currentState.Process();
            }

            if (enemyData.isRotate)
            {
                if (lastPositionX > transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (lastPositionX < transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else
            {
                if (lastPositionX > transform.position.x)
                {
                    sr.flipX = true;
                }
                else if (lastPositionX < transform.position.x)
                {
                    sr.flipX = false;
                }
            }

            if (enemyData.eEnemyController == EnemyController.PLAYER && playerInput.IsShoot)
            {
                playerInput.IsShoot = false;
                enemyData.isAttack = true;
            }

            lastPositionX = transform.position.x;
        }

        public void GetDamage(int damage)
        {
            if (!enemyData.isDamaged)
            {
                enemyData.isDamaged = true;
                enemyData.damagedValue = damage;
            }
        }

        public void EnemyDestroy()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();
        }

        public string GetEnemyId()
        {
            return enemyData.enemyType.ToString();
        }

        public void MoveEnemy()
        {
            enemyData.isEnemyMove = true;
        }

        public float EnemyHpPercent()
        {
            return ((float)enemyData.hp / enemyData.maxHP) * 100f;
        }

        public void EnemyControllerChange(EnemyController eEnemyController)
        {
            enemyData.eEnemyController = eEnemyController;

            if (eEnemyController == EnemyController.AI)
            {
                gameObject.tag = "Untagged";
                gameObject.layer = LayerMask.NameToLayer("ENEMY");

                hpBar.SetActive(true);

                sr.color = enemyData.normalColor;

                EnemyManager.Instance.player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                gameObject.tag = "Player";
                gameObject.layer = LayerMask.NameToLayer("PLAYER");

                hpBar.SetActive(false);

                sr.color = enemyData.playerNormalColor;

                EnemyManager.Instance.player = transform;
            }

            if (enemyAttackCheck != null)
            {
                if (enemyAttackCheck.enemyControllerChange != null)
                {
                    enemyAttackCheck.enemyControllerChange(eEnemyController);
                }
                else
                {
                    enemyAttackCheck.AddEnemyController();
                    enemyAttackCheck.enemyControllerChange(eEnemyController);
                }
            }
        }
    }
}