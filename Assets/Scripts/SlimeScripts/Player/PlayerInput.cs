using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerStatus playerStatus = null;

    private Vector2 moveVector = Vector2.zero;
    public Vector2 MoveVector
    {
        get { return moveVector; }
    }

    private bool isBodySlap = false;
    public bool IsBodySlap
    {
        get { return isBodySlap; }
        set { isBodySlap = value; }
    }

    private bool isShoot = false;
    public bool IsShoot
    {
        get { return isShoot; }
        set { isShoot = value; }
    }

    private void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }

    void Update()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        moveVector.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("BodySlap")) // left shift
        {
            isBodySlap = true;
        }

        if(Input.GetButtonDown("Shoot")) // left ctrl
        {
            isShoot = true;
        }
    }
    private void FixedUpdate()
    {
        if (isBodySlap)
        {
            playerStatus.BodySlapping = true;
            isBodySlap = false;
        }
    }
}
