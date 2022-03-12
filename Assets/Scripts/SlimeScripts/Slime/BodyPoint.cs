using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPoint : MonoBehaviour
{
    private Rigidbody2D rigid = null;

    private MiddlePoint middlePoint = null;

    [SerializeField]
    private LayerMask whatIsWall;

    [SerializeField]
    private float returnToOriginSpeed = 2f;
    [SerializeField]
    private float moveToMiddleSpeed = 1f;

    [SerializeField]
    private float moveToMiddleTime = 1f;
    private float moveToMiddleTimer = 0f;

    private Vector2 originLocalPosition = Vector2.zero;
    public Vector2 OriginLocalPosition
    {
        get { return originLocalPosition; }
    }

    [SerializeField]
    private bool isMiddlePoint = false;

    private bool isWall = false;
    public bool IsWall
    {
        get { return isWall; }
        set { isWall = value; }
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
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        originLocalPosition = transform.localPosition;

        if (!isMiddlePoint)
        {
            middlePoint = transform.parent.GetComponent<MiddlePoint>();
        }
    }
    private void OnEnable()
    {
         EventManager.StartListening("PlayerShoot", PlayerShoot);
         EventManager.StartListening("PlayerBodySlap", (Action<float>)PlayerBodySlap);
    }
    private void OnDisable()
    {
         StopListenings();
    }

    public void StopListenings()
    {
        EventManager.StopListening("PlayerShoot", PlayerShoot);
        EventManager.StopListening("PlayerBodySlap", (Action<float>)PlayerBodySlap);
    }

    private void Update()
    {
        MoveToOriginPos();

        if (!isMiddlePoint)
        {
            CheckCrossWall();
            MoveToMiddleTimerCheck();
        }
    }

    private void MoveToOriginPos()
    {
        if ((!isWall || isMove) && !isMiddlePoint && !isMoveToMiddle)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, originLocalPosition, Time.deltaTime * returnToOriginSpeed);
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
            moveToMiddleTimer = 0.1f;
            Debug.Log("앗 벽에 끼었수다. 지금 빼드리리다");
        }
    }
    private void PlayerShoot()
    {
        moveToMiddleTimer = moveToMiddleTime;
    }
    private void PlayerBodySlap(float bodySlapTime)
    {
        moveToMiddleTimer = bodySlapTime;
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
            }
            else
            {
                isMoveToMiddle = true;

                MoveToMiddle();
            }
        }
    }
    private void MoveToMiddle()
    {
        transform.position = Vector2.Lerp(transform.position, middlePoint.transform.position, Time.deltaTime * moveToMiddleSpeed);

        CheckCrossWall();
    }
}
