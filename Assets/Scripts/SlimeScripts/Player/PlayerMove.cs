using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    private Stat playerStat = null;
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

            rigid.velocity = MoveVec;

            childRigids.ForEach(x => x.velocity = Vector2.Lerp(x.velocity, MoveVec * 0.8f, Time.deltaTime));
        }
    }
}
