using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace Enemy
{
    [RequireComponent(typeof(OrderInLayerController))] // �� layer ����
    public class Enemy : EnemyPoolData, ICanGetDamagableEnemy // �� ���� Ŭ����
    {
        public List<EnemyLootData> enemyLootListSO = new List<EnemyLootData>(); // �� ����ǰ ����Ʈ
        public EnemyDataSO enemyDataSO; // �� ������ ���� ( ������ Scriptable Object���� ������ �� )
        public Image hpBarFillImage; // �� HP �� ä�������� ü�� Ȯ�ο�
        public Image hpBarDamageFillImage; // �� HP �� ä�������� ������ Ȯ�ο�
        public GameObject hpBar; // �� HP �� ������Ʈ ( hpBarFillImage�� �θ� ĵ���� ������Ʈ )

        protected EnemyData enemyData; // �� ������

        // GetComponent�� ������
        protected SpriteRenderer sr; 
        protected Animator anim;
        protected Rigidbody2D rb;

        // SlimeGameManager���� ������
        protected PlayerInput playerInput;

        private EnemyState currentState; // Enemy FSM ����

        public EnemyAttackCheck[] enemyAttackCheck; // �� ���� Ȯ�� ( �ٰŸ� ���� �ְ� ���Ÿ� ���� ���� )
        public EnemyPositionCheckData positionCheckData = new EnemyPositionCheckData(); // ���� �� ��ġ Ȯ��

        private Tween hpTween = null;
        private Tween damageHPTween = null;

        public float hpTweenTime = 0.1f;
        public float hpTweenDelayTime = 0.3f;
        public float damageHPTweenTime = 0.2f;

        [SerializeField]
        private Animator hpEffectAnimation = null;

        EnemyCommand enemyDamagedCommand;

        private float isDamageCurrentTime = 0f;
        protected bool isStop = false;

        private bool isUsePlayerSpeedEvent = false;

        private WaitForSeconds moveDelay;
        private Coroutine currentCoroutine;

        private bool isHpEffectAnimationPlay = false;

        private Vector2 knockBackDirection;

        [SerializeField]
        private float addExperience;

        public float AddExperience
        { 
            get
            {
                return addExperience;
            }
        }

        protected virtual void Awake()
        {
            // GetComponent
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            moveDelay = new WaitForSeconds(0.2f);

            // CSV GetData
            CSVEnemyLoot.Instance.GetData();

            if (CSVEnemyLoot.Instance.lootData.Count > (int)enemyDataSO.enemyType)
            {
                enemyLootListSO = CSVEnemyLoot.Instance.lootData[(int)enemyDataSO.enemyType]; // �� ����ǰ �޾ƿ��� ( ������ �������� ������ �� )
            }
        }

        protected virtual void OnDisable() // ������Ʈ ���Ž� �̺�Ʈ ����
        {
            EventManager.StopListening("PlayerDead", EnemyDataReset);
            EventManager.StopListening("EnemyStart", EnemyStart);
            EventManager.StopListening("EnemyStop", EnemyStop);
            EventManager.StopListening("StartSkill0", StartAttack);

            if (isUsePlayerSpeedEvent)
            {
                isUsePlayerSpeedEvent = false;

                EventManager.StopListening("OnAttackSpeedChage", OnAttackSpeedChage);
            }
        }

        private void EnemyStart()
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                isStop = false;
                anim.speed = 1f;

                if (hpEffectAnimation != null)
                {
                    hpEffectAnimation.speed = 1f;
                }
            }
        }

        private void EnemyStop()
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                isStop = true;
                anim.speed = 0f;

                if (hpEffectAnimation != null)
                {
                    hpEffectAnimation.speed = 0f;
                }
            }
        }

        private void EnemyDataReset() // (�̺�Ʈ ��) �� ����
        {
            currentState = null;
            enemyData.enemyAnimator.enabled = false;

            ChangeHpEffectAnimationPlay(false);
        }

        private void StartAttack() // (�̺�Ʈ ��) ���� ����������
        {
            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyData.isAttack = true;
                playerInput.AttackMousePosition = playerInput.MousePosition;
            }
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
                enemy = this
            };

            enemyData.enemyAnimator.enabled = true; // �ִϸ��̼� ����
            isDamageCurrentTime = 0f;

            enemyDamagedCommand = new EnemyGetDamagedCommand(enemyData);

            SetHP(false);

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Reset, anim, TriggerType.SetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.IsDead, anim, false);

            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMY"); // layer ����

            // ù ���� �ֱ�
            currentState = new EnemyIdleState(enemyData);

            // ���� ����
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyData.enemy.ChangeColor(enemyData.normalColor);
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyData.enemy.ChangeColor(enemyData.playerNormalColor);
            }

            // �̺�Ʈ �߰�
            EventManager.StartListening("PlayerDead", EnemyDataReset);
            EventManager.StartListening("EnemyStart", EnemyStart);
            EventManager.StartListening("EnemyStop", EnemyStop);
            EventManager.StartListening("StartSkill0", StartAttack);

            EnemyStart();

            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();

            if (hpBar != null)
            {
                enemyData.enemyCanvas = hpBar;
            }

            ChangeHpEffectAnimationPlay(false);
        }

        protected virtual void Update()
        {
            if (isStop)
            {
                return;
            }

            // Enemy FSM
            if (currentState != null)
            {
                currentState = currentState.Process();
            }

            if (enemyData.isDamaged)
            {
                isDamageCurrentTime = enemyData.damageDelay;
                enemyData.hp -= enemyData.damagedValue;

                SetHP(true);

                if (EnemyHpPercent() > 0 && EnemyHpPercent() <= EnemyManager.CanDrainPercent())
                {
                    ChangeHpEffectAnimationPlay(true);
                }
                else
                {
                    ChangeHpEffectAnimationPlay(false);
                }

                enemyDamagedCommand.Execute();

                if (!enemyData.isNoKnockback && enemyData.isKnockBack)
                {
                    EnemyKnockBack();
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

            var enemyCheck = collision.gameObject.GetComponent<Enemy>();

            if (enemyCheck != null)
            {
                enemyData.movePosition = collision.contacts[0].normal;
                currentCoroutine = StartCoroutine(ResetMove());

                enemyCheck.EnemyMoveReset();
            }
        }

        private IEnumerator ResetMove()
        {
            yield return moveDelay;

            enemyData.movePosition = null;
        }

        // �� ������ �޴� �ڵ�
        public void GetDamage(float damage, bool critical, bool isKnockBack, bool isStun, Vector2 effectPosition, Vector2 direction, float knockBackPower = 20f, float stunTime = 1f, Vector3? effectSize = null)
        {
            if (AttackInit(damage, isKnockBack, isStun, direction, knockBackPower, stunTime))
            {
                EffectManager.Instance.OnDamaged(damage, critical, true, transform.position, effectPosition, direction, effectSize);
            }
        }

        public bool AttackInit(float damage, bool isKnockBack, bool isStun, Vector2? direction = null, float knockBackPower = 20f, float stunTime = 1f)
        {
            if (enemyData.isEnemyMove && !enemyData.isDamaged)
            {
                enemyData.isDamaged = true;
                enemyData.damagedValue = damage;
                enemyData.isKnockBack = isKnockBack;
                enemyData.stunTime = isStun ? stunTime : 0;

                knockBackDirection = (direction == null ? Vector2.zero : direction.Value.normalized) * knockBackPower;

                return true;
            }

            return false;
        }

        private void EnemyKnockBack()
        {
            rb.AddForce(knockBackDirection, ForceMode2D.Impulse);
        }

        // ���� �׾����� �ߵ��Ǵ� �ڵ�
        public void EnemyDestroy()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString()); // MonsterCollection�κ��� �ڵ尡 �̺�Ʈ�� �Ű����� ���� ��������
                                                                                                // ���� �۵����� ���� ���� ����� �ӽù��� ���� �Ŀ� �����Ұ�

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString(), false); // 3��° �Ű������� ��������� �������ΰ��� ������
        }

        // ���� Drain�Ǿ��� �� �ߵ��ϴ� �ڵ�
        public void EnemyDrain()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString()); // MonsterCollection�κ��� �ڵ尡 �̺�Ʈ�� �Ű����� ���� ��������
                                                                                                // ���� �۵����� ���� ���� ����� �ӽù��� ���� �Ŀ� �����Ұ�

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString(), true);
        }

        // �� ��Ʈ�ѷ��� �ٸ������� �ٲ�
        public void ChangeToPlayerController()
        {
            enemyData.eEnemyController = EnemyController.PLAYER;

            gameObject.tag = "Player";
            gameObject.layer = LayerMask.NameToLayer("PLAYER");

            if (hpBar != null)
            {
                hpBar.SetActive(false);
            }

            enemyData.enemy.ChangeColor(enemyData.playerNormalColor);

            MoveEnemy();

            if (enemyAttackCheck != null)
            {
                for (int i = 0; i < enemyAttackCheck.Length; i++)
                {
                    if (enemyAttackCheck[i].enemyControllerChange != null)
                    {
                        enemyAttackCheck[i].enemyControllerChange(EnemyController.PLAYER);
                    }
                    else
                    {
                        enemyAttackCheck[i].AddEnemyController();
                        enemyAttackCheck[i].enemyControllerChange(EnemyController.PLAYER);
                    }
                }
            }

            isUsePlayerSpeedEvent = true;

            EventManager.StartListening("OnAttackSpeedChage", OnAttackSpeedChage);
        }

        protected virtual void SetHP(bool isUseTween)
        {
            float fillValue = (float)enemyData.hp / enemyData.maxHP;

            if (isUseTween)
            {
                if (hpBarFillImage != null)
                {
                    if (hpTween.IsActive())
                    {
                        hpTween.Kill();
                    }

                    hpTween = hpBarFillImage.DOFillAmount(fillValue, hpTweenTime);
                }

                if (hpBarDamageFillImage != null)
                {
                    Util.DelayFunc(() =>
                    {
                        if (damageHPTween.IsActive())
                        {
                            damageHPTween.Kill();
                        }

                        damageHPTween = hpBarDamageFillImage.DOFillAmount(fillValue, damageHPTweenTime);
                    }, hpTweenDelayTime);
                }
            }
            else
            {
                hpBarFillImage.fillAmount = fillValue;
                hpBarDamageFillImage.fillAmount = fillValue;
            }
        }

        public virtual void SetColor(float time)
        {
            Color currentColor = sr.color;
            Color changeColor = currentColor;

            changeColor.a = 0f;
            sr.color = changeColor;

            sr.DOColor(currentColor, time);
        }

        public virtual void MoveEnemy()
        {
            enemyData.isEnemyMove = true; // ���� �����̴� ���·� �ٲ�
        }

        public virtual void ChangeColor(Color color)
        {
            sr.color = color;
        }

        private void OnAttackSpeedChage()
        {
            SlimeGameManager.Instance.SetSkillDelay(0, enemyData.playerAnimationDelay);
            SlimeGameManager.Instance.AddSkillDelay(0, enemyData.playerAnimationTime);
        }

        public void EnemyMoveReset()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);

                currentCoroutine = null;
            }
            
            enemyData.movePosition = null;
        }

        private void ChangeHpEffectAnimationPlay(bool isActive)
        {
            if (hpEffectAnimation != null && isHpEffectAnimationPlay != isActive)
            {
                isHpEffectAnimationPlay = isActive;
                hpEffectAnimation.SetBool(EnemyManager.Instance.hashIsStart, isActive);
            }
        }
            
        public EnemyController GetEnemyController() => enemyData.eEnemyController;
        public Transform GetTransform() => transform;
        public GameObject GetGameObject() => gameObject;
        public string GetEnemyId() => enemyData.enemyType.ToString(); // �� ���̵� ������
        public float EnemyHpPercent() => ((float)enemyData.hp / enemyData.maxHP) * 100f; // �� ü�� �ۼ�Ʈ�� ������
        public float GetEnemyAttackPower() => enemyData.attackPower; // �� ���ݷ��� ������
        public bool GetIsKnockBack() => enemyData.isUseKnockBack; // ���� �˹� ������ �� �� �ִ����� ������
        public bool GetIsParrying() => enemyData.isParrying;
    }
}