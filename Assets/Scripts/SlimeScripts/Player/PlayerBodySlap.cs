using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodySlap : PlayerAction
{
    private Player player = null;
    [SerializeField]
    private LayerMask canCrashLayer;
    [SerializeField]
    private Vector2 bodySlapMoveVec = Vector2.zero;

    [Header("돌진하기 전 살짝 뒤로 뺄 때의 속도")]
    [SerializeField]
    private float moveBackSpeed = 8f;
    [SerializeField]
    private float bodySlapMovePower = 2f;

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

    public override void Start()
    {
        base.Start();
        player = GetComponent<Player>();

        SlimeEventManager.StartListening("BodyPointCrash", BodyPointCrash);
    }

    void Update()
    {
        if (playerStatus.BodySlapping && !bodySlapStart)
        {
            bodySlapStart = true;

            bodySlapMoveVec = playerInput.MoveVector;

            rigid.velocity = -bodySlapMoveVec * moveBackSpeed;
            childRigids.ForEach(x => x.velocity = -bodySlapMoveVec * moveBackSpeed);

            bodySlapTimer = bodySlapTime;
        }

        CheckBodySlapTime();
        CheckStopBodySlapTime();
    }
    private void FixedUpdate()
    {
        if (playerStatus.BodySlapping && bodySlapStart) // 움직임
        {
            rigid.AddForce(bodySlapMoveVec * bodySlapMovePower);

            childRigids.ForEach(x => x.AddForce(Vector2.Lerp(bodySlapMoveVec, bodySlapMoveVec * bodySlapMovePower, Time.fixedDeltaTime)));
        }
    }
    private void OnDisable()
    {
        SlimeEventManager.StopListening("BodyPointCrash", BodyPointCrash);
    }
    private void BodyPointCrash(GameObject targetObject) // BodyPoint가 특정 오브젝트와 충돌했을 때 호출
    {
        if (canCrashLayer.CompareGameObjectLayer(targetObject) && playerStatus.BodySlapping)
        {
            Enemy.Enemy enemy = targetObject.GetComponent<Enemy.Enemy>();

            if (enemy != null)
            {
                enemy.GetDamage(player.BodySlapDamage);
            }

            stopBodySlapTimer = stopBodySlapTime;
        }
    }
    private void StopBodySlap()
    {
        stopBodySlapTimer = 0f;

        bodySlapStart = false;
        playerStatus.BodySlapping = false;
    }
    private void CheckBodySlapTime()
    {
        if (bodySlapTimer > 0f)
        {
            bodySlapTimer -= Time.deltaTime;

            if (bodySlapTimer <= 0f)
            {
                stopBodySlapTimer = stopBodySlapTime;
            }
        }
    }
    private void CheckStopBodySlapTime()
    {
        
        if(stopBodySlapTimer > 0f)
        {
            stopBodySlapTimer -= Time.deltaTime;
            
            rigid.AddForce(Vector2.Lerp(Vector2.zero, -rigid.velocity / stopBodySlapOffset, stopBodySlapTimer / stopBodySlapTime));

            childRigids.ForEach(x => x.AddForce(Vector2.Lerp(Vector2.zero, -x.velocity / stopBodySlapOffset, stopBodySlapTimer / stopBodySlapTime)));

            if(stopBodySlapTimer <= 0f)
            {
                StopBodySlap();
            }
        }
    }
}
