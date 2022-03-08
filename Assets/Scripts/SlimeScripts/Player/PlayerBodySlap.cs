using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodySlap : PlayerSkill
{
    private Stat playerStat = null;
    [SerializeField]
    private LayerMask canCrashLayer;

    [SerializeField]
    private Vector2 bodySlapMoveVec = Vector2.zero;
    private Vector2 lastPos = Vector2.zero; //BodySlap 후 도달할 위치
    private Vector2 moveOriginPos = Vector2.zero;
    private Vector2 moveTargetPos = Vector2.zero;

    [Header("BodySlap이 기본 데미지의 몇배의 데미지를 줄 것인가에 대한 값.")]
    [Header("예를 들어 이 값이 2 이고 기본 데미지가 4면 8의 BodySlap데미지가 들어감")]
    [SerializeField]
    private float damageMagnificationOfBodySlap = 2f;

    [Header("돌진하기 전 살짝 뒤로 뺄 때의 속도")]
    [SerializeField]
    private float moveBackSpeed = 8f;
    [SerializeField]
    private float bodySlapMoveSpeed = 2f;

    // [SerializeField]
    // private float bodySlapDelay = 5f;
    // private float bodySlapDelayTimer = 0f;

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
    private bool bodySlapStart = false;
    private bool bodyStopBodySlapTimerStart = false;

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

        //if (playerState.BodySlapping && !bodySlapStart && canBodySlap)
        //{
        //    DoSkill();
        //}
        //else if (!canBodySlap)
        //{
        //    playerState.BodySlapping = false;
        //}

        CheckBodySlapTime();
        CheckStopBodySlapTime();
    }
    private void FixedUpdate()
    {
        if (playerState.BodySlapping && bodySlapStart && !bodyStopBodySlapTimerStart) // 움직임
        {
            transform.position = Vector2.Lerp(moveOriginPos, moveTargetPos, bodySlapTimer / currentBodySlapTime);
        }
    }
    public override void DoSkill()
    {
        base.DoSkill();

        if (canBodySlap)
        {
            playerState.BodySlapping = true;
            bodySlapStart = true;

            bodySlapMoveVec = playerInput.LastMoveVector;

            currentBodySlapTime = bodySlapTime;

            moveOriginPos = transform.position;
            moveTargetPos = moveOriginPos + bodySlapTime * bodySlapMoveSpeed * bodySlapMoveVec;
            moveTargetPos = SlimeGameManager.Instance.PosCantCrossWall(canCrashLayer, moveOriginPos, moveTargetPos);

            currentBodySlapTime = Vector2.Distance(moveOriginPos, moveTargetPos) / bodySlapMoveSpeed;

            EventManager.TriggerEvent("PlayerBodySlap", bodySlapTime);

            bodySlapTimer = 0f;
            SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = skillDelay;
        }
    }
    private void BodyPointCrash(GameObject targetObject) // BodyPoint가 특정 오브젝트와 충돌했을 때 호출
    {
        if (canCrashLayer.CompareGameObjectLayer(targetObject) && playerState.BodySlapping)
        {
            Enemy.Enemy enemy = targetObject.GetComponent<Enemy.Enemy>();

            if (enemy != null)
            {
                SlimeGameManager.Instance.Player.GiveDamage(enemy, SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, damageMagnificationOfBodySlap, true);
            }

            if (!bodyStopBodySlapTimerStart)
            {
                StopBodySlap();

                bodyStopBodySlapTimerStart = true;
                stopBodySlapTimer = stopBodySlapTime;
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

        canBodySlap = false;
        bodySlapStart = false;
        playerState.BodySlapping = false;
    }
    private void CheckBodySlapTime()
    {
        if (bodySlapTimer < bodySlapTime)
        {
            bodySlapTimer += Time.deltaTime;

            if (bodySlapTimer >= bodySlapTime)
            {
                stopBodySlapTimer = stopBodySlapTime;
                bodyStopBodySlapTimerStart = false;
            }
        }
    }
    private void CheckStopBodySlapTime()
    {
        if (stopBodySlapTimer > 0f)
        {
            stopBodySlapTimer -= Time.deltaTime;

            rigid.AddForce(Vector2.Lerp(Vector2.zero, -rigid.velocity / stopBodySlapOffset, stopBodySlapTimer / stopBodySlapTime));

            childRigids.ForEach(x => x.AddForce(Vector2.Lerp(Vector2.zero, -x.velocity / stopBodySlapOffset, stopBodySlapTimer / stopBodySlapTime)));

            if (stopBodySlapTimer <= 0f)
            {
                StopBodySlap();
            }
        }
    }
}
