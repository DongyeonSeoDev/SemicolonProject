using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    private Vector2 lastMoveVec = Vector2.zero;

    private Stat playerStat = null;

    //private List<Rigidbody2D> leftChildRigids = new List<Rigidbody2D>();
    //private List<Rigidbody2D> rightChildRigids = new List<Rigidbody2D>();
    private Rigidbody2D rightestRigid = new Rigidbody2D(); // ������ ���� ��ġ�� �ٵ�����Ʈ
    private float middleToRightestDistance = 0f; // Rightest�� Middle�� �Ÿ�

    private Rigidbody2D leftestRigid = new Rigidbody2D(); // ���� ���� ��ġ�� �ٵ�����Ʈ
    private float middleToLeftestDistance = 0f; // Leftest�� Middle�� �Ÿ�

    private List<Rigidbody2D> upChildRigids = new List<Rigidbody2D>();
    private List<Rigidbody2D> downChildRigids = new List<Rigidbody2D>();

    // �� �ٵ�����Ʈ���� �������� �̵��Ѵٸ� ���� ���� �ٵ�����Ʈ���� �Ÿ��� ����� ���� ��� �������� �̵��Ѵ�.
    // �� ���κ��� ����Ʈ���� �������� �ʴ´�.
    // �ٸ� �ٴڸ��� ����Ʈ���� �ȿ����δ�.

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
