using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerState playerState = null;

    private Vector2 moveVector = Vector2.zero;
    public Vector2 MoveVector
    {
        get { return moveVector; }
    }
    private Vector2 lastMoveVector = Vector2.zero;
    public Vector2 LastMoveVector
    {
        get { return lastMoveVector; }
    }

    private bool isBodySlap = false;
    public bool IsBodySlap
    {
        get { return isBodySlap; }
        set { isBodySlap = value; }
    }

    private bool isInteraction = false;
    public bool IsInterraction
    {
        get { return isInteraction; }
        set { isInteraction = value; }
    }

    private bool isShoot = false;
    public bool IsShoot
    {
        get { return isShoot; }
        set { isShoot = value; }
    }

    private bool isDrain = false;
    public bool IsDrain
    {
        get { return isDrain; }
        set { isDrain = value; }
    }

    private void Start()
    {
        playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        if (!playerState.IsDead)
        {
            moveVector.x = Input.GetAxisRaw("Horizontal");
            moveVector.y = Input.GetAxisRaw("Vertical");

            if (moveVector != Vector2.zero)
            {
                lastMoveVector = moveVector;
            }

            if (Input.GetButtonDown("BodySlap")) // left shift
            {
                isBodySlap = true;
            }

            if (Input.GetButtonDown("Shoot")) // left ctrl
            {
                isShoot = true;
            }

            if (Input.GetButtonDown("Drain"))
            {
                isDrain = true;
            }

            if (Input.GetButtonDown("Interaction"))
            {
                isInteraction = true;
            }
        }
        else
        {
            moveVector = Vector2.zero;

            isBodySlap = false;
            isShoot = false;
            isDrain = false;
            isInteraction = false;
        }
    }
    private void FixedUpdate()
    {
        if (isBodySlap)
        {
            playerState.BodySlapping = true;
            isBodySlap = false;
        }
    }
}
