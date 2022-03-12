using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Enemy : EnemyPoolData // 적 관리 클래스
    {
        public List<EnemyLootData> enemyLootListSO;
        public EnemyDataSO enemyDataSO;
        public Image hpBarFillImage;
        public GameObject hpBar;

        protected EnemyData enemyData;
        protected SpriteRenderer sr;
        protected Animator anim;
        protected Rigidbody2D rb;

        private EnemyState currentState;
        protected PlayerInput playerInput;

        public EnemyAttackCheck enemyAttackCheck;

        private float lastPositionX;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            CSVEnemyLoot.Instance.GetData("");
            enemyLootListSO = CSVEnemyLoot.Instance.lootData[(int)enemyDataSO.enemyType];
        }

        private void Start()
        {
            EventManager.StartListening("PlayerDead", EnemyDataReset);

            if(enemyData.eEnemyController == EnemyController.PLAYER)
            {
                EventManager.StartListening("StartSkill0", () => enemyData.isAttack = true);
            }

            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
        }

        private void OnDestroy()
        {
            EventManager.StopListening("PlayerDead", EnemyDataReset);
        }

        private void EnemyDataReset()
        {
            currentState = null;
            enemyData.enemyAnimator.enabled = false;
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

            if (!enemyData.isUseAttacking || !enemyData.isAttacking)
            {
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
            }

            lastPositionX = transform.position.x;
        }

        public void GetDamage(int damage, bool critical = false, bool isKnockBack = false, float knockBackPower = 20f, float stunTime = 1f, Vector2? direction = null)
        {
            if (!enemyData.isDamaged)
            {
                enemyData.isDamaged = true;
                enemyData.damagedValue = damage;

                enemyData.isKnockBack = isKnockBack;
                enemyData.knockBackPower = knockBackPower;
                enemyData.stunTime = stunTime;

                enemyData.knockBackDirection = direction;

                EffectManager.Instance.OnDamaged(damage, critical, true, transform.position);
            }
        }

        public void EnemyDestroy()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();

            EventManager.TriggerEvent("EnemyDead", enemyData.enemyType.ToString());
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