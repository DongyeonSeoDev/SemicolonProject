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
    private Vector2 attackMousePosition = Vector2.zero;
    public Vector2 AttackMousePosition
    {
        get { return attackMousePosition; }
        set { attackMousePosition = value; }
    }
    private bool isInteraction = false;
    public bool IsInterraction
    {
        get { return isInteraction; }
        set { isInteraction = value; }
    }

    private bool isDoSkill0 = false;
    public bool IsDoSkill0
    {
        get { return isDoSkill0; }
        set { isDoSkill0 = value; }
    }
    private bool isDoSkill1 = false;
    public bool IsDoSkill1
    {
        get { return isDoSkill1; }
        set { isDoSkill1 = value; }
    }

    private bool isDoSkill2 = false;
    public bool IsDoSkill2
    {
        get { return isDoSkill2; }
        set { isDoSkill2 = value; }
    }
    private bool isPause = false;

    private void Start()
    {
        playerState = GetComponent<PlayerState>();

        lastMoveVector = Vector2.left;
    }
    private void OnEnable()
    {
        TimeManager.timePauseAction += TimePause;
        TimeManager.timeResumeAction += TimeResume;
        
    }
    private void OnDisable()
    {
        TimeManager.timePauseAction -= TimePause;
        TimeManager.timeResumeAction -= TimeResume;
    }

    void Update()
    {
        if (!(playerState.IsDead || playerState.IsSturn || playerState.IsKnockBack) && !isPause)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            moveVector.x = Input.GetAxisRaw("Horizontal");
            moveVector.y = Input.GetAxisRaw("Vertical");

            moveVector = moveVector.normalized;

            if(playerState.BodySlapping)
            {
                moveVector = Vector2.zero;
            }
            else if (moveVector != Vector2.zero)
            {
                lastMoveVector = moveVector;
            }

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK])) // left shift
            {
                isDoSkill1 = true;
            }

            if (!EventSystem.current.IsPointerOverGameObject()) // mouse 0
            {
                if(Input.GetMouseButtonDown(0))
                {
                    isDoSkill0 = true;
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    isDoSkill0 = false;
                }
            }

            //Debug.Log(isDoSkill0 == Input.GetButton("Shoot"));

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.DRAIN])) // q
            {
                isDoSkill2 = true;
            }

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.INTERACTION])) // e
            {
                isInteraction = true;
            }
        }
        else
        {
            moveVector = Vector2.zero;

            isDoSkill0 = false;
            isDoSkill1 = false;
            isDoSkill2 = false;
            isInteraction = false;
        }
    }
    private void FixedUpdate()
    {
        if (isDoSkill0)
        {
            EventManager.TriggerEvent("StartSkill0"); // 기본공격

            //isDoSkill0 = false;
        }

        if (isDoSkill1)
        {
            EventManager.TriggerEvent("StartSkill1"); // 스킬1

            isDoSkill1 = false;
        }

        if(isDoSkill2)
        {
            EventManager.TriggerEvent("StartSkill2"); // 스킬2

            isDoSkill2 = false;
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
