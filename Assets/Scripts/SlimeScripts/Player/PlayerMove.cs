using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    Vector2 lastMoveVec = Vector2.zero;
    private Stat playerStat = null;

    [SerializeField]
    private float breakSpeed = 2f;

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

            Debug.Log(rigid.velocity);

            childRigids.ForEach(x => x.velocity = Vector2.Lerp(x.velocity, MoveVec * 0.8f, Time.fixedDeltaTime));
        }
    }
}
