using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Enemy
{
    [RequireComponent(typeof(OrderInLayerConroller))] // 적 layer 관리
    public class Enemy : EnemyPoolData, ICanGetDamagableEnemy // 적 관리 클래스
    {
        public List<EnemyLootData> enemyLootListSO = new List<EnemyLootData>(); // 적 전리품 리스트
        public EnemyDataSO enemyDataSO; // 적 데이터 관리 ( 없으면 Scriptable Object에서 만들어야 함 )
        public Image hpBarFillImage; // 적 HP 바 채워진것중 체력 확인용
        public Image hpBarDamageFillImage; // 적 HP 바 채워진것중 데미지 확인용
        public GameObject hpBar; // 적 HP 바 오브젝트 ( hpBarFillImage의 부모 캔버스 오브젝트 )

        protected EnemyData enemyData; // 적 데이터

        // GetComponent로 가져옴
        protected SpriteRenderer sr; 
        protected Animator anim;
        protected Rigidbody2D rb;

        // SlimeGameManager에서 가져옴
        protected PlayerInput playerInput;

        private EnemyState currentState; // Enemy FSM 실행

        public EnemyAttackCheck[] enemyAttackCheck; // 적 공격 확인 ( 근거리 적은 있고 원거리 적은 없음 )
        public EnemyPositionCheckData positionCheckData = new EnemyPositionCheckData(); // 벽과 적 위치 확인

        private Tween hpTween = null;
        private Tween damageHPTween = null;

        public float hpTweenTime = 0.1f;
        public float hpTweenDelayTime = 0.3f;
        public float damageHPTweenTime = 0.2f;

        EnemyCommand enemyDamagedCommand;
        EnemyCommand enemyKnockBackCommand;

        private float isDamageCurrentTime = 0f;
        protected bool isStop = false;

        private bool isUsePlayerSpeedEvent = false;

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

            // CSV GetData
            CSVEnemyLoot.Instance.GetData();

            if (CSVEnemyLoot.Instance.lootData.Count > (int)enemyDataSO.enemyType)
            {
                enemyLootListSO = CSVEnemyLoot.Instance.lootData[(int)enemyDataSO.enemyType]; // 적 전리품 받아오기 ( 없으면 엑셀에서 만들어야 함 )
            }
        }

        protected virtual void OnDisable() // 오브젝트 제거시 이벤트 제거
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
            }
        }

        private void EnemyStop()
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                isStop = true;
                anim.speed = 0f;
            }
        }

        private void EnemyDataReset() // (이벤트 용) 적 리셋
        {
            currentState = null;
            enemyData.enemyAnimator.enabled = false;
        }

        private void StartAttack() // (이벤트 용) 공격 시작했을때
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

            enemyData.enemyAnimator.enabled = true; // 애니메이션 실행
            isDamageCurrentTime = 0f;

            enemyDamagedCommand = new EnemyGetDamagedCommand(enemyData);
            enemyKnockBackCommand = new EnemyAddForceCommand(enemyData.enemyRigidbody2D, this);

            SetHP(false);

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Reset, anim, TriggerType.SetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.IsDead, anim, false);

            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMY"); // layer 리셋

            // 첫 상태 넣기
            currentState = new EnemyIdleState(enemyData);

            // 색깔 리셋
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyData.enemy.ChangeColor(enemyData.normalColor);
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyData.enemy.ChangeColor(enemyData.playerNormalColor);
            }

            // 이벤트 추가
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

                enemyDamagedCommand.Execute();

                if (!enemyData.isNoKnockback && enemyData.isKnockBack)
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
            if (collision.gameObject.CompareTag("Wall")) // 벽 확인
            {
                positionCheckData.isWall = true;
                positionCheckData.oppositeDirectionWall = collision.contacts[0].normal;
            }
        }

        // 적 데미지 받는 코드
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
                enemyData.knockBackPower = knockBackPower;

                enemyData.stunTime = isStun ? stunTime : 0;

                enemyData.knockBackDirection = direction;

                return true;
            }

            return false;
        }

        // 적이 죽었을때 발동되는 코드
        public void EnemyDestroy()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString()); // MonsterCollection부분의 코드가 이벤트의 매개변수 갯수 변동으로
                                                                                                // 인해 작동하지 않을 것을 대비한 임시방편 머지 후에 수정할것

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString(), false); // 3번째 매개변수는 흡수로인해 죽은것인가를 보내줌
        }

        // 적이 Drain되었을 때 발동하는 코드
        public void EnemyDrain()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString()); // MonsterCollection부분의 코드가 이벤트의 매개변수 갯수 변동으로
                                                                                                // 인해 작동하지 않을 것을 대비한 임시방편 머지 후에 수정할것

            EventManager.TriggerEvent("EnemyDead", gameObject, enemyData.enemyType.ToString(), true);
        }

        // 적 컨트롤러를 다른것으로 바꿈
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
            enemyData.isEnemyMove = true; // 적을 움직이는 상태로 바꿈
        }

        public virtual void ChangeColor(Color color)
        {
            sr.color = color;
        }

        private void OnAttackSpeedChage() => SlimeGameManager.Instance.SetSkillDelay(0, enemyData.playerAnimationDelay + enemyData.playerAnimationTime);

        public EnemyController GetEnemyController() => enemyData.eEnemyController;
        public Vector2? GetKnockBackDirection() => enemyData.knockBackDirection; // 적이 넉백 공격을 할 수 있는지를 가져옴
        public Transform GetTransform() => transform;
        public GameObject GetGameObject() => gameObject;
        public string GetEnemyId() => enemyData.enemyType.ToString(); // 적 아이디를 가져옴
        public float EnemyHpPercent() => ((float)enemyData.hp / enemyData.maxHP) * 100f; // 적 체력 퍼센트를 가져옴
        public float GetKnockBackPower() => enemyData.knockBackPower; // 적이 넉백 공격을 할 수 있는지를 가져옴
        public float GetEnemyAttackPower() => enemyData.attackPower; // 적 공격력을 가져옴
        public bool GetIsKnockBack() => enemyData.isUseKnockBack; // 적이 넉백 공격을 할 수 있는지를 가져옴
        public bool GetIsParrying() => enemyData.isParrying;
    }
}