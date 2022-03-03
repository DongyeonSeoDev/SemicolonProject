using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    private Vector2 lastMoveVec = Vector2.zero;

    private Stat playerStat = null;

    //private List<Rigidbody2D> leftChildRigids = new List<Rigidbody2D>();
    //private List<Rigidbody2D> rightChildRigids = new List<Rigidbody2D>();
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
        float distance = 0f;

        childRigids.ForEach(r => {

            distance = Vector2.Distance(r.position, rigid.position);

            if (r.position.x > rigid.position.x)
            {
                if(distance > middleToRightestDistance)
                {
                    middleToRightestDistance = distance;
                    rightestRigid = r;
                }
            }
            else if (r.position.x < rigid.position.x)
            {
                if(distance > middleToLeftestDistance)
                {
                    distance = middleToLeftestDistance;
                    leftestRigid = r;
                }
            }

            if (r.position.y > rigid.position.y)
            {
                upChildRigids.Add(r);
            }
            else if(r.position.y < rigid.position.y)
            {
                downChildRigids.Add(r);
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

            float movePower = 0f;

            upChildRigids.ForEach(x => 
            {
                movePower = Vector2.Distance(x.position, rigid.position);

                if(isRightChildRigid(x))
                {
                    movePower = (middleToRightestDistance - movePower).Abs();
                }
                else
                {
                    movePower = (middleToLeftestDistance - movePower).Abs(); 
                }

                x.velocity = MoveVec * movePower;
            });

            downChildRigids.ForEach(x => x.velocity = -MoveVec * playerStat.Speed);
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
