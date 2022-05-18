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
        set 
        {
            bodySlapping = value;
        }
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

    private bool cantMove = false; 
    public bool CantMove
    {
        get { return cantMove; }
        set { cantMove = value; }
    }

    private bool cantChangeDir = false;
    public bool CantChangeDir
    {
        get { return cantChangeDir; }
        set { cantChangeDir = value; }
    }

    private bool isStun = false; // 외부(적, 장해물 등)에 의한 멈춤처리
    public bool IsStun
    {
        get { return isStun; }
        set { isStun = value; }
    }

    private bool isInMomentom = false;
    public bool IsInMomentom
    {
        get { return isInMomentom; }
        set { isInMomentom = value; }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", PlayerReset);
        EventManager.StartListening("ChangeBody", PlayerReset);
        EventManager.StartListening("PlayerStop", PlayerStop);
        EventManager.StartListening("PlayerStart", PlayerStart);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", PlayerReset);
        EventManager.StopListening("ChangeBody", PlayerReset);
        EventManager.StopListening("PlayerStop", PlayerStop);
        EventManager.StopListening("PlayerStart", PlayerStart);
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
        cantMove = false;
        isKnockBack = false;
        isStun = false;
        isDrain = false;
    }
    private void PlayerStop()
    {
        cantMove = true;
    }
    private void PlayerStart()
    {
        cantMove = false;
    }
}
