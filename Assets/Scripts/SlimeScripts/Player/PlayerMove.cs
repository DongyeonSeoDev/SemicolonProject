using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerAction
{

    [SerializeField]
    private float speed = 0f;

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
            Vector2 MoveVec = playerInput.MoveVector * speed;

            rigid.velocity = MoveVec;

            childRigids.ForEach(x => x.velocity = Vector2.Lerp(x.velocity, -MoveVec * 2f, Time.deltaTime));
        }
    }
}
