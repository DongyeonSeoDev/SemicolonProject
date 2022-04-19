using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPoint : MonoBehaviour
{
    private Rigidbody2D rigid = null;

    private MiddlePoint middlePoint = null;
    private BodyPointCrashCheckCollider bodyPointCrashCheckCollider = null;

    [SerializeField]
    private LayerMask whatIsWall;

    [SerializeField]
    private SoundBox whenDrainBodyPointMoveSound = null;

    [SerializeField]
    private float returnToOriginSpeed = 2f;
    [SerializeField]
    private float moveToMiddleSpeed = 1f;
    [SerializeField]
    private float farByMiddleSpeed = 1f;

    [SerializeField]
    private float moveToOriginTime = 1f;
    private float moveToOriginTimer = 0f;

    [SerializeField]
    private float moveToMiddleTime = 1f;
    private float moveToMiddleTimer = 0f;

    private bool isFarByPlayerByDrain = false;
    //private float returnToPlayerPosUpdateTime = 0f;

    private Vector2 farMaxPos = Vector2.zero;

    private float farByMiddleTime = 1f;
    private float farByMiddleTimer = 0f;

    private Vector2 originLocalPosition = Vector2.zero;
    public Vector2 OriginLocalPosition
    {
        get { return originLocalPosition; }
    }

    #region 상태관련 변수들
    [SerializeField]
    private bool isMiddlePoint = false;

    private bool isMoveToOriginASec = false;

    private bool isWall = false;
    public bool IsWall
    {
        get { return isWall; }
        set { isWall = value; }
    }

    private bool isUpWall = false;
    public bool IsUpWall
    {
        get { return isUpWall; }
        set { isUpWall = value; }
    }

    private bool isCrossWall = false;
    public bool IsCrossWall
    {
        get { return isCrossWall; }
    }

    private bool isMove = false;
    public bool IsMove
    {
        get { return isMove; }
        set { isMove = value; }
    }

    private bool isMoveToMiddle = false;
    public bool IsMoveToMiddle
    {
        get { return isMoveToMiddle; }
        set { isMoveToMiddle = value; }
    }

    private bool isFarByMiddle = false;
    public bool IsFarByMiddle
    {
        get { return isFarByMiddle; }
        set { isFarByMiddle= value; }
    }

    private bool farByMiddleMax = false;

    private bool isDownBodyPoint = false;
    #endregion
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        originLocalPosition = transform.localPosition;
        farByMiddleTimer = farByMiddleTime;

        if (!isMiddlePoint)
        {
            middlePoint = transform.parent.GetComponent<MiddlePoint>();
        }

        bodyPointCrashCheckCollider = transform.GetComponentInChildren<BodyPointCrashCheckCollider>();

        if (isDownBodyPoint)
        {
            StopListenings();
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening("StartNextStage", ResetWallBoolean);

        if (!isMiddlePoint)
        {
            EventManager.StartListening("SetDrainTime", (Action<float>)PlayerDrain);
            EventManager.StartListening("PlayerShoot", SetMoveToMiddleTimer);
            EventManager.StartListening("PlayerCharging", SetMoveToMiddleTimer);
            EventManager.StartListening("PlayerBodySlap", (Action<float>)PlayerBodySlap);
        }
    }
    
    private void OnDisable()
    {
        ResetBodyPoint();
        StopListenings();

        if(!isMiddlePoint)
        {
            EventManager.StopListening("SetDrainTime", (Action<float>)PlayerDrain);
        }
    }
    public void SetTrueisDownBodyPoint()
    {
        isDownBodyPoint = true;
    }
    private void StopListenings()
    {
        EventManager.StopListening("StartNextStage", ResetWallBoolean);

        if (!isMiddlePoint)
        {
            EventManager.StopListening("PlayerShoot", SetMoveToMiddleTimer);
            EventManager.StopListening("PlayerCharging", SetMoveToMiddleTimer);
            EventManager.StopListening("PlayerBodySlap", (Action<float>)PlayerBodySlap);
        }
    }

    private void Update()
    {
        MoveToOriginPos();

        if (!isMiddlePoint)
        {
            CheckCrossWall();
            MoveToMiddleTimerCheck();
            FarByMiddleTimerCheck();
            MoveToOriginASec();
        }
    }
    private void FixedUpdate()
    {
        CheckWall();
    }
    private void CheckWall()
    {
        Ray2D ray = new Ray2D();
        RaycastHit2D hit = new RaycastHit2D();

        ray.origin = transform.position;
        ray.direction = transform.up;

        hit = Physics2D.Raycast(ray.origin, ray.direction, bodyPointCrashCheckCollider.Col.radius, whatIsWall);

        isUpWall = hit;

    }
    private void MoveToOriginPos()
    {
        if (((!(isWall) || isMove)
            && !(isMiddlePoint || isMoveToMiddle)) || isUpWall)
        {
            if (isFarByMiddle)
            {
                return;
            }

            if (Vector2.Distance(transform.localPosition, originLocalPosition) > 0.01f)
            {
                transform.localPosition = Vector2.Lerp(transform.localPosition, originLocalPosition, Time.deltaTime * returnToOriginSpeed);
            }
            else
            {
                transform.localPosition = originLocalPosition;
            }
        }
    }
    private void MoveToOriginASec()
    {
        if(moveToOriginTimer > 0f)
        {
            isMoveToOriginASec = true;
            moveToOriginTimer -= Time.deltaTime;

            transform.localPosition = Vector2.Lerp(transform.localPosition, originLocalPosition, Time.deltaTime * returnToOriginSpeed);

            if(moveToOriginTimer <= 0f)
            {
                isMoveToOriginASec = false;
                moveToOriginTimer = 0f;

                transform.localPosition = originLocalPosition;
            }
        }
    }
    private void CheckCrossWall()
    {
        Ray2D ray;
        RaycastHit2D hit;
        float distance = 0f;

        ray = new Ray2D(transform.position, (middlePoint.transform.position - transform.position).normalized);
        distance = Vector2.Distance(transform.position, middlePoint.transform.position);

        hit = Physics2D.Raycast(ray.origin, ray.direction, distance, whatIsWall);

        if (hit)
        {
            isCrossWall = true;
            moveToMiddleTimer = 0.1f;
        }
        else
        {
            isCrossWall = false;
        }
    }
    private void SetMoveToMiddleTimer()
    {
        moveToMiddleTimer = moveToMiddleTime;
    }
    private void PlayerBodySlap(float bodySlapTime)
    {
        moveToMiddleTimer = bodySlapTime;
    }
    private void PlayerDrain(float drainTime)
    {
        isFarByPlayerByDrain = true;

        StartFarByMiddleTimer(drainTime);
    }

    private void StartFarByMiddleTimer(float drainTime)
    {
        farByMiddleTime = drainTime;
        farByMiddleMax = false;
        farByMiddleTimer = 0f;
    }

    private void MoveToMiddleTimerCheck()
    {
        if (moveToMiddleTimer > 0f)
        {
            moveToMiddleTimer -= Time.deltaTime;

            if (moveToMiddleTimer <= 0f)
            {
                isMoveToMiddle = false;

                moveToMiddleTimer = 0f;

                return;
            }
           
            isMoveToMiddle = true;

            MoveToMiddle();
        }
    }
    private void MoveToMiddle()
    {
        if(isMoveToOriginASec || isFarByMiddle)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, middlePoint.transform.position);

        if (distance >= middlePoint.MinDisWithBodyPoints)
        {
            if (!isUpWall)
            {
                transform.position = Vector2.Lerp(transform.position, middlePoint.transform.position, Time.deltaTime * moveToMiddleSpeed);
            }
        }
        else
        {
            moveToOriginTimer = moveToOriginTime;
        }

        CheckCrossWall();
    }
    private void FarByMiddleTimerCheck()
    {
        if(farByMiddleTimer < farByMiddleTime)
        {
            farByMiddleTimer += Time.deltaTime;

            if(farByMiddleTimer  >= farByMiddleTime)
            {
                isFarByMiddle = false;
                farByMiddleMax = false;
                isFarByPlayerByDrain = false;

                moveToOriginTimer = moveToMiddleTime;
                farByMiddleTimer = farByMiddleTime;

                DrainEffect();

                return;
            }

            isFarByMiddle = true;

            if (farByMiddleMax)
            {
                ReturnToMiddleWhenDrain();

                return;
            }

            FarByMiddle();
        }
    }
    private void FarByMiddle()
    {
        Vector3 dir = (transform.position - middlePoint.transform.position).normalized;
        float distance = Vector2.Distance(transform.position, middlePoint.transform.position);

        if(distance < middlePoint.MaxDisWithBodyPoints)
        {
            transform.position = Vector2.Lerp(transform.position, transform.position + dir * farByMiddleSpeed  * farByMiddleTime, farByMiddleTimer / farByMiddleTime);
        }
        else if(isFarByPlayerByDrain)
        {
            farByMiddleMax = true;
            farMaxPos = transform.position;
        }
    }
    private void ReturnToMiddleWhenDrain()
    {
        if (farByMiddleTimer % middlePoint.PlayerDrain.PlayerDrainCol.DrainMoveUpdateTIme <= 0.1f)
        {
            DrainEffect();

            if (farByMiddleMax && isFarByPlayerByDrain)
            {
                transform.position = Vector2.Lerp(farMaxPos, middlePoint.transform.position, farByMiddleTimer / farByMiddleTime);
            }
        }
        else
        {
            middlePoint.DrainEffectSpawned = false;
        }
    }

    private void DrainEffect()
    {
        if (!middlePoint.DrainEffectSpawned)
        {
            middlePoint.DrainEffectSpawned = true;
            EventManager.TriggerEvent("SpawnAfterImageSoftBody");
            SoundManager.Instance.PlaySoundBox(whenDrainBodyPointMoveSound);
        }
    }

    private void ResetBodyPoint()
    {
        if (!isMiddlePoint)
        {
            transform.localPosition = originLocalPosition;
        }

        ResetWallBoolean();

        isFarByPlayerByDrain = false;
        isMoveToOriginASec = false;
        isMoveToMiddle = false;
        isFarByMiddle = false;
        farByMiddleMax = false;

        moveToMiddleTimer = 0f;
        moveToOriginTimer = 0f;

        farByMiddleTimer = farByMiddleTime;
    }
    private void ResetWallBoolean()
    {
        isWall = false;
        isUpWall = false;
        isCrossWall = false;
    }
}
