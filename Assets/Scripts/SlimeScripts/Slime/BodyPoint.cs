using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPoint : MonoBehaviour
{
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

    [SerializeField]
    private bool isMiddlePoint = false;

    private bool isWall = false;
    public bool IsWall
    {
        get { return isWall; }
    }

    private bool isMoveToMiddle = false;
    public bool IsMoveToMiddle
    {
        get { return isMoveToMiddle; }
        set { isMoveToMiddle = value; }
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
        SlimeEventManager.StartListening("BodyPointCrash", BodyPointCrash);
        SlimeEventManager.StartListening("PlayerShoot", PlayerShoot);
    }
    private void Update()
    {
        MoveToOriginPos();

        if (!isMiddlePoint)
        {
            MoveToMiddleTimerCheck();
        }
    }

    private void MoveToOriginPos()
    {
        if (!isWall && !isMiddlePoint && !isMoveToMiddle)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, originLocalPosition, Time.deltaTime * returnToOriginSpeed);
        }
    }

    private void OnDisable()
    {
        SlimeEventManager.StopListening("BodyPointCrash", BodyPointCrash);
        SlimeEventManager.StopListening("PlayerShoot", PlayerShoot);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (whatIsWall.CompareGameObjectLayer(other.gameObject))
        {
            isWall = true;
        }

        SlimeEventManager.TriggerEvent("BodyPointCrash", other.gameObject);
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (whatIsWall.CompareGameObjectLayer(other.gameObject))
        {
            isWall = false;
        }
    }
    private void BodyPointCrash(GameObject targetObject)
    {

    }
    private void PlayerShoot()
    {
        moveToMiddleTimer = moveToMiddleTime;
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
    }
}
