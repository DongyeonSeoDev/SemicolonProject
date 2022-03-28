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

        protected EnemyData enemyData; // 자식 클래스에서 만들어야 함

        // GetComponent로 가져옴
        protected SpriteRenderer sr; 
        protected Animator anim;
        protected Rigidbody2D rb;

        // SlimeGameManager에서 가져옴
        protected PlayerInput playerInput;

        private EnemyState currentState; // Enemy FSM 실행

        public EnemyAttackCheck enemyAttackCheck; // 적 공격 확인 ( 근거리 적은 있고 원거리 적은 없음 )

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

        private void Start()
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
            enemyData.enemyAnimator.enabled = true; // 애니메이션 실행

            // 마지막 위치, HP UI, 애니메이션, 죽음 확인 리셋
            enemyData.hpBarFillImage.fillAmount = (float)enemyData.hp / enemyData.maxHP;
            enemyData.enemyAnimator.SetTrigger(enemyData.hashReset);
            enemyData.enemyAnimator.SetBool(enemyData.hashIsDead, false);

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

        // 적 아이디를 가져옴
        public string GetEnemyId()
        {
            return enemyData.enemyType.ToString();
        }

        // 적을 움직이는 상태로 바꿈
        public void MoveEnemy()
        {
            enemyData.isEnemyMove = true;
        }

        // 적 체력 퍼센트를 가져옴
        public float EnemyHpPercent()
        {
            return ((float)enemyData.hp / enemyData.maxHP) * 100f;
        }

        // 적 컨트롤러를 다른것으로 바꿈
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