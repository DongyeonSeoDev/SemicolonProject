using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public enum MovingState
    {
        left,
        right,
        up,
        down,
        leftUp,
        leftDown,
        rightUp,
        rightDown
    }
}
public class PlayerState : MonoBehaviour
{

    private State.MovingState lastPlayerMovingPoint;
    public State.MovingState LastPlayerMovingPoint
    {
        get { return lastPlayerMovingPoint; }
    }

    private PlayerInput playerInput = null;

    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    private bool bodySlapping = false;
    public bool BodySlapping
    {
        get { return bodySlapping; }
        set { bodySlapping = value; }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        float horizontal = playerInput.MoveVector.x;
        float vertical = playerInput.MoveVector.y;

        if (horizontal > 0f)
        {
            if (vertical > 0f)
            {
                lastPlayerMovingPoint = State.MovingState.rightUp;
            }
            else if (vertical < 0f)
            {
                lastPlayerMovingPoint = State.MovingState.rightDown;
            }
            else
            {
                lastPlayerMovingPoint = State.MovingState.right;
            }
        }
        else if (horizontal < 0f)
        {
            if (vertical > 0f)
            {
                lastPlayerMovingPoint = State.MovingState.leftUp;
            }
            else if (vertical < 0f)
            {
                lastPlayerMovingPoint = State.MovingState.leftDown;
            }
            else
            {
                lastPlayerMovingPoint = State.MovingState.left;
            }
        }
        else if (vertical > 0f)
        {
            lastPlayerMovingPoint = State.MovingState.up;
        }
        else if (vertical < 0f)
        {
            lastPlayerMovingPoint = State.MovingState.down;
        }
    }
}
