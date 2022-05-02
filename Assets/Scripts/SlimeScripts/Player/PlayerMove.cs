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

    [SerializeField]
    private float bodyPointMovePower = 1f;
    [Header("�ٵ�����Ʈ�� localPosition�� x, y ������ ���� ó���� ��ġ���� �� �� �̻� �־����� ���Ѵ�.")]
    [SerializeField]
    private float maxBodyPointLocalPos = 0.1f;

    public override void Awake()
    {
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

        base.Awake();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!(playerState.BodySlapping || playerInput.IsPauseByCutScene || playerState.IsDrain))
        {
            Vector2 moveVec = playerInput.MoveVector * (playerStat.Speed);

            if (!(playerState.CantMove || playerState.CantChangeDir))
            {
                if (moveVec != Vector2.zero)
                {
                    lastMoveVec = moveVec;
                }
                else
                {
                    lastMoveVec = Vector2.Lerp(lastMoveVec, Vector2.zero, Time.fixedDeltaTime * playerStat.Speed / 2f); // �������� ������ �� ���ڱ� ���ߴ� ���� ����
                }

                rigid.velocity = lastMoveVec;

                // ������� ������ �ٵ�����Ʈ���� ������ ó��

                PlayerBodyPointMove(moveVec);
            }
            else if(!playerState.CantChangeDir)
            {
                rigid.velocity = Vector2.zero;
            }
        }
        else
        {
            lastMoveVec = Vector2.zero;
            rigid.velocity = Vector2.zero;
        }
    }
    private float GetMovePower(Transform pos)
    {
        float movePower = 0f;

        movePower = pos.localPosition.y;

        return movePower.Abs() * bodyPointMovePower;
    }
    public void PlayerBodyPointMove(Vector2 moveVec)
    {
        float movePower = 0f;

        float distance = 0f;

        foreach (var x in softBody.UpNotMiddlePoints)
        {
            if (x.IsWall || x.IsCrossWall)
            {
                continue;
            }

            movePower = GetMovePower(x.transform);

            if (moveVec != Vector2.zero)
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
                if (!x.IsWall)
                {
                    x.transform.localPosition = Vector2.Lerp(x.transform.localPosition, (Vector2)x.transform.localPosition + moveVec * movePower, Time.fixedDeltaTime);
                }
            }
        }

        foreach (var x in softBody.DownNotMiddlePoints)
        {
            if (x.IsWall || x.IsCrossWall)
            {
                continue;
            }

            movePower = GetMovePower(x.transform);

            if (moveVec != Vector2.zero)
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
                if (!x.IsWall)
                {
                    x.transform.localPosition = Vector2.Lerp(x.transform.localPosition, (Vector2)x.transform.localPosition - moveVec * movePower, Time.fixedDeltaTime);
                }
            }
        }
    }
}
