using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    private Stat playerStat = null;
    public override void Start()
    {
        playerStat = SlimeGameManager.Instance.PlayerStat;

        base.Start();
    }
    
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!playerStatus.BodySlapping)
        {
            Vector2 MoveVec = playerInput.MoveVector * (playerStat.eternalStat.speed + playerStat.additionalEternalStat.speed);

            rigid.velocity = MoveVec;

            childRigids.ForEach(x => x.velocity = Vector2.Lerp(x.velocity, MoveVec * 0.8f, Time.deltaTime));
        }
    }
}
