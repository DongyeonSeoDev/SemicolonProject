using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Playables;

namespace Enemy
{
    public class Boss2Centipede : Enemy
    {
        public Transform movePivot;
        public Tilemap stageTilemap = null;
        public PlayableDirector playableDirector;

        private Vector2 dashTargetPosition = Vector2.zero;

        private Color originSpriteColor = Color.white;

        private float originMinAttackPower = 0f;
        private float originMaxAttackPower = 0f;

        private float bulletMinAttackPower = 0f;
        private float bulletMaxAttackPower = 0f;

        private float originCurrentSpeed = 0f;

        private float shootBulletRotationOffset = 0f;
        private float shootBulletRotationValue = 15f; // 각도 변동값

        private float stopAnimTimer = 0f;
        [SerializeField]
        private float minMoveTimer = 1f;
        [SerializeField]
        private float maxMoveTimer = 3f;

        private float moveTimer = 0f;
        private float dashTimer = 0f;
        private float meleeAttackDis = 12f;

        private float meleeAttack1Percentage = 30f; // 지면파괴 확률
        private float meleeAttack2Percentage = 20f; // 물어뜯기 확률
        private float sneerPercentage = 50; // 침뱉기 확률
        private float multipleMeleeAttack1Percentage = 0f; // 연속 지면파괴 확률
        private float bitingAndTearingPercentage = 0f; // 물고 뜯기 확률

        #region meleeAttack1(지면파괴)관련 변수
        [Header("meleeAttack1(지면파괴)의 원통형 공격 범위")]
        [SerializeField]
        private float meleeAttack1CircleRadius = 2;
        [Header("meleeAttack1(지면파괴)의 원통형 데미지 변동값")]
        [SerializeField]
        private float meleeAttack1CircleDamageValue = 3;
        [Header("meleeAttack1(지면파괴)의 구체 데미지 변동 값")]
        [SerializeField]
        private float meleeAttack1BulletDamageValue = -10; 

        [Header("meleeAttack1(지면파괴)의 차징시 색깔")]
        [SerializeField]
        private Color meleeAttack1ChargingColor = Color.white;
        private Color upColorValueByCharging = Color.white;
        private float meleeAttack1ChargingColorLerpEndTime = 0f;
        private float meleeAttack1ChargingColorTimer = 0f;
        private bool meleeAttack1ChargingColorTimerStart = false;
        #endregion

        #region meleeAttack2(물어뜯기)관련 변수
        [Header("meleeAttack2(물어뜯기)의 돌진 거리")]
        [SerializeField]
        private float dashAttackDis = 5f;
        [Header("meleeAttack2(물어뜯기)의 돌진 데미지 변동 값")]
        [SerializeField]
        private float meleeAttack2DamageValue = -5f;
        #endregion

        #region sneer(침뱉기)관련 변수
        [Header("Sneer(침뱉기)의 구체 데미지 변동 값")]
        [SerializeField]
        private float sneerDamageValue = -10f;
        #endregion

        private bool isMove = false;
        private bool isDashToPlayer = false;
        private bool isAttack = false;
        private bool prevIsAttack = false;

        public readonly int hashMeleeAttack1 = Animator.StringToHash("meleeAttack1");
        public readonly int hashMeleeAttack2 = Animator.StringToHash("meleeAttack2");
        public readonly int hashSneer = Animator.StringToHash("sneer");

        public readonly int hashAttackEnd = Animator.StringToHash("attackEnd");
        private readonly int hashMove = Animator.StringToHash("move");

        private int meleeAttack1Count = 0;
        private int sneerCount = 0;

        private EnemyCommand enemyMoveCommand;
        private EnemyCommand dashCommand;
        private EnemyCommand enemySneerCommand;
        private EnemyCommand shootAroundCommand;

        [HideInInspector] public LayerMask whatIsWall;
        [HideInInspector] public float currentSpeed = 0f;

        [SerializeField]
        private EnemyBullet sneerBullet = null;
        [SerializeField]
        private Transform shootTrm = null;

        [SerializeField]
        private Collider2D enemyMeleeAttack1Collider = null;
        [SerializeField]
        private Collider2D enemyMeleeAttack2Collider = null;

        private BossCanvas bossHPBar;

        [SerializeField]
        private CamShakeData shootAroundShakeData;

        protected override void Awake()
        {
            base.Awake();

            enemyMeleeAttack1Collider.GetComponent<CircleCollider2D>().radius = meleeAttack1CircleRadius;
            currentSpeed = enemyDataSO.speed;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetBossHPBar();

            stageTilemap = null;
            foreach (var item in StageManager.Instance.stageParent.GetComponentsInChildren<Tilemap>())
            {
                if(item.gameObject.name == "BackgroundTilemap")
                {
                    stageTilemap = item;
                    break;
                }
            }

            bossHPBar = GetComponentInChildren<BossCanvas>();
            bossHPBar.Init(enemyData);

            enemyData.enemyCanvas = bossHPBar.gameObject;

            enemyData.attackDelay = 2f;
            enemyData.noAttackTime = 5f;
            enemyData.isAttackPlayerDistance = 20f;
            enemyData.enemySpriteRenderer = GetComponent<SpriteRenderer>();
            enemyData.isNoKnockback = true;
            enemyData.isNoStun = true;

            originSpriteColor = sr.color;
            originCurrentSpeed = currentSpeed;
            originMinAttackPower = enemyData.minAttackPower;
            originMaxAttackPower = enemyData.maxAttackPower;

            SetCommands();

            EventManager.StartListening("PlayerDead", StopAttack);
            EventManager.StartListening("BossDead", StopAttack);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            EventManager.StopListening("PlayerDead", StopAttack);
            EventManager.StopListening("BossDead", StopAttack);
        }
        protected override void Update()
        {
            base.Update();

            if (isStop || !enemyData.isEnemyMove)
            {
                return;
            }

            PercentageCheck();

            StopAnimTimerCheck();
            MeleeAttack1ColorTimer();

            DashCheck();
            MoveCheck();
        }
        
        private void PercentageCheck()
        {
            // 체력에따른 퍼센테이지 변경

            float hpPer = (enemyData.hp / enemyData.maxHP) * 100f ;

            #region 확률 조정 코드
            if (hpPer <= 10f)
            {
                meleeAttack1Percentage = 0f;
                meleeAttack2Percentage = 45f;
                sneerPercentage = 20f;
                multipleMeleeAttack1Percentage = 35f;
                bitingAndTearingPercentage = 0f;
            }
            else if(hpPer <= 20f)
            {
                meleeAttack1Percentage = 10f;
                meleeAttack2Percentage = 15f;
                sneerPercentage = 5f;
                multipleMeleeAttack1Percentage = 40f;
                bitingAndTearingPercentage = 30f;
            }
            else if(hpPer <= 40f)
            {
                meleeAttack1Percentage = 20f;
                meleeAttack2Percentage = 30f;
                sneerPercentage = 30f;
                multipleMeleeAttack1Percentage = 15f;
                bitingAndTearingPercentage = 25f;
            }
            else if(hpPer <= 60f)
            {
                meleeAttack1Percentage = 25f;
                meleeAttack2Percentage = 45f;
                sneerPercentage = 20f;
                multipleMeleeAttack1Percentage = 5f;
                bitingAndTearingPercentage = 5f;
            }
            else if(hpPer <= 80f)
            {
                meleeAttack1Percentage = 40f;
                meleeAttack2Percentage = 25f;
                sneerPercentage = 30f;
                multipleMeleeAttack1Percentage = 5f;
                bitingAndTearingPercentage = 0f;
            }
            else
            {
                meleeAttack1Percentage = 30f;
                meleeAttack2Percentage = 20f;
                sneerPercentage = 50f;
                multipleMeleeAttack1Percentage = 0f;
                bitingAndTearingPercentage = 0f;
            }
            #endregion
        }
        private void MoveCheck()
        {
            Vector3 playerPosition = EnemyManager.Player.transform.position;
            float dis = Vector3.Distance(playerPosition, transform.position);

            if (isMove && !isDashToPlayer)
            {
                moveTimer -= Time.deltaTime;

                enemyMoveCommand.Execute();

                if (moveTimer <= 0)
                {
                    moveTimer = 0f;
                    isMove = false;
                    prevIsAttack = false;
                    AttackCheck();
                }
            }
        }
        public override void MoveEnemy()
        {
            EventManager.TriggerEvent("StartCutScene");

            playableDirector.Play();
        }
        public void EndCutScene()
        {
            EventManager.TriggerEvent("EndCutScene");

            base.MoveEnemy();
        }
        private void SetBossHPBar()
        {
            bossHPBar = GetComponentInChildren<BossCanvas>();
            bossHPBar.Init(enemyData);

            enemyData.enemyCanvas = bossHPBar.gameObject;

            if (bossHPBar != null)
            {
                bossHPBar.SetFill();
            }
        }

        private void DashCheck()
        {
            if (isDashToPlayer)
            {
                dashTimer -= Time.deltaTime;

                if (dashCommand == null)
                {
                    DashToPlayerEnd();
                }

                dashCommand.Execute();

                if (dashTimer <= 0)
                {
                    DashToPlayerEnd();
                }
            }
        }
        public void DashToPlayer(float speed) // 대쉬 시작
        {
            dashTargetPosition = EnemyManager.Player.transform.position - ((Vector3)EnemyManager.Player.GetComponent<PlayerMove>().LastMoveVec * Time.deltaTime / speed);

            isDashToPlayer = true;
            dashTimer = dashAttackDis / speed;
            currentSpeed = speed;

            dashCommand = new CentipedeBossDashCommand(enemyData, dashTargetPosition, rb, this);
        }
        private void DashToPlayerEnd()// 대쉬 끝났을 때
        {
            currentSpeed = originCurrentSpeed;
            dashCommand = null;
            isDashToPlayer = false;
        }

        private void SetCommands()
        {
            enemyMoveCommand = new CentipedeBossMoveCommand(enemyData, movePivot, rb, this);

            enemyData.enemyMoveCommand = enemyMoveCommand;
            
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyChaseStateChangeCondition = AttackCheck;
            enemyData.addAIAttackStateChangeCondition = AttackStateChangeCondition;
            enemyData.addChangeAttackCondition = ChangeAttackCondition;
        }
        protected override void SetHP(bool useTween)
        {
            if (bossHPBar != null)
            {
                bossHPBar.SetFill();
            }

            if (enemyData.hp <= 0)
            {
                EventManager.TriggerEvent("BossDead");
            }
        }
        public void StopAttack() // EventManager에서 실행 - 적 공격 정지
        {
            StopAllCoroutines();

            bossHPBar.SetActiveHPBar(false);
        }
        public EnemyState AttackCheck() // 이벤트 구독에 사용됨 - 특수공격 사용 확인
        {
            if (isAttack)
            {
                prevIsAttack = true;
                return new EnemyAIAttackState(enemyData);
            }

            if (isMove)
            {
                return null;
            }

            float value = Random.Range(0f, 100f);
            float checkValue = 0f;

            if (prevIsAttack)
            {
                isAttack = false;
                isMove = true;
                moveTimer = Random.Range(minMoveTimer, maxMoveTimer);

                enemyData.animationDictionary[EnemyAnimationType.Move] = hashMove;
                enemyMoveCommand.Execute();

                return null;
            }
            else
            {
                if ((checkValue += meleeAttack1Percentage) >= value)
                {
                    enemyData.attackDelay = 1f;

                    shootBulletRotationOffset = 0f;
                    meleeAttack1Count = 1;
                }
                else if ((checkValue += meleeAttack2Percentage) >= value)
                {
                    enemyData.attackDelay = 1f;

                    enemyData.animationDictionary[EnemyAnimationType.Attack] = hashMeleeAttack2;
                }
                else if ((checkValue += sneerPercentage) >= value)
                {
                    enemyData.attackDelay = 2f;

                    sneerCount = 3;
                }
                else if ((checkValue += multipleMeleeAttack1Percentage) >= value)
                {
                    enemyData.attackDelay = 1f;

                    shootBulletRotationOffset = 0f;
                    meleeAttack1Count = 3;
                }
                else if ((checkValue += bitingAndTearingPercentage) >= value)
                {
                    Debug.Log("DoBitingAndTearing");
                }

                prevIsAttack = true;
                isAttack = true;
                return new EnemyAIAttackState(enemyData);
            }
        }
        public void SetOriginSpriteColor()
        {
            sr.color = originSpriteColor;
        }
        public void SetMeleeAttack1Color(float time)
        {
            if(time <= 0f)
            {
                sr.color = meleeAttack1ChargingColor;

                return;
            }

            upColorValueByCharging = meleeAttack1ChargingColor - originSpriteColor;

            Debug.Log(upColorValueByCharging);

            meleeAttack1ChargingColorLerpEndTime = time;
            meleeAttack1ChargingColorTimer = 0f;

            meleeAttack1ChargingColorTimerStart = true;
        }
        private void MeleeAttack1ColorTimer()
        {
            if(meleeAttack1ChargingColorTimerStart)
            {
                if(meleeAttack1ChargingColorTimer < meleeAttack1ChargingColorLerpEndTime)
                {
                    Color color = upColorValueByCharging * (meleeAttack1ChargingColorTimer / meleeAttack1ChargingColorLerpEndTime);
                    meleeAttack1ChargingColorTimer += Time.deltaTime;

                    sr.color = originSpriteColor + new Color(color.r, color.g, color.b, sr.color.a);
                }

                if (meleeAttack1ChargingColorTimer >= meleeAttack1ChargingColorLerpEndTime)
                {
                    meleeAttack1ChargingColorTimerStart = false;
                }
            }
        }
        public void StopAnim(float stopTime)
        {
            if(stopTime <= 0)
            {
                return;
            }

            stopAnimTimer = stopTime;

            SetAnimSpeed(0f);
        }
        private void StopAnimTimerCheck()
        {
            if(stopAnimTimer > 0f)
            {
                stopAnimTimer -= Time.deltaTime;

                if(stopAnimTimer <= 0f)
                {
                    SetAnimSpeed(1f);
                }
            }
        }
        public void SetAnimSpeed(float speed)
        {
            anim.speed = speed;
        }
        // sneer(침뱉기)에 쓰이는 총알 발사 함수
        public void ShootBulletToPlayer()
        {
            enemySneerCommand = new CentipedeLongRangeAttackCommand(this, shootTrm, EnemyManager.Player.transform.position, Type.CentipedeBullet, shootTrm, bulletMinAttackPower, bulletMaxAttackPower, enemyData.randomCritical, enemyData.randomCritical);
            enemySneerCommand.Execute();
        }
        // meleeAttack1(지면 파괴)에 쓰이는 총알 발사 함수
        public void ShootBulletAround(int num)
        {
            shootAroundCommand = new CentipedeShootAroundCommand(this, shootTrm, Type.CentipedeAroundBullet, bulletMinAttackPower, bulletMaxAttackPower, enemyData.randomCritical, enemyData.randomCritical, true, stageTilemap);
            CinemachineCameraScript.Instance.Shake(shootAroundShakeData);

            for (int i = 0; i < num; i++)
            {
                shootAroundCommand.Execute();
            }
        }
        public void BrakeGroundParticle()
        {
            EnemyPoolManager.Instance.GetPoolObject(Type.CentipedeBrakeEffect, shootTrm.position + new Vector3(0f, 0.7f, 0f));
        }
        public EnemyState AttackStateChangeCondition() // 이벤트 구독에 사용됨 - 공격2 사용 가능 확인
        {
            if (isMove)
            {
                return new EnemyChaseState(enemyData);
            }

            if (sneerCount > 0)
            {
                sneerCount--;

                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashSneer;

                return new EnemyAIAttackState(enemyData);
            }
            else if (meleeAttack1Count > 0)
            {
                meleeAttack1Count--;

                shootBulletRotationOffset += shootBulletRotationValue;

                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashMeleeAttack1;

                return new EnemyAIAttackState(enemyData);
            }

            return new EnemyChaseState(enemyData);
        }
        public EnemyState ChangeAttackCondition() // 이벤트 구독에 사용됨 - 공격을 해야하는지 확인
        {
            return null;
        }
        public void CentipedeMeleeAttack1Start()
        {
            CentipedeAttackStart();

            enemyData.minAttackPower += meleeAttack1CircleDamageValue;
            enemyData.maxAttackPower += meleeAttack1CircleDamageValue;

            bulletMinAttackPower = originMinAttackPower + meleeAttack1BulletDamageValue;
            bulletMaxAttackPower = originMaxAttackPower + meleeAttack1BulletDamageValue;
        }
        public void CentipedeMeleeAttack2Start()
        {
            CentipedeAttackStart();

            enemyData.minAttackPower += meleeAttack2DamageValue;
            enemyData.maxAttackPower += meleeAttack2DamageValue;
        }
        public void CentipedeSneerStart()
        {
            CentipedeAttackStart();

            bulletMinAttackPower = originMinAttackPower + sneerDamageValue;
            bulletMaxAttackPower = originMaxAttackPower + sneerDamageValue;
        }
        private void CentipedeAttackStart()
        {
            //prevIsAttack = true;
            //isAttack = true;
        }
        public void CentipedeAttackEnd()
        {
            isAttack = false;
            anim.speed = 1f;

            enemyData.minAttackPower = originMinAttackPower;
            enemyData.maxAttackPower = originMaxAttackPower;

            bulletMinAttackPower = originMinAttackPower;
            bulletMaxAttackPower= originMaxAttackPower;

            enemyMeleeAttack1Collider.GetComponent<EnemyAttackCheck>().AttackObjectReset();
            enemyMeleeAttack2Collider.GetComponent<EnemyAttackCheck>().AttackObjectReset();

            enemyData.enemyAnimator.SetTrigger(hashAttackEnd);
        }
    }
}
