using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    private Vector2 lastMoveVec = Vector2.zero;

    private Stat playerStat = null;

    private BodyPoint upestPoint = null;
    private float middleToUpestDistance = 0f;

    private BodyPoint downestPoint = null;
    private float middleToDownestDistance = 0f;

    private BodyPoint rightestPoint = null; // ������ ���� ��ġ�� �ٵ�����Ʈ
    private float middleToRightestDistance = 0f; // Rightest�� Middle�� �Ÿ�

    private BodyPoint leftestPoint = null; // ���� ���� ��ġ�� �ٵ�����Ʈ
    private float middleToLeftestDistance = 0f; // Leftest�� Middle�� �Ÿ�

    private List<BodyPoint> upChildRigids = new List<BodyPoint>();
    private List<BodyPoint> downChildRigids = new List<BodyPoint>();

    // �� �ٵ�����Ʈ���� �������� �̵��Ѵٸ� ���� ���� �ٵ�����Ʈ���� �Ÿ��� ����� ���� ��� �������� �̵��Ѵ�.
    // �� ���κ��� ����Ʈ���� �������� �ʴ´�.
    // �ٸ� �ٴڸ��� ����Ʈ���� �ȿ����δ�.

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
        BodyPoint bodyPoint = null;

        foreach(var r in childRigids)
        {
            bodyPoint = r.GetComponent<BodyPoint>();

            if (bodyPoint == null)
            {
                continue;
            }

            distance = Vector2.Distance(r.position, rigid.position);

            if (r.position.x > rigid.position.x)
            {
                if (distance > middleToRightestDistance)
                {
                    middleToRightestDistance = distance;
                    rightestPoint = bodyPoint;
                }
            }
            else if (r.position.x < rigid.position.x)
            {
                if (distance > middleToLeftestDistance)
                {
                    middleToLeftestDistance = distance;
                    leftestPoint = bodyPoint;
                }
            }

            if (r.position.y > rigid.position.y)
            {
                upChildRigids.Add(bodyPoint);

                if (distance > middleToUpestDistance)
                {
                    middleToUpestDistance = distance;
                    upestPoint = r.GetComponent<BodyPoint>();
                }
            }
            else if (r.position.y < rigid.position.y)
            {
                downChildRigids.Add(bodyPoint);

                if (distance > middleToDownestDistance)
                {
                    middleToDownestDistance = distance;
                    downestPoint = bodyPoint;
                }
            }
        }
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

            // ������� ������ �ٵ�����Ʈ���� ������ ó��

            float movePower = 0f;

            upChildRigids.ForEach(x => 
            {
                if(isRightChildRigid(x))
                {
                    if (MoveVec.x > 0)
                    {
                        movePower = GetMovePower(x.transform.position, leftestPoint.transform.position, middleToRightestDistance);
                    }
                    else if(MoveVec.y != 0)
                    {
                        if(MoveVec.y > 0)
                        {
                            movePower = GetMovePower(x.transform.position, upestPoint.transform.position, middleToUpestDistance);
                        }
                        else
                        {
                            movePower = GetMovePower(x.transform.position, downestPoint.transform.position, middleToDownestDistance);
                        }
                    }
                }
                else
                {
                    if (MoveVec.x < 0)
                    {
                        movePower = GetMovePower(x.transform.position, rightestPoint.transform.position, middleToLeftestDistance);
                    }
                    else if (MoveVec.y != 0)
                    {
                        if (MoveVec.y > 0)
                        {
                            movePower = GetMovePower(x.transform.position, upestPoint.transform.position, middleToUpestDistance);
                        }
                        else
                        {
                            movePower = GetMovePower(x.transform.position, downestPoint.transform.position, middleToDownestDistance);
                        }
                    }
                }

                x.GetComponent<Rigidbody2D>().velocity = MoveVec * movePower;
            });

            //downChildRigids.ForEach(x => x.velocity = -new Vector2(MoveVec.x, x.velocity.y));
            downChildRigids.ForEach((x) => x.transform.localPosition = Vector2.Lerp(x.transform.localPosition, (Vector2)x.transform.localPosition - MoveVec, Time.fixedDeltaTime)); ;
        }
        else
        {
            lastMoveVec = Vector2.zero;
        }
    }
    private float GetMovePower(Vector2 pos1, Vector2 pos2, float maxDistanceOfDirection)
    {
        float movePower = 0f;

        movePower = Vector2.Distance(pos1, pos2);
        movePower = (maxDistanceOfDirection - movePower).Abs();

        return movePower;
    }
    private bool isRightChildRigid(BodyPoint r)
    {
        return r.transform.position.x > rigid.position.x;
    }
}
