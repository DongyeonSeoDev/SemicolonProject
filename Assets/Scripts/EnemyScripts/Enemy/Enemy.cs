using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Enemy : PoolManager // �� ���� Ŭ����
    {
        public EnemyMoveSO enemyMoveSO;
        public EnemyLootListSO enemyLootListSO;
        public Image hpBarFillImage;

        protected EnemyData enemyData;
        protected SpriteRenderer sr;
        protected Animator anim;
        protected Rigidbody2D rb;

        private EnemyState currentState;

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

        private void Update()
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
            return enemyData.enemyId;
        }

        public void MoveEnemy()
        {
            enemyData.isEnemyMove = true;
        }

        public float EnemyHpPercent()
        {
            return ((float)enemyData.hp / enemyData.maxHP) * 100f;
        }
    }
}