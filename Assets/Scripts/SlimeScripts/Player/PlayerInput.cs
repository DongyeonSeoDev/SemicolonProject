using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private Vector2 mousePosition = Vector2.zero;
    public Vector2 MousePosition
    {
        get { return mousePosition; }
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
    private bool isPause = false;

    private void Start()
    {
        playerState = GetComponent<PlayerState>();

        lastMoveVector = Vector2.left;
    }
    private void OnEnable()
    {
        EventManager.StartListening("TimePause", TimePause);
        EventManager.StartListening("TimeResume", TimeResume);
    }
    private void OnDisable()
    {
        EventManager.StopListening("TimePause", TimePause);
        EventManager.StopListening("TimeResume", TimeResume);
    }

    void Update()
    {
        if (!playerState.IsDead && !isPause)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            moveVector.x = Input.GetAxisRaw("Horizontal");
            moveVector.y = Input.GetAxisRaw("Vertical");

            moveVector = moveVector.normalized;

            if (moveVector != Vector2.zero)
            {
                lastMoveVector = moveVector;
            }

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK])) // left shift
            {
                isBodySlap = true;
            }

            if (Input.GetButtonDown("Shoot") && !EventSystem.current.IsPointerOverGameObject()) // mouse 0
            {
                isShoot = true;
            }

            if (Input.GetButtonDown("Drain")) // q
            {
                isDrain = true;
            }

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.INTERACTION])) // e
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
    private void TimePause()
    {
        isPause = true;
    }
    private void TimeResume()
    {
        isPause = false;
    }
}
