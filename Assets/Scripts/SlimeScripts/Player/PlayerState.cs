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

    private bool isStop = false;
    public bool IsStop
    {
        get { return isStop; }
        set { isStop = value; }
    }

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

    private bool charging = false;
    public bool Chargning
    {
        get { return charging; }
        set { charging = value; }
    }

    private bool isDrain = false;
    public bool IsDrain
    {
        get { return isDrain; }
        set { isDrain = value; }
    }

    private bool isKnockBack = false;
    public bool IsKnockBack
    {
        get { return isKnockBack; }
        set { isKnockBack = value; }
    }

    private bool isSturn = false;
    public bool IsSturn
    {
        get { return isSturn; }
        set { isSturn = value; }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", PlayerReset);
        EventManager.StartListening("ChangeBody", PlayerReset);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", PlayerReset);
        EventManager.StopListening("ChangeBody", PlayerReset);
    }

    void Update()
    {
        LastMovingPointSet();
    }

    private void LastMovingPointSet()
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
    public void PlayerReset()
    {
        charging = false;
        bodySlapping = false;
        isKnockBack = false;
        isSturn = false;
        isDrain = false;
    }
}
