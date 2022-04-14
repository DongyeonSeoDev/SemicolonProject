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
            }
        }
    }
}
public class InputTutorial : MonoBehaviour
{
    public bool isTestMode = true;

    [SerializeField]
    private List<InputTutoData> inputTutoDatas = new List<InputTutoData>();

    private Dictionary<KeyAction, InputTutoData> inputTutoDataDict = new Dictionary<KeyAction, InputTutoData>();
    public Dictionary<KeyAction, InputTutoData> InputTutoDataDict
    {
        get { return inputTutoDataDict; }
    }

    private bool moveKeyClearAll = false;

    private void Awake()
    {
        #region 이동관련
        inputTutoDatas.Add(new InputTutoData(KeyAction.LEFT, 3f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.RIGHT, 3f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.UP, 3f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.DOWN, 3f));
        #endregion

        #region 공격관련
        inputTutoDatas.Add(new InputTutoData(KeyAction.ATTACK, 0f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.SPECIALATTACK1, 0f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.SPECIALATTACK2, 0f));
        #endregion
    }

    void Start()
    {
        if (isTestMode)
        {
            moveKeyClearAll = true;
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
        foreach(var item in inputTutoDataDict)
        {
            item.Value.CheckTimer();
        }

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

        if (!moveKeyClearAll)
        {
            int mkClearNum = 0;

            if (CheckClear(KeyAction.LEFT))
            {
                mkClearNum++;
            }

            if (CheckClear(KeyAction.RIGHT))
            {
                mkClearNum++;
            }

            if (CheckClear(KeyAction.UP))
            {
                mkClearNum++;
            }

            if (CheckClear(KeyAction.DOWN))
            {
                mkClearNum++;
            }

            if (mkClearNum >= 4)
            {
                moveKeyClearAll = true;
                EventManager.TriggerEvent("Tuto_GainAllArrowKey");
            }
        }

        #endregion
    }
    private void CheckFixedKey(KeyAction keyAction)
    {
        if (Input.GetKey(KeySetting.fixedKeyDict[keyAction]))
        {
            CheckStartTimer(keyAction);
        }
        else
        {
            SetTimerStartedFalse(keyAction);
        }
    }
    private void CheckKey(KeyAction keyAction)
    {
        if (Input.GetKey(KeySetting.keyDict[keyAction]))
        {
            CheckStartTimer(keyAction);
        }
        else
        {
            SetTimerStartedFalse(keyAction);
        }
    }
    private void CheckStartTimer(KeyAction keyAction)
    {
        if (inputTutoDataDict.ContainsKey(keyAction))
        {
            if (!(inputTutoDataDict[keyAction].isClear || inputTutoDataDict[keyAction].timerStarted))
            {
                inputTutoDataDict[keyAction].StartTimer();
            }
        }
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
}
