using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    private Vector2 lastMoveVec = Vector2.zero;

    private Stat playerStat = null;

    //private List<Rigidbody2D> leftChildRigids = new List<Rigidbody2D>();
    //private List<Rigidbody2D> rightChildRigids = new List<Rigidbody2D>();
    private Rigidbody2D upestRigid = new Rigidbody2D();
    private float middleToUpestDistance = 0f;

    private Rigidbody2D downestRigid = new Rigidbody2D();
    private float middleToDownestDistance = 0f;

    private Rigidbody2D rightestRigid = new Rigidbody2D(); // 오른쪽 끝에 위치한 바디포인트
    private float middleToRightestDistance = 0f; // Rightest와 Middle의 거리

    private Rigidbody2D leftestRigid = new Rigidbody2D(); // 왼쪽 끝에 위치한 바디포인트
    private float middleToLeftestDistance = 0f; // Leftest와 Middle의 거리

    private List<Rigidbody2D> upChildRigids = new List<Rigidbody2D>();
    private List<Rigidbody2D> downChildRigids = new List<Rigidbody2D>();

    // 각 바디포인트들은 왼쪽으로 이동한다면 왼쪽 끝의 바디포인트와의 거리에 비례한 힘을 얻어 왼쪽으로 이동한다.
    // 각 끝부분의 포인트들은 움직이지 않는다.
    // 다만 바닥면의 포인트들은 안움직인다.

    [SerializeField]
    private float breakSpeed = 2f;

    public override void Awake()
    {
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

        base.Awake();
    }
    private void Start()
    {
        SetRigids();
    }

    private void SetRigids()
    {
        float distance = 0f;

        childRigids.ForEach(r =>
        {
            distance = Vector2.Distance(r.position, rigid.position);

            if (r.position.x > rigid.position.x)
            {
                if (distance > middleToRightestDistance)
                {
                    middleToRightestDistance = distance;
                    rightestRigid = r;
                }
            }
            else if (r.position.x < rigid.position.x)
            {
                if (distance > middleToLeftestDistance)
                {
                    middleToLeftestDistance = distance;
                    leftestRigid = r;
                }
            }

            if (r.position.y > rigid.position.y)
            {
                upChildRigids.Add(r);

                if(distance > middleToUpestDistance)
                {
                    middleToUpestDistance = distance;
                    upestRigid = r;
                }
            }
            else if (r.position.y < rigid.position.y)
            {
                downChildRigids.Add(r);

                if(distance > middleToDownestDistance)
                {
                    middleToDownestDistance = distance;
                    downestRigid = r;
                }
            }
        });
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!playerState.BodySlapping)
        {
            Vector2 MoveVec = playerInput.MoveVector * (playerStat.Speed);

            if(MoveVec != Vector2.zero)
            {
                lastMoveVec = MoveVec;
            }
            else
            {
                lastMoveVec = Vector2.Lerp(lastMoveVec, Vector2.zero, Time.fixedDeltaTime * playerStat.Speed / 2f);
            }

            rigid.velocity = lastMoveVec;

            // 여기부턴 슬라임 바디포인트들의 움직임 처리

            float movePower = 0f;

            upChildRigids.ForEach(x => 
            {
                if(isRightChildRigid(x))
                {
                    if (MoveVec.x > 0)
                    {
                        movePower = Vector2.Distance(x.position, leftestRigid.position);
                        movePower = (middleToRightestDistance - movePower).Abs();
                    }
                    else if(MoveVec.y != 0)
                    {
                        if(MoveVec.y > 0)
                        {
                            movePower = Vector2.Distance(x.position, upestRigid.position);
                            movePower = (middleToUpestDistance - movePower).Abs();
                        }
                        else
                        {
                            movePower = Vector2.Distance(x.position, downestRigid.position);
                            movePower = (middleToDownestDistance - movePower).Abs();
                        }
                    }
                }
                else
                {
                    if (MoveVec.x < 0)
                    {
                        movePower = Vector2.Distance(x.position, rightestRigid.position);
                        movePower = (middleToLeftestDistance - movePower).Abs();
                    }
                    else if (MoveVec.y != 0)
                    {
                        if (MoveVec.y > 0)
                        {
                            movePower = Vector2.Distance(x.position, upestRigid.position);
                            movePower = (middleToUpestDistance - movePower).Abs();
                        }
                        else
                        {
                            movePower = Vector2.Distance(x.position, downestRigid.position);
                            movePower = (middleToDownestDistance - movePower).Abs();
                        }
                    }
                }

                x.velocity = MoveVec * movePower;
            });

            //downChildRigids.ForEach(x => x.velocity = -new Vector2(MoveVec.x, x.velocity.y));
            downChildRigids.ForEach((x) => x.transform.localPosition = Vector2.Lerp(x.transform.localPosition, (Vector2)x.transform.localPosition - MoveVec, Time.fixedDeltaTime)); ;
        }
        else
        {
            lastMoveVec = Vector2.zero;
        }
    }
    private bool isRightChildRigid(Rigidbody2D r)
    {
        return r.position.x > rigid.position.x;
    }
}
