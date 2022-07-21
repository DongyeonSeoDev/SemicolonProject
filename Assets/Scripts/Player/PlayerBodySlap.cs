using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodySlap : PlayerSkill
{
    [SerializeField]
    private LayerMask cantCrossLayer;
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
    private CamShakeData doBodySlapShakeData;
    [SerializeField]
    private CamShakeData whenBodySlapCrashShakeData;

    [SerializeField]
    private float maxChargingTime = 3f;
    public float MaxChargingTime
    {
        get { return maxChargingTime; }
    }

    private float currentChargingTimer = 0f; // 이 타이머는 0에서 플레이어가 대쉬를 눌렀을 때 부터 올라간다. 이 값이 maxChargingTime과
                                             // 같으면 풀차징으로 판정한다.

    [Header("차징 타임에따라 오르는 돌진 거리의 값")]
    [SerializeField]
    private float targetPosFarPerCharge = 1f;

    [Header("BodySlap이 기본 데미지의 몇배의 데미지를 줄 것인가에 대한 값.")]
    [Header("예를 들어 이 값이 2 이고 기본 데미지가 4면 8의 BodySlap데미지가 들어감")]
    [SerializeField]
    private float damageMagnificationOfBodySlap = 2f;

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
    private PlayerEffectScript bodySlapChargingEffect = null;

    [SerializeField]
    private float bodySlapLineLength = 1f;

    [SerializeField]
    private List<Material> materials = new List<Material>();

    private List<GameObject> hitWhenBodySlap = new List<GameObject>(); // 중복처리를 위한 리스트

    public override void Awake()
    {
        base.Awake();
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
            StopBodySlap();

            canBodySlap = false;

            bodySlapChargingEffect.gameObject.SetActive(true);
            bodySlapLine.gameObject.SetActive(true);

            playerState.Chargning = true;

            currentBodySlapTime = bodySlapTime;

            startCharging = true;
            maxCharging = false;

            currentChargingTimer = 0f;
            moveTargetPos = Vector2.zero;
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
        bodySlapChargingEffect.gameObject.SetActive(false);
        bodySlapLine.gameObject.SetActive(false);

        playerState.Chargning = false;
        playerState.BodySlapping = true;
        bodySlapStart = true;

        startCharging = false;
        maxCharging = false;

        moveOriginPos = transform.position;

        Debug.DrawRay(moveOriginPos, moveTargetPos, Color.red, 10f);

        moveTargetPos = SlimeGameManager.Instance.PosCantCrossWall(cantCrossLayer, moveOriginPos, moveTargetPos);

        currentBodySlapTime = Vector2.Distance(moveOriginPos, moveTargetPos) / bodySlapMoveSpeed;

        doBodySlapShakeData.duration = currentBodySlapTime;
        CinemachineCameraScript.Instance.Shake(doBodySlapShakeData);

        SoundManager.Instance.PlaySoundBox("SlimeSkill1Start");

        EventManager.TriggerEvent("PlayerBodySlap", currentBodySlapTime);
        EventManager.TriggerEvent("OnBodySlap");

        bodySlapTimer = 0f;

        SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];
    }
    private void BodyPointCrash(GameObject targetObject) // BodyPoint가 특정 오브젝트와 충돌했을 때 호출
    {   
        if ((canCrashLayer.CompareGameObjectLayer(targetObject) || whatIsEnemy.CompareGameObjectLayer(targetObject)) && playerState.BodySlapping)
        {
            if(hitWhenBodySlap.Contains(targetObject))
            {
                return;
            }

            hitWhenBodySlap.Add(targetObject);

            StageDoor stageDoor = targetObject.GetComponent<StageDoor>();

            if(stageDoor != null && (stageDoor.IsOpen || stageDoor.IsExitDoor))
            {
                return;
            }

            IDamageableBySlimeBodySlap damagableByBodySlap = targetObject.GetComponent<IDamageableBySlimeBodySlap>();

            if (damagableByBodySlap != null)
            {
                damagableByBodySlap.GetDamage(1, currentChargingTimer);// 여기에 매개변수 추가

                if (!bodyStopBodySlapTimerStart && !SlimeGameManager.Instance.Player.PlayerStat.choiceStat.frenzy.isUnlock)
                {
                    TutorialStageDoor tutoStageDoor = targetObject.GetComponent<TutorialStageDoor>();

                    if (tutoStageDoor == null)
                    {
                        StopBodySlap();

                        bodyStopBodySlapTimerStart = true;
                        stopBodySlapTimer = stopBodySlapTime;
                    }
                }
            }
            else
            {
                ICanGetDamagableEnemy enemy = targetObject.GetComponent<ICanGetDamagableEnemy>();

                if (enemy != null)
                {
                    SlimeGameManager.Instance.Player.Mag_GiveDamage(enemy, SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, enemy.GetTransform().position, bodySlapMoveVec, damageMagnificationOfBodySlap, effectSize: new Vector3(1.5f, 1.5f, 1.5f));

                    EventManager.TriggerEvent("OnEnemyAttack");
                }
                else
                {
                    EventManager.TriggerEvent("OnAttackMiss");
                }

                if (!bodyStopBodySlapTimerStart && !SlimeGameManager.Instance.Player.PlayerStat.choiceStat.frenzy.isUnlock)
                {
                    StopBodySlap();

                    bodyStopBodySlapTimerStart = true;
                    stopBodySlapTimer = stopBodySlapTime;
                }

                SoundManager.Instance.PlaySoundBox("SlimeSkill1Crash");
                CinemachineCameraScript.Instance.Shake(whenBodySlapCrashShakeData);
            }
        }
    }
    public override void SkillCancel()
    {
        base.SkillCancel();

        startCharging = false;
        canBodySlap = true;

        EventManager.TriggerEvent("PlayerChargingCancel");

        bodySlapChargingEffect.gameObject.SetActive(false);
        bodySlapLine.gameObject.SetActive(false);

        playerState.Chargning = false;

        StopBodySlap();
    }
    public override void WhenSkillDelayTimerZero()
    {
        base.WhenSkillDelayTimerZero();

        canBodySlap = true;
    }
    private void StopBodySlap()
    {
        hitWhenBodySlap.Clear();
        stopBodySlapTimer = 0f;

        bodySlapStart = false;
        playerState.BodySlapping = false;

        bodySlapTimer = bodySlapTime;
        moveTargetPos = Vector2.zero;
    }
    private void CheckChargeTime()
    {
        if (startCharging)
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

            int chargingNum = (int)((currentChargingTimer / maxChargingTime) * (materials.Count - 1));
            int matNum = 0;
            
            matNum = (int)((currentChargingTimer / maxChargingTime) * (materials.Count - 1));

            bodySlapMoveVec = (playerInput.MousePosition - (Vector2)transform.position).normalized;
            playerInput.LastBodySlapVector = bodySlapMoveVec;

            moveTargetPos = (Vector2)transform.position + bodySlapTime * offsetBodySlapMovePos * bodySlapMoveVec +
chargingNum * targetPosFarPerCharge * bodySlapMoveVec;

            bodySlapLine.SetPosition(0, transform.position);
            bodySlapLine.SetPosition(1, (Vector2)transform.position + bodySlapMoveVec * bodySlapLineLength);

            bodySlapLine.material = materials[chargingNum];
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
