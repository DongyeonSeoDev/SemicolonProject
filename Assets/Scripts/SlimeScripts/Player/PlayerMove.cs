using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    private Vector2 lastMoveVec = Vector2.zero;
    public Vector2 LastMoveVec
    {
        get { return lastMoveVec; }
    }

    private Stat playerStat = null;

    [Header("바디포인트의 localPosition의 x, y 각각의 값은 처음의 위치에서 이 값 이상 멀어지지 못한다.")]
    [SerializeField]
    private float maxBodyPointLocalPos = 0.1f;

    public override void Awake()
    {
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

        base.Awake();
    }
    //private void Start()
    //{
    //    SetRigids();
    //}

    //private void SetRigids()
    //{
    //    float distance = 0f;
    //    BodyPoint bodyPoint = null;

    //    foreach(var r in childRigids)
    //    {
    //        bodyPoint = r.GetComponent<BodyPoint>();

    //        if (bodyPoint == null)
    //        {
    //            continue;
    //        }

    //        distance = Vector2.Distance(r.position, rigid.position);

    //        if (r.position.y > rigid.position.y)
    //        {
    //            upChildPoints.Add(bodyPoint);
    //        }
    //        else if (r.position.y < rigid.position.y)
    //        {
    //            downChildPoints.Add(bodyPoint);
    //        }
    //    }
    //}

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
                lastMoveVec = Vector2.Lerp(lastMoveVec, Vector2.zero, Time.fixedDeltaTime * playerStat.Speed / 2f); // 움직임이 멈췄을 때 갑자기 멈추는 것을 방지
            }

            rigid.velocity = lastMoveVec;

            // 여기부턴 슬라임 바디포인트들의 움직임 처리

            float movePower = 0f;

            float distance = 0f;

            softBody.UpNotMiddlePoints.ForEach(x => 
            {
                movePower = GetMovePower(x.transform);

                if (MoveVec != Vector2.zero)
                {
                    x.IsMove = true;
                }
                else
                {
                    x.IsMove = false;
                }

                distance = Vector2.Distance(x.transform.localPosition, x.OriginLocalPosition);

                if (distance < maxBodyPointLocalPos)
                {
                    x.transform.localPosition = Vector2.Lerp(x.transform.localPosition, (Vector2)x.transform.localPosition + MoveVec * movePower, Time.fixedDeltaTime);
                }
            });

            foreach(var x in softBody.DownNotMiddlePoints)
            {
                movePower = GetMovePower(x.transform);
                if (MoveVec != Vector2.zero)
                {
                    x.IsMove = true;
                }
                else
                {
                    x.IsMove = false;
                }

                distance = Vector2.Distance(x.transform.localPosition, x.OriginLocalPosition);

                if (distance < maxBodyPointLocalPos)
                {
                    x.transform.localPosition = Vector2.Lerp(x.transform.localPosition, (Vector2)x.transform.localPosition - MoveVec * movePower, Time.fixedDeltaTime);
                }
            }
        }
        else
        {
            lastMoveVec = Vector2.zero;
        }
    }
    private float GetMovePower(Transform pos)
    {
        float movePower = 0f;

        movePower = pos.localPosition.y;

        return movePower.Abs();
    }
}
