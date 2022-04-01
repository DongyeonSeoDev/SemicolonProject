using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    [RequireComponent(typeof(OrderInLayerConroller))] // 적 layer 관리
    public class Enemy : EnemyPoolData // 적 관리 클래스
    {
        public List<EnemyLootData> enemyLootListSO = new List<EnemyLootData>(); // 적 전리품 리스트
        public EnemyDataSO enemyDataSO; // 적 데이터 관리 ( 없으면 Scriptable Object에서 만들어야 함 )
        public Image hpBarFillImage; // 적 HP 바 채워진것 ( 없으면 UI 만들어야 함 ( Assets > Prefabs > EnemyPrefabs > EnemyUI 참고 ) )
        public GameObject hpBar; // 적 HP 바 오브젝트 ( hpBarFillImage의 부모 캔버스 오브젝트 )

        protected EnemyData enemyData; // 적 데이터 

        // GetComponent로 가져옴
        protected SpriteRenderer sr; 
        protected Animator anim;
        protected Rigidbody2D rb;

        // SlimeGameManager에서 가져옴
        protected PlayerInput playerInput;

        private EnemyState currentState; // Enemy FSM 실행

        public EnemyAttackCheck enemyAttackCheck; // 적 공격 확인 ( 근거리 적은 있고 원거리 적은 없음 )
        public EnemyPositionCheckData positionCheckData = new EnemyPositionCheckData(); // 벽과 적 위치 확인

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
                enemyLootListSO = CSVEnemyLoot.Instance.lootData[(int)enemyDataSO.enemyType]; // 적 전리품 받아오기 ( 없으면 엑셀에서 만들어야 함 )
            }
        }

        protected virtual void Start()
        {
            // 이벤트 추가

            EventManager.StartListening("PlayerDead", EnemyDataReset);

            if(enemyData.eEnemyController == EnemyController.PLAYER)
            {
                EventManager.StartListening("StartSkill0", StartAttack);
            }

            // SlimeGameManager

            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
        }

        private void OnDestroy() // 오브젝트 제거시 이벤트 제거
        {
            EventManager.StopListening("PlayerDead", EnemyDataReset);
            EventManager.StopListening("StartSkill0", StartAttack);
        }

        private void EnemyDataReset() // (이벤트 용) 적 리셋
        {
            currentState = null;
            enemyData.enemyAnimator.enabled = false;
        }

        private void StartAttack() // (이벤트 용) 공격 시작했을때
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

            enemyData.enemyAnimator.enabled = true; // 애니메이션 실행
            isDamageCurrentTime = 0f;

            enemyDamagedCommand = new EnemyGetDamagedCommand(enemyData);
            enemyKnockBackCommand = new EnemyAddForceCommand(enemyData.enemyRigidbody2D, this);

            // 마지막 위치, HP UI, 애니메이션, 죽음 확인 리셋
            if (enemyData.hpBarFillImage != null)
            {
                enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;
            }

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Reset, anim, TriggerType.SetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.IsDead, anim, false);

            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMY"); // layer 리셋

            // 첫 상태 넣기
            currentState = new EnemyIdleState(enemyData);

            // 색깔 리셋
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
            if (collision.gameObject.CompareTag("Wall")) // 벽 확인
            {
                positionCheckData.isWall = true;
                positionCheckData.oppositeDirectionWall = collision.contacts[0].normal;
            }
        }

        // 적 데미지 받는 코드
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

        // 적이 죽었을때 발동되는 코드
        public void EnemyDestroy()
        {
            gameObject.SetActive(false);

            EnemyManager.Instance.EnemyDestroy();

            EventManager.TriggerEvent("EnemyDead", enemyData.enemyType.ToString());
        }

        // 적 컨트롤러를 다른것으로 바꿈
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

        public void MoveEnemy() => enemyData.isEnemyMove = true; // 적을 움직이는 상태로 바꿈

        public EnemyType GetEnemyType() => enemyData.enemyType; // 적 타입을 가져옴
        public EnemyController GetEnemyController() => enemyData.eEnemyController;
        public Vector2? GetKnockBackDirection() => enemyData.knockBackDirection; // 적이 넉백 공격을 할 수 있는지를 가져옴
        public string GetEnemyId() => enemyData.enemyType.ToString(); // 적 아이디를 가져옴
        public float EnemyHpPercent() => ((float)enemyData.hp / enemyData.maxHP) * 100f; // 적 체력 퍼센트를 가져옴
        public float GetKnockBackPower() => enemyData.knockBackPower; // 적이 넉백 공격을 할 수 있는지를 가져옴
        public int GetEnemyAttackPower() => enemyData.attackPower; // 적 공격력을 가져옴
        public bool GetIsKnockBack() => enemyData.isUseKnockBack; // 적이 넉백 공격을 할 수 있는지를 가져옴
    }
}