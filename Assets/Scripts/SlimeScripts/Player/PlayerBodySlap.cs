using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodySlap : PlayerSkill
{
    private Stat playerStat = null;
    [SerializeField]
    private LayerMask canCrashLayer;
    [SerializeField]
    private LayerMask whatIsEnemy;

    [SerializeField]
    private Vector2 bodySlapMoveVec = Vector2.zero;
    private Vector2 lastPos = Vector2.zero; //BodySlap 후 도달할 위치
    private Vector2 moveOriginPos = Vector2.zero;
    private Vector2 moveTargetPos = Vector2.zero;

    [SerializeField]
    private float maxChargingTime = 3f;
    public float MaxChargingTime
    {
        get { return maxChargingTime; }
    }
    [SerializeField]
    private float minMoveToMouseChargeTime = 0.1f; // currentChargingTimer 값이 이 값보다 높아야 마우스로 이동한다.
    private float currentChargingTimer = 0f; // 이 타이머는 0에서 플레이어가 대쉬를 눌렀을 때 부터 올라간다. 이 값이 maxChargingTime과
                                             // 같으면 풀차징으로 판정한다.

    [Header("차징 타임에따라 오르는 돌진 거리의 값")]
    [SerializeField]
    private float targetPosFarPerCharge = 1f;

    [Header("BodySlap이 기본 데미지의 몇배의 데미지를 줄 것인가에 대한 값.")]
    [Header("예를 들어 이 값이 2 이고 기본 데미지가 4면 8의 BodySlap데미지가 들어감")]
    [SerializeField]
    private float damageMagnificationOfBodySlap = 2f;

    [Header("돌진하기 전 살짝 뒤로 뺄 때의 속도")]
    [SerializeField]
    private float moveBackSpeed = 8f;
    [SerializeField]
    private float bodySlapMoveSpeed = 2f;

    [Header("기본 돌진 거리 값")]
    [SerializeField]
    private float offsetBodySlapMovePos = 2f;
    [SerializeField]
    private float bodySlapTime = 3f;
    private float currentBodySlapTime = 0f;
    private float bodySlapTimer = 0f;

    [Header("BodySlap이 완전히 멈출 때 까지 걸리는 시간")]
    [SerializeField]
    private float stopBodySlapTime = 2f;
    private float stopBodySlapTimer = 0f;

    [Header("BodySlap이 멈출 때 적용되는 관성 관련 값 (값에 따라 서서히 멈추는 정도가 달라짐)")]
    [SerializeField]
    private float stopBodySlapOffset = 3f;

    private bool canBodySlap = true;
    private bool startCharging = false;

    private bool maxCharging = true;
    public bool MaxCharging
    {
        get { return maxCharging; }
    }

    private bool bodySlapStart = false;
    private bool bodyStopBodySlapTimerStart = false;

    [SerializeField]
    private LineRenderer bodySlapLine = null;

    [SerializeField]
    private float bodySlapLineLength = 1f;

    [SerializeField]
    private List<Material> materials = new List<Material>();

    public override void Awake()
    {
        base.Awake();
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;
    }
    private void Start()
    {
        bodySlapTimer = bodySlapTime;
    }
    public override void OnEnable()
    {
        base.OnEnable();

        EventManager.StartListening("BodyPointCrash", BodyPointCrash);
        EventManager.StartListening("PlayerDead", StopBodySlap);
    }
    public override void OnDisable()
    {
        base.OnDisable();

        EventManager.StopListening("BodyPointCrash", BodyPointCrash);
        EventManager.StopListening("PlayerDead", StopBodySlap);
    }
    public override void Update()
    {
        base.Update();

        CheckChargeTime();
        CheckBodySlapTime();
        CheckStopBodySlapTime();
    }
    private void FixedUpdate()
    {
        if (playerState.BodySlapping && bodySlapStart && !bodyStopBodySlapTimerStart) // 움직임
        {
            if(currentBodySlapTime <= 0f)
            {
                currentBodySlapTime = bodySlapTime;
            }

            transform.position = Vector2.Lerp(moveOriginPos, moveTargetPos, bodySlapTimer / currentBodySlapTime);
        }
    }
    public override void DoSkill()
    {
        base.DoSkill();

        // currentChargingTimer 측정을 시작한다.

        if (canBodySlap)
        {
            canBodySlap = false;

            bodySlapLine.gameObject.SetActive(true);

            playerState.Chargning = true;

            bodySlapMoveVec = playerInput.LastMoveVector;
            currentBodySlapTime = bodySlapTime;

            startCharging = true;
            maxCharging = false;

            currentChargingTimer = 0f;
        }
    }

    public override void SkillButtonUp()
    {
        base.SkillButtonUp();

        if (startCharging)
        {
            DoBodySlap();
        }
    }
    private void DoBodySlap()
    {
        bodySlapLine.gameObject.SetActive(false);

        playerState.Chargning = false;
        playerState.BodySlapping = true;
        bodySlapStart = true;

        startCharging = false;
        maxCharging = false;

        moveOriginPos = transform.position;

        Debug.DrawRay(moveOriginPos, moveTargetPos, Color.red, 10f);

        moveTargetPos = SlimeGameManager.Instance.PosCantCrossWall(canCrashLayer, moveOriginPos, moveTargetPos);

        currentBodySlapTime = Vector2.Distance(moveOriginPos, moveTargetPos) / bodySlapMoveSpeed;

        SoundManager.Instance.PlaySoundBox("SlimeSkill1Start");

        EventManager.TriggerEvent("PlayerBodySlap", currentBodySlapTime);

        bodySlapTimer = 0f;

        SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];
    }
    private void BodyPointCrash(GameObject targetObject) // BodyPoint가 특정 오브젝트와 충돌했을 때 호출
    {
        if (canCrashLayer.CompareGameObjectLayer(targetObject) && playerState.BodySlapping)
        {
            IDamageableBySlimeBodySlap damagableByBodySlap = targetObject.GetComponent<IDamageableBySlimeBodySlap>();

            if (damagableByBodySlap != null)
            {
                damagableByBodySlap.GetDamage(1, currentChargingTimer);// 여기에 매개변수 추가
            }
            else
            {
                ICanGetDamagableEnemy enemy = targetObject.GetComponent<ICanGetDamagableEnemy>();

                if (enemy != null)
                {
                    SlimeGameManager.Instance.Player.Mag_GiveDamage(enemy, SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, damageMagnificationOfBodySlap);

                    EventManager.TriggerEvent("OnEnemyAttack");
                }
                else
                {
                    EventManager.TriggerEvent("OnAttackMiss");
                }

                if (!bodyStopBodySlapTimerStart)
                {
                    StopBodySlap();

                    bodyStopBodySlapTimerStart = true;
                    stopBodySlapTimer = stopBodySlapTime;
                }

                SoundManager.Instance.PlaySoundBox("SlimeSkill1Crash");
            }
        }
    }
    private void StartBodySlap()
    {

    }
    public override void WhenSkillDelayTimerZero()
    {
        base.WhenSkillDelayTimerZero();

        canBodySlap = true;
    }
    private void StopBodySlap()
    {
        stopBodySlapTimer = 0f;

        bodySlapStart = false;
        playerState.BodySlapping = false;
    }
    private void CheckChargeTime()
    {
        if(startCharging)
        {
            if (!maxCharging)
            {
                currentChargingTimer += Time.deltaTime;

                if (currentChargingTimer > maxChargingTime)
                {
                    currentChargingTimer = maxChargingTime;
                    maxCharging = true;
                }
            }

            EventManager.TriggerEvent("PlayerCharging");

            int chargingNum = (int)((currentChargingTimer / maxChargingTime) * (materials.Count - 2)) + 1;
            int matNum = 0;

            if (currentChargingTimer > minMoveToMouseChargeTime)
            {
                matNum = (int)((currentChargingTimer / maxChargingTime) * (materials.Count - 2)) + 1;

                    bodySlapMoveVec = (playerInput.MousePosition - (Vector2)transform.position).normalized;

                    moveTargetPos = (Vector2)transform.position + bodySlapTime * offsetBodySlapMovePos * bodySlapMoveVec +
    chargingNum * targetPosFarPerCharge * bodySlapMoveVec;

            }
            else
            {
                moveTargetPos = (Vector2)transform.position + bodySlapTime * offsetBodySlapMovePos * bodySlapMoveVec
                    /*+ minMoveToMouseChargeTime * targetPosFarPerCharge * bodySlapMoveVec*/;
            }

            bodySlapLine.SetPosition(0, transform.position);
            bodySlapLine.SetPosition(1, (Vector2)transform.position + bodySlapMoveVec * bodySlapLineLength);

            bodySlapLine.material = materials[matNum];
        }
    }
    private void CheckBodySlapTime()
    {
        if (bodySlapTimer < currentBodySlapTime)
        {
            bodySlapTimer += Time.deltaTime;

            if (bodySlapTimer >= currentBodySlapTime)
            {
                stopBodySlapTimer = stopBodySlapTime;
                bodyStopBodySlapTimerStart = false;
            }
        }
        else
        {
            bodyStopBodySlapTimerStart = false;
        }
    }
    private void CheckStopBodySlapTime()
    {
        if (stopBodySlapTimer > 0f)
        {
            stopBodySlapTimer -= Time.deltaTime;

            rigid.AddForce(Vector2.Lerp(Vector2.zero, -rigid.velocity / stopBodySlapOffset, stopBodySlapTimer / stopBodySlapTime));

            if (stopBodySlapTimer <= 0f)
            {
                StopBodySlap();
            }
        }
    }
}
