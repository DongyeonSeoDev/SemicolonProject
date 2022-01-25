using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{
    public override void Start()
    {
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
            Vector2 MoveVec = playerInput.MoveVector * SlimeGameManager.Instance.PlayerStat.eternalStat.speed;

            rigid.velocity = MoveVec;

            childRigids.ForEach(x => x.velocity = Vector2.Lerp(x.velocity, MoveVec * 0.8f, Time.deltaTime));
        }
    }
}
