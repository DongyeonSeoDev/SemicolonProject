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
        if(timerStarted && pressTimer > 0)
        {
            pressTimer -= Time.deltaTime;

            if(pressTimer <= 0)
            {
                isClear = true;
                timerStarted = false;
                pressTimer = 0f;
            }
        }
    }
}
public class InputTutorial : MonoBehaviour
{
    public bool isTestMode = false;

    [SerializeField]
    private List<InputTutoData> inputTutoDatas = new List<InputTutoData>();

    private Dictionary<KeyAction, InputTutoData> inputTutoDataDict = new Dictionary<KeyAction, InputTutoData>();
    public Dictionary<KeyAction, InputTutoData> InputTutoDataDict
    {
        get { return inputTutoDataDict; }
    }

    private void Awake()
    {
        #region 이동관련
        inputTutoDatas.Add(new InputTutoData(KeyAction.LEFT, 3f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.RIGHT, 3f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.UP, 3f));
        inputTutoDatas.Add(new InputTutoData(KeyAction.DOWN, 3f));
        #endregion

    }

    void Start()
    {
        if (isTestMode)
        {
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
            if (inputTutoDataDict.ContainsKey(item.key))
            {
                inputTutoDataDict.Add(item.key, item);
            }
            else
            {
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
    private void SetTimerStartedFalse(KeyAction keyAction)
    {
        if (inputTutoDataDict.ContainsKey(keyAction))
        {
            inputTutoDataDict[keyAction].timerStarted = false;
        }
    }
}
