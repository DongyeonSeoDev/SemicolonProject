using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    [RequireComponent(typeof(OrderInLayerConroller))] // �� layer ����
    public class Enemy : EnemyPoolData // �� ���� Ŭ����
    {
        public List<EnemyLootData> enemyLootListSO = new List<EnemyLootData>(); // �� ����ǰ ����Ʈ
        public EnemyDataSO enemyDataSO; // �� ������ ���� ( ������ Scriptable Object���� ������ �� )
        public Image hpBarFillImage; // �� HP �� ä������ ( ������ UI ������ �� ( Assets > Prefabs > EnemyPrefabs > EnemyUI ���� ) )
        public GameObject hpBar; // �� HP �� ������Ʈ ( hpBarFillImage�� �θ� ĵ���� ������Ʈ )

        protected EnemyData enemyData; // �� ������ 

        // GetComponent�� ������
        protected SpriteRenderer sr; 
        protected Animator anim;
        protected Rigidbody2D rb;

        // SlimeGameManager���� ������
        protected PlayerInput playerInput;

        private EnemyState currentState; // Enemy FSM ����

        public EnemyAttackCheck enemyAttackCheck; // �� ���� Ȯ�� ( �ٰŸ� ���� �ְ� ���Ÿ� ���� ���� )
        public EnemyPositionCheckData positionCheckData = new EnemyPositionCheckData(); // ���� �� ��ġ Ȯ��

        EnemyCommand enemyDamagedCommand;
        EnemyCommand enemyKnockBackCommand;

        private float isDamageCurrentTime = 0f;

        private void Awake()
        {
            // GetComponent
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            // CSV GetData
            CSVEnemyLoot.Instance.GetData();

            if (CSVEnemyLoot.Instance.lootData.Count > (int)enemyDataSO.enemyType)
            {
                enemyLootListSO = CSVEnemyLoot.Instance.lootData[(int)enemyDataSO.enemyType]; // �� ����ǰ �޾ƿ��� ( ������ �������� ������ �� )
            }
        }

        protected virtual void Start()
        {
            // �̺�Ʈ �߰�

            EventManager.StartListening("PlayerDead", EnemyDataReset);

            if(enemyData.eEnemyController == EnemyController.PLAYER)
            {
                EventManager.StartListening("StartSkill0", StartAttack);
            }

            // SlimeGameManager

            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
        }

        private void OnDestroy() // ������Ʈ ���Ž� �̺�Ʈ ����
        {
            EventManager.StopListening("PlayerDead", EnemyDataReset);
            EventManager.StopListening("StartSkill0", StartAttack);
        }

        private void EnemyDataReset() // (�̺�Ʈ ��) �� ����
        {
            currentState = null;
            enemyData.enemyAnimator.enabled = false;
        }

        private void StartAttack() // (�̺�Ʈ ��) ���� ����������
        {
            enemyData.isAttack = true;
            playerInput.AttackMousePosition = playerInput.MousePosition;
        }

        protected virtual void OnEnable()
        {
            enemyData = new EnemyData(enemyDataSO)
            {
                enemyObject = gameObject,
                enemyLootList = enemyLootListSO,
                enemyAnimator = anim,
                enemySpriteRenderer = sr,
                enemyRigidbody2D = rb,
                hpBarFillImage = hpBarFillImage,
            };

            enemyData.enemyAnimator.enabled = true; // �ִϸ��̼� ����
            isDamageCurrentTime = 0f;

            enemyDamagedCommand = new EnemyGetDamagedCommand(enemyData);
            enemyKnockBackCommand = new EnemyAddForceCommand(enemyData.enemyRigidbody2D, this);

            // ������ ��ġ, HP UI, �ִϸ��̼�, ���� Ȯ�� ����
            if (enemyData.hpBarFillImage != null)
            {
                enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;
            }

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Reset, anim, TriggerType.SetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.IsDead, anim, false);

            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMY"); // layer ����

            // ù ���� �ֱ�
            currentState = new EnemyIdleState(enemyData);

            // ���� ����
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                sr.color = enemyData.normalColor;
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                sr.color = enemyData.playerNormalColor;
            }
        }

        protected virtual void Update()
        {
            // Enemy FSM
            if (currentState != null)
            {
                currentState = currentState.Process();
            }

            if (enemyData.isDamaged)
            {
                isDamageCurrentTime = enemyData.damageDelay;
                enemyData.hp -= enemyData.damagedValue;

                if (hpBarFillImage != null)
                {
                    hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;
                }

                enemyDamagedCommand.Execute();

                if (enemyData.isKnockBack)
                {
                    enemyKnockBackCommand.Execute();

                    enemyData.isKnockBack = false;
                }

                enemyData.isDamaged = false;
            }

            if (isDamageCurrentTime > 0f)
            {
                isDamageCurrentTime -= Time.deltaTime;

                if (isDamageCurrentTime <= 0)
                {
                    enemyDamagedCommand.Execute();
                    isDamageCurrentTime = 0;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Wall")) // �� Ȯ��
            {
                positionCheckData.isWall = true;
                positionCheckData.oppositeDirectionWall = collision.contacts[0].normal;
            }
        }

        // �� ������ �޴� �ڵ�
        public void GetDamage(int damage, bool critical = false, bool isKnockBack = false, float knockBackPower = 20f, float stunTime = 1f, Vector2? direction = null)
        {
            if (enemyData.isEnemyMove && !enemyData.isDamaged)
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

        // ���� �׾����� �ߵ��Ǵ� �ڵ�
        public void EnemyDestroy()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();

            EventManager.TriggerEvent("EnemyDead", enemyData.enemyType.ToString());
        }

        // �� ��Ʈ�ѷ��� �ٸ������� �ٲ�
        public void EnemyControllerChange(EnemyController eEnemyController)
        {
            enemyData.eEnemyController = eEnemyController;

            if (eEnemyController == EnemyController.AI)
            {
                gameObject.tag = "Untagged";
                gameObject.layer = LayerMask.NameToLayer("ENEMY");

                if (hpBar != null)
                {
                    hpBar.SetActive(true);
                }

                sr.color = enemyData.normalColor;

                EnemyManager.Player = GameObject.FindGameObjectWithTag("Player");
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                gameObject.tag = "Player";
                gameObject.layer = LayerMask.NameToLayer("PLAYER");

                if (hpBar != null)
                {
                    hpBar.SetActive(false);
                }

                sr.color = enemyData.playerNormalColor;

                EnemyManager.Player = gameObject;
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

        public void MoveEnemy() => enemyData.isEnemyMove = true; // ���� �����̴� ���·� �ٲ�

        public EnemyType GetEnemyType() => enemyData.enemyType; // �� Ÿ���� ������
        public EnemyController GetEnemyController() => enemyData.eEnemyController;
        public Vector2? GetKnockBackDirection() => enemyData.knockBackDirection; // ���� �˹� ������ �� �� �ִ����� ������
        public string GetEnemyId() => enemyData.enemyType.ToString(); // �� ���̵� ������
        public float EnemyHpPercent() => ((float)enemyData.hp / enemyData.maxHP) * 100f; // �� ü�� �ۼ�Ʈ�� ������
        public float GetKnockBackPower() => enemyData.knockBackPower; // ���� �˹� ������ �� �� �ִ����� ������
        public int GetEnemyAttackPower() => enemyData.attackPower; // �� ���ݷ��� ������
        public bool GetIsKnockBack() => enemyData.isUseKnockBack; // ���� �˹� ������ �� �� �ִ����� ������
    }
}