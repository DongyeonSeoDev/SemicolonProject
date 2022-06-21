using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InputTutoData
{
    public KeyAction key;

    public bool isClear;
    public bool timerStarted;

    public float pressTime;
    public float pressTimer;

    public InputTutoData(KeyAction k, float pt)
    {
        key = k;
        pressTime = pt;

    }

    public void StartTimer()
    {
        if (SlimeGameManager.Instance.Player.PlayerInput.IsPauseByTuto)
        {
            return;
        }

        timerStarted = true;
        pressTimer = pressTime;
    }
    public void CheckTimer()
    {
        if(timerStarted && pressTimer >= 0)
        {
            pressTimer -= Time.deltaTime;

            if(pressTimer < 0)
            {
                isClear = true;
                timerStarted = false;
                pressTimer = -1f;
                KeyActionManager.Instance.EndExclamationCharging(true);
                UIManager.Instance.RequestLogMsg("'" + key.ToString() + "' 키를(을) 획득하였습니다.");
            }
        }
    }
}
public class InputTutorial : MonoBehaviour
{
    private readonly string tutoStageId = "Stage0";

    public bool isTestMode = false;
    public bool keyNotPressed = false;

    private readonly float keyNotPressTime = 5f;

    private readonly string[] keyNotPressStrArr = {
            "으...",
            "어둡다...",
            "근처에 누가 없을까..?",
            "왜 움직여지지 않는 거지"
        };

    public float keyNotPressTimer = 0f;


    [SerializeField]
    private List<InputTutoData> inputTutoDatas = new List<InputTutoData>();

    private Dictionary<KeyAction, InputTutoData> inputTutoDataDict = new Dictionary<KeyAction, InputTutoData>();
    public Dictionary<KeyAction, InputTutoData> InputTutoDataDict
    {
        get { return inputTutoDataDict; }
    }

    private bool moveKeyClear = false;

    private bool skill0Clear = false;
    private bool skill1Clear = false;
    private bool skill2Clear = false;

    private void Awake()
    {
        #region 이동관련
#if UNITY_EDITOR
        float moveTutoWaitTime = 0.5f;
#else
        float moveTutoWaitTime = 2f;
#endif
        inputTutoDatas.Add(new InputTutoData(KeyAction.LEFT, moveTutoWaitTime));
        inputTutoDatas.Add(new InputTutoData(KeyAction.RIGHT, moveTutoWaitTime));
        inputTutoDatas.Add(new InputTutoData(KeyAction.UP, moveTutoWaitTime));
        inputTutoDatas.Add(new InputTutoData(KeyAction.DOWN, moveTutoWaitTime));
        #endregion

        #region 공격관련
        inputTutoDatas.Add(new InputTutoData(KeyAction.ATTACK, 0f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.SPECIALATTACK1, 0f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.SPECIALATTACK2, 0f));
#endregion
    }
    private void OnEnable()
    {
        EventManager.StartListening("Skill0TutoClear", Skill0TutoClear);
        EventManager.StartListening("Skill1TutoClear", Skill1TutoClear);
        EventManager.StartListening("Skill2TutoClear", Skill2TutoClear);
    }
    private void OnDisable()
    {
        EventManager.StopListening("Skill0TutoClear", Skill0TutoClear);
        EventManager.StopListening("Skill1TutoClear", Skill1TutoClear);
        EventManager.StopListening("Skill2TutoClear", Skill2TutoClear);
    }
    void Start()
    {
        if (isTestMode)
        {
            moveKeyClear = true;

            skill0Clear = true;
            skill1Clear = true;
            skill2Clear = true;

            EventManager.TriggerEvent("Tuto_GainAllArrowKey");

            for (int i = 0; i < inputTutoDatas.Count; i++)
            {
                inputTutoDatas[i].isClear = true;
            }
        }
        else
        {
            for (int i = 0; i < inputTutoDatas.Count; i++)
            {
                inputTutoDatas[i].isClear = false;
            }
        }

        foreach (var item in inputTutoDatas)
        {
            if (!inputTutoDataDict.ContainsKey(item.key))
            {
                inputTutoDataDict.Add(item.key, item);
            }
            else
            {
                Debug.LogWarning("중복되는 값이 있네용? 나중에 확인해보셔요. '" + item.key + "' <- 이게 중복되고있어용!");

                inputTutoDataDict[item.key] = item;
            }
        }
    }


    void Update()
    {
        foreach (var item in inputTutoDataDict)
        {
            item.Value.CheckTimer();
        }

        keyNotPressed = true;

#region fixedKeyDict
        CheckFixedKey(KeyAction.LEFT);
        CheckFixedKey(KeyAction.RIGHT);
        CheckFixedKey(KeyAction.UP);
        CheckFixedKey(KeyAction.DOWN);
        CheckFixedKey(KeyAction.ATTACK);
#endregion
#region keyDict
        CheckKey(KeyAction.SPECIALATTACK1);
        CheckKey(KeyAction.SPECIALATTACK2);
#endregion

#region MoveKeyClearCheck

        if ((CheckClear(KeyAction.LEFT) || CheckClear(KeyAction.RIGHT) || CheckClear(KeyAction.UP) || CheckClear(KeyAction.DOWN)) && !moveKeyClear)
        {
            moveKeyClear = true;

            EventManager.TriggerEvent("Tuto_GainArrowKey");
            EventManager.TriggerEvent("StartBGM", tutoStageId);
        }
#endregion

        if(!moveKeyClear && keyNotPressed)
        {
            keyNotPressTimer += Time.deltaTime;

            if(keyNotPressTimer >= keyNotPressTime)
            {
                TalkManager.Instance.SetSubtitle(keyNotPressStrArr);

                keyNotPressTimer = 0f;
            }
        }
        else
        {
            keyNotPressTimer = 0f;
        }
    }
    private void CheckFixedKey(KeyAction keyAction)
    {
        if (KeyAction.ATTACK == keyAction && skill0Clear)
        {
            inputTutoDataDict[keyAction].isClear = true;
        }

        if (inputTutoDataDict.ContainsKey(keyAction) && inputTutoDataDict[keyAction].isClear)
        {
            return;
        }

        if (SlimeGameManager.Instance.Player.PlayerInput.IsPauseByTuto)
        {
            return;
        }

        if (Input.GetKey(KeySetting.fixedKeyDict[keyAction]))
        {
            if (inputTutoDataDict.ContainsKey(keyAction) && !inputTutoDataDict[keyAction].timerStarted && !KeyActionManager.Instance.IsNoticingGetMove)
            {
                //KeyActionManager.Instance.SetPlayerHeadText("?", 0.5f);
                if (KeyAction.ATTACK == keyAction)
                {
                    KeyActionManager.Instance.ShowQuestionMark();
                }
                else
                {
                    KeyActionManager.Instance.ExclamationCharging(inputTutoDataDict[keyAction].pressTime, keyAction);
                }

                if (KeyAction.ATTACK == keyAction)
                {
                    return;
                }

                CheckStartTimer(keyAction);

                keyNotPressed = false;
            }
        }
        
        if(inputTutoDataDict[keyAction].timerStarted && Input.GetKeyUp(KeySetting.fixedKeyDict[keyAction]))
        {
            SetTimerStartedFalse(keyAction);
            KeyActionManager.Instance.EndExclamationCharging(false);
        }
    }
    private void CheckKey(KeyAction keyAction)
    {
        switch (keyAction)
        {
            case KeyAction.SPECIALATTACK1:
                if (skill1Clear)
                {
                    inputTutoDataDict[keyAction].isClear = true;
                }
                break;
            case KeyAction.SPECIALATTACK2:
                if (skill2Clear)
                {
                    inputTutoDataDict[keyAction].isClear = true;
                }
                break;
        }

        if (inputTutoDataDict.ContainsKey(keyAction) && inputTutoDataDict[keyAction].isClear)
        {
            return;
        }

        if (SlimeGameManager.Instance.Player.PlayerInput.IsPauseByTuto)
        {
            return;
        }

        if (Input.GetKey(KeySetting.keyDict[keyAction]))
        {
            if (KeyAction.SPECIALATTACK1 == keyAction || KeyAction.SPECIALATTACK2 == keyAction)
            {
                return;
            }

            if (inputTutoDataDict.ContainsKey(keyAction) && !inputTutoDataDict[keyAction].timerStarted)
            {
                KeyActionManager.Instance.ShowQuestionMark();
            }

            CheckStartTimer(keyAction);

            keyNotPressed = false;
        }

        if (inputTutoDataDict[keyAction].timerStarted && Input.GetKeyUp(KeySetting.keyDict[keyAction]))
        {
            SetTimerStartedFalse(keyAction);
        }
    }
    private void CheckStartTimer(KeyAction keyAction)
    {
        if (CheckAnyTimerStarted())
        {
            return;
        }

        if (inputTutoDataDict.ContainsKey(keyAction))
        {
            if (!(inputTutoDataDict[keyAction].isClear || inputTutoDataDict[keyAction].timerStarted))
            {
                inputTutoDataDict[keyAction].StartTimer();
            }
        }
    }
    public bool CheckAnyTimerStarted()
    {
        foreach(var item in inputTutoDataDict.Values)
        {
            if(item.timerStarted)
            {
                return true;
            }
        }

        return false;
    }
    public bool CheckClear(KeyAction keyAction)
    {
        if(inputTutoDataDict.ContainsKey(keyAction))
        {
            return inputTutoDataDict[keyAction].isClear;
        }

        return false;
    }
    private void SetTimerStartedFalse(KeyAction keyAction)
    {
        if (inputTutoDataDict.ContainsKey(keyAction))
        {
            inputTutoDataDict[keyAction].timerStarted = false;
        }
    }
    private void Skill0TutoClear()
    {
        skill0Clear = true;
    }
    private void Skill1TutoClear()
    {
        skill1Clear = true;
    }
    private void Skill2TutoClear()
    {
        skill2Clear = true;
    }
}
