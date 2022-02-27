using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodySlap : PlayerAction
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
    private float DamageMagnificationOfBodySlap = 2f;

    [Header("돌진하기 전 살짝 뒤로 뺄 때의 속도")]
    [SerializeField]
    private float moveBackSpeed = 8f;
    [SerializeField]
    private float bodySlapMoveSpeed = 2f;

    [SerializeField]
    private float bodySlapDelay = 5f;
    private float bodySlapDelayTimer = 0f;
    public float BodySlapDelayTimer
    {
        get { return bodySlapDelayTimer; }
    }
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
    private void OnEnable()
    {
        EventManager.StartListening("BodyPointCrash", BodyPointCrash);
        EventManager.StartListening("PlayerDead", StopBodySlap);
    }
    void Update()
    {
        if (playerState.BodySlapping && !bodySlapStart && canBodySlap)
        {
            bodySlapStart = true;

            bodySlapMoveVec = playerInput.LastMoveVector;

            currentBodySlapTime = bodySlapTime;

            moveOriginPos = transform.position;
            moveTargetPos = moveOriginPos + bodySlapTime * bodySlapMoveSpeed * bodySlapMoveVec;
            moveTargetPos = PosCantCrossWall(canCrashLayer, moveOriginPos, moveTargetPos);

            currentBodySlapTime = Vector2.Distance(moveOriginPos, moveTargetPos) / bodySlapMoveSpeed;

            EventManager.TriggerEvent("PlayerBodySlap", bodySlapTime);

            bodySlapTimer = 0f;
            bodySlapDelayTimer = bodySlapDelay;
        }
        else if (!canBodySlap)
        {
            playerState.BodySlapping = false;
        }

        CheckBodySlapDelay();
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
    private void OnDisable()
    {
        EventManager.StopListening("BodyPointCrash", BodyPointCrash);
        EventManager.StopListening("PlayerDead", StopBodySlap);
    }
    private void BodyPointCrash(GameObject targetObject) // BodyPoint가 특정 오브젝트와 충돌했을 때 호출
    {
        if (canCrashLayer.CompareGameObjectLayer(targetObject) && playerState.BodySlapping)
        {
            Enemy.Enemy enemy = targetObject.GetComponent<Enemy.Enemy>();

            if (enemy != null)
            {
                enemy.GetDamage((int)(playerStat.Damage * DamageMagnificationOfBodySlap), true);
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
    private void StopBodySlap()
    {
        stopBodySlapTimer = 0f;
        bodySlapDelayTimer = bodySlapDelay;

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
    private void CheckBodySlapDelay()
    {
        if (bodySlapDelayTimer > 0f)
        {
            bodySlapDelayTimer -= Time.deltaTime;

            if (bodySlapDelayTimer <= 0f)
            {
                canBodySlap = true;
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
    private Vector2 PosCantCrossWall(LayerMask wallLayer, Vector2 startPos, Vector2 targetPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, (targetPos - startPos).normalized, Vector2.Distance(startPos, targetPos), wallLayer);

        if (hit)
        {
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                return hit.point - (targetPos - startPos).normalized * (Vector2.Distance(startPos, targetPos) / 10f);
            }
            else
            {
                return hit.point;
            }
        }
        else
        {
            return targetPos;
        }
    }
}
