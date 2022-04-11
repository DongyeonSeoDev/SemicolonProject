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

    private bool skill0ButtonDowned = false;
    private bool skill1ButtonDowned = false;
    private bool skill2ButtonDowned = false;

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
        EventManager.StartListening("PlayerDead", PlayerReset); 
        EventManager.StartListening("ChangeBody", PlayerReset);

        TimeManager.timePauseAction += TimePause;
        TimeManager.timeResumeAction += TimeResume;
        
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", PlayerReset);
        EventManager.StopListening("ChangeBody", PlayerReset);

        TimeManager.timePauseAction -= TimePause;
        TimeManager.timeResumeAction -= TimeResume;
    }

    void Update()
    {
        if (!(playerState.IsDead || playerState.IsSturn || playerState.IsKnockBack || playerState.IsDrain) && !isPause)
        {
            if (!playerState.Chargning)
            {
                moveVector.x = Input.GetAxisRaw("Horizontal");
                moveVector.y = Input.GetAxisRaw("Vertical");

                moveVector = moveVector.normalized;

                if (playerState.BodySlapping)
                {
                    moveVector = Vector2.zero;
                }
                else if (moveVector != Vector2.zero)
                {
                    lastMoveVector = moveVector;
                }

                if (!EventSystem.current.IsPointerOverGameObject()) // mouse 0
                {
                    isDoSkill0 = Input.GetMouseButton(0);

                    if(Input.GetMouseButtonDown(0))
                    {
                        skill0ButtonDowned = true;
                    }

                    if (Input.GetMouseButtonUp(0) || (skill0ButtonDowned && !Input.GetMouseButton(0)))
                    {
                        skill0ButtonDowned = false;

                        EventManager.TriggerEvent("SkillButtonUp0");
                    }
                }

                //Debug.Log(isDoSkill0 == Input.GetButton("Shoot"));

                if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK2])) // q
                {
                    isDoSkill2 = true;
                    skill2ButtonDowned = true;
                }

                if (Input.GetKeyUp(KeySetting.keyDict[KeyAction.SPECIALATTACK2]) || (skill2ButtonDowned && !Input.GetKey(KeySetting.keyDict[KeyAction.SPECIALATTACK2])))
                {
                    skill2ButtonDowned = false;

                    EventManager.TriggerEvent("SkillButtonUp2");
                }

                if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.INTERACTION])) // e
                {
                    isInteraction = true;
                }
            }
            else
            {
                PlayerReset();
            }

            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK1])) // left shift
            {
                skill1ButtonDowned = true;

                isDoSkill1 = true;
            }

            if(Input.GetKeyUp(KeySetting.keyDict[KeyAction.SPECIALATTACK1]) || (skill1ButtonDowned && !Input.GetKey(KeySetting.keyDict[KeyAction.SPECIALATTACK1])))
            {
                skill1ButtonDowned = false;

                EventManager.TriggerEvent("SkillButtonUp1");
            }
        }
        else
        {
            PlayerReset();
        }
    }

    private void PlayerReset()
    {
        moveVector = Vector2.zero;

        isDoSkill0 = false;
        isDoSkill1 = false;
        isDoSkill2 = false;
        skill0ButtonDowned = false;
        skill1ButtonDowned = false;
        skill2ButtonDowned = false;
        isInteraction = false;
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
