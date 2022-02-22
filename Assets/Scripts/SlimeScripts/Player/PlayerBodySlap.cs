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

    [Header("돌진하기 전 살짝 뒤로 뺄 때의 속도")]
    [SerializeField]
    private float moveBackSpeed = 8f;
    [SerializeField]
    private float bodySlapMoveSpeed = 2f;

    [SerializeField]
    private float bodySlapTime = 3f;
    private float bodySlapTimer = 0f;

    [Header("BodySlap이 완전히 멈출 때 까지 걸리는 시간")]
    [SerializeField]
    private float stopBodySlapTime = 2f;
    private float stopBodySlapTimer = 0f;

    [Header("BodySlap이 멈출 때 적용되는 관성 관련 값 (값에 따라 서서히 멈추는 정도가 달라짐)")]
    [SerializeField]
    private float stopBodySlapOffset = 3f;

    private bool bodySlapStart = false;
    private bool bodyStopBodySlapTimerStart = false;

    public override void Awake()
    {
        base.Awake();
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

    }
    private void OnEnable()
    {
        EventManager.StartListening("BodyPointCrash", BodyPointCrash);
        EventManager.StartListening("PlayerDead", StopBodySlap);
    }
    void Update()
    {
        if (playerState.BodySlapping && !bodySlapStart)
        {
            bodySlapStart = true;

            bodySlapMoveVec = playerInput.LastMoveVector;

            moveOriginPos = transform.position;
            moveTargetPos = moveOriginPos + bodySlapTime * bodySlapMoveSpeed * bodySlapMoveVec;
            moveTargetPos = PosCantCrossWall(canCrashLayer, moveOriginPos, moveTargetPos);

            EventManager.TriggerEvent("PlayerBodySlap", bodySlapTime);

            // rigid.velocity = -bodySlapMoveVec * moveBackSpeed;
            // childRigids.ForEach(x => x.velocity = -bodySlapMoveVec * moveBackSpeed);

            bodySlapTimer = 0f;
        }

        CheckBodySlapTime();
        CheckStopBodySlapTime();
    }
    private void FixedUpdate()
    {
        if (playerState.BodySlapping && bodySlapStart && !bodyStopBodySlapTimerStart) // 움직임
        {
            // rigid.AddForce(bodySlapMoveVec * bodySlapMovePower);

            // childRigids.ForEach(x => x.AddForce(Vector2.Lerp(bodySlapMoveVec, bodySlapMoveVec * bodySlapMovePower, Time.fixedDeltaTime)));

            transform.position = Vector2.Lerp(moveOriginPos, moveTargetPos, bodySlapTimer / bodySlapTime);

            Debug.Log(bodySlapTimer);
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
                enemy.GetDamage(playerStat.Damage);
            }

            if (!bodyStopBodySlapTimerStart)
            {
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

        // rigid.velocity = Vector2.zero;
        // childRigids.ForEach(x => x.velocity = Vector2.zero);

        bodySlapStart = false;
        playerState.BodySlapping = false;
    }
    private void CheckBodySlapTime()
    {
        if (bodySlapTimer < bodySlapTime)
        {
            bodySlapTimer += Time.deltaTime;

            if (bodySlapTimer > bodySlapTime)
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

            // rigid.AddForce(Vector2.Lerp(Vector2.zero, -rigid.velocity / stopBodySlapOffset, stopBodySlapTimer / stopBodySlapTime));

            // childRigids.ForEach(x => x.AddForce(Vector2.Lerp(Vector2.zero, -x.velocity / stopBodySlapOffset, stopBodySlapTimer / stopBodySlapTime)));

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
            bodySlapTime = Vector2.Distance(moveOriginPos, moveTargetPos) / bodySlapMoveSpeed;

            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                return hit.point - (targetPos - startPos).normalized * (Vector2.Distance(startPos, targetPos) / 6f);
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
