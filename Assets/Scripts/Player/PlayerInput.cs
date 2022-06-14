using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    private PlayerState playerState = null;
    private InputTutorial inputTutorial = null;

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
    private Vector2 lastBodySlapVector = Vector2.zero;
    public Vector2 LastBodySlapVector
    {
        get { return lastBodySlapVector; }
        set { lastBodySlapVector = value; }
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

    private bool cantPlaySkill0 = false;
    public bool CantPlaySkill0
    {
        get { return cantPlaySkill0; }
        set { cantPlaySkill0 = value; }
    }

    private bool cantPlaySkill1 = false;
    public bool CantPlaySkill1
    {
        get { return cantPlaySkill1; }
        set { cantPlaySkill1 = value; }
    }

    private bool cantPlaySkill2 = false;
    public bool CantPlaySkill2
    {
        get { return cantPlaySkill2; }
        set { cantPlaySkill2 = value; }
    }

    private bool skill0ButtonDowned = false;
    private bool skill1ButtonDowned = false;
    private bool skill2ButtonDowned = false;

    private bool skill0TutoClear = false; // 기본공격
    private bool skill1TutoClear = false;
    private bool skill2TutoClear = false;

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

    [SerializeField]
    private bool isPauseByTuto = false;
    public bool IsPauseByTuto
    {
        get { return isPauseByTuto; }
        set { isPauseByTuto = value; }
    }

    [SerializeField]
    private bool isPauseByCutScene = false;
    public bool IsPauseByCutScene
    {
        get { return isPauseByCutScene; }
        set { isPauseByCutScene = value; }
    }

    private bool gameClear = true;

    [SerializeField]
    private float playerStopStayTime = 2f;
    private float playerStopStayTimer = 0f;

    private bool playerStopStayTimerStarted = false;

    private bool playerStopStay = false;
    public bool PlayerStopStay
    {
        get { return playerStopStay; }
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", PlayerReset);
        EventManager.StartListening("ChangeBody", PlayerReset);
        EventManager.StartListening("StartCutScene", PauseByCutScene);
        EventManager.StartListening("EndCutScene", ResumeByCutScene);
        EventManager.StartListening("GameClear", GameClear);

        EventManager.StartListening("Skill0TutoClear", Skill0TutoClear);
        EventManager.StartListening("Skill1TutoClear", Skill1TutoClear);
        EventManager.StartListening("Skill2TutoClear", Skill2TutoClear);

        TimeManager.timePauseAction += TimePause;
        TimeManager.timeResumeAction += TimeResume;
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", PlayerReset);
        EventManager.StopListening("ChangeBody", PlayerReset);
        EventManager.StopListening("StartCutScene", PauseByCutScene);
        EventManager.StopListening("EndCutScene", ResumeByCutScene);
        EventManager.StopListening("GameClear", GameClear);

        EventManager.StopListening("Skill0TutoClear", Skill0TutoClear);
        EventManager.StopListening("Skill1TutoClear", Skill1TutoClear);
        EventManager.StopListening("Skill2TutoClear", Skill2TutoClear);

        TimeManager.timePauseAction -= TimePause;
        TimeManager.timeResumeAction -= TimeResume;
    }

    private void Start()
    {
        playerState = GetComponent<PlayerState>();

        lastMoveVector = Vector2.left;

        inputTutorial = GetComponent<InputTutorial>();

        if (TutorialManager.Instance.IsTutorialStage)
        {
            if (inputTutorial == null)
            {
                inputTutorial = gameObject.AddComponent<InputTutorial>();
            }

            isPauseByTuto = true;
        }
        else
        {
            if (inputTutorial != null)
            {
                Destroy(inputTutorial);
            }

            isPauseByTuto = false;

            skill0TutoClear = true;
            skill1TutoClear = true;
            skill2TutoClear = true;
        }

        gameClear = false;
        playerState.CantMove = false;
    }

    void Update()
    {
        if(playerStopStayTimerStarted)
        {
            playerStopStayTimer -= Time.deltaTime; 

            if(playerStopStayTimer <= 0f)
            {
                playerStopStayTimer = 0f;
                playerStopStay = true;
            }
        }

        if (!(isPauseByTuto || isPause || isPauseByCutScene || gameClear) &&
            !(playerState.IsDead || playerState.IsStun || playerState.IsKnockBack || playerState.IsDrain))
        {
            if (!playerState.Chargning && !playerState.BodySlapping)
            {
                moveVector.x = Input.GetAxisRaw("Horizontal");
                moveVector.y = Input.GetAxisRaw("Vertical");

                if (inputTutorial != null)
                {
                    // ContainKey체크
                    if ((moveVector.x > 0f && inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.RIGHT) &&
                        !inputTutorial.InputTutoDataDict[KeyAction.RIGHT].isClear) ||

                        (moveVector.x < 0f && inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.LEFT) && 
                        !inputTutorial.InputTutoDataDict[KeyAction.LEFT].isClear) ||

                            (moveVector.y > 0f && inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.UP) &&
                        !inputTutorial.InputTutoDataDict[KeyAction.UP].isClear) ||

                        (moveVector.y < 0f && inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.DOWN) &&
                        !inputTutorial.InputTutoDataDict[KeyAction.DOWN].isClear))
                    {
                        playerState.CantMove = true;
                    }
                    else
                    {
                        playerState.CantMove = false;
                    }
                }

                moveVector = moveVector.normalized;

                if (SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock && !playerState.IsInMomentom) // 추진력을 얻었는지, 추진력 중복상태는 아닌지 체크
                {
                    if (moveVector != Vector2.zero)
                    {
                        if (playerStopStayTimerStarted && playerStopStay)
                        {
                            // 추진력 실행 코드
                            Debug.Log("이러석구나... 내가 멈췄던 것은 추진력을 얻기 위해서였다!!!");
                            playerState.IsInMomentom = true;
                        }

                        playerStopStay = false;
                        playerStopStayTimerStarted = false;

                    }
                    else if (!(playerStopStayTimerStarted || playerStopStay))
                    {
                        playerStopStayTimerStarted = true;

                        playerStopStayTimer = playerStopStayTime;
                    }
                }

                if (playerState.BodySlapping)
                {
                    moveVector = Vector2.zero;
                }
                else if (moveVector != Vector2.zero)
                {
                    lastMoveVector = moveVector;
                }

                // ContainKey체크
                if (inputTutorial == null || (!inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.INTERACTION) ||
                    inputTutorial.InputTutoDataDict[KeyAction.INTERACTION].isClear))
                {
                    if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.INTERACTION]))
                    {
                        isInteraction = true;
                    }
                }

                if (skill0TutoClear)
                {
                    if (
                    (inputTutorial == null || (!inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.ATTACK) ||
                    inputTutorial.InputTutoDataDict[KeyAction.ATTACK].isClear))
                    && !cantPlaySkill0) // mouse 0
                    {
                        //isDoSkill0 = Input.GetMouseButton(0);

                        if (Input.GetMouseButtonDown(0))
                        {
                            isDoSkill0 = true;
                            skill0ButtonDowned = true;
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            isDoSkill0 = false;
                            skill0ButtonDowned = false;

                            EventManager.TriggerEvent("SkillButtonUp0");
                        }

                        if((skill0ButtonDowned && !Input.GetMouseButton(0)))
                        {
                            isDoSkill0 = false;
                            skill0ButtonDowned = false;

                            EventManager.TriggerEvent("SkillCancel0");
                        }
                    }
                }

                //Debug.Log(isDoSkill0 == Input.GetButton("Shoot"));
                // ContainKey체크

                if (skill2TutoClear)
                {
                    if ((inputTutorial == null || (!inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.SPECIALATTACK2) ||
                        inputTutorial.InputTutoDataDict[KeyAction.SPECIALATTACK2].isClear))
                        && !cantPlaySkill2)
                    {
                        if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK2]))
                        {
                            isDoSkill2 = true;
                            skill2ButtonDowned = true;
                        }

                        if (Input.GetKeyUp(KeySetting.keyDict[KeyAction.SPECIALATTACK2]))
                        {
                            skill2ButtonDowned = false;

                            EventManager.TriggerEvent("SkillButtonUp2");
                        }

                        if((skill2ButtonDowned && !Input.GetKey(KeySetting.keyDict[KeyAction.SPECIALATTACK2])))
                        {
                            skill2ButtonDowned = false;

                            EventManager.TriggerEvent("SkillCancel2");
                        }
                    }
                }
            }
            else
            {
                PlayerReset();
            }

            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // ContainKey체크
            if ((inputTutorial == null || (!inputTutorial.InputTutoDataDict.ContainsKey(KeyAction.SPECIALATTACK1) ||
                    inputTutorial.InputTutoDataDict[KeyAction.SPECIALATTACK1].isClear))
                    && !cantPlaySkill1)
            {

                if (skill1TutoClear)
                {
                    if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK1]) || Input.GetMouseButtonDown(1)) // left shift
                    {
                        skill1ButtonDowned = true;

                        isDoSkill1 = true;
                    }

                    if (Input.GetKeyUp(KeySetting.keyDict[KeyAction.SPECIALATTACK1]) || Input.GetMouseButtonUp(1))
                    {
                        skill1ButtonDowned = false;

                        EventManager.TriggerEvent("SkillButtonUp1");
                    }

                    if((skill1ButtonDowned && (!Input.GetKey(KeySetting.keyDict[KeyAction.SPECIALATTACK1]) && !Input.GetMouseButton(1))))
                    {
                        skill1ButtonDowned = false;

                        EventManager.TriggerEvent("SkillCancel1");
                    }
                }
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
        //skill0ButtonDowned = false;
        ////skill1ButtonDowned = false;
        //skill2ButtonDowned = false;
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

    private void Skill0TutoClear()
    {
        skill0TutoClear = true;
    }
    private void Skill1TutoClear()
    {
        skill1TutoClear = true;
    }
    private void Skill2TutoClear()
    {
        skill2TutoClear = true;
    }

    private void PauseByCutScene()
    {
        playerState.CantMove = true;

        isPauseByCutScene = true;

        PlayerReset();
    }
    private void ResumeByCutScene()
    {
        playerState.CantMove = false;

        isPauseByCutScene = false;

        PlayerReset();
    }
    private void GameClear()
    {
        gameClear = true;
        playerState.CantMove = true; ;
    }
}
