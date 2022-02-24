using System.Collections.Generic;
using UnityEngine;

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keyDict = new Dictionary<KeyAction, KeyCode>();
    public static Dictionary<KeyAction, KeyCode> fixedKeyDict = new Dictionary<KeyAction, KeyCode>();

    public static void SetDefaultKeySetting() //커스텀 키셋 가능한 키세팅 기본값 설정
    {
        keyDict[KeyAction.INVENTORY] = KeyCode.I;
        keyDict[KeyAction.STAT] = KeyCode.T;
        keyDict[KeyAction.MONSTER_COLLECTION] = KeyCode.C;
        keyDict[KeyAction.INTERACTION] = KeyCode.F;
        keyDict[KeyAction.MANASTONE] = KeyCode.E;
        keyDict[KeyAction.DRAIN] = KeyCode.Q;
        keyDict[KeyAction.SPECIALATTACK] = KeyCode.LeftShift;
        keyDict[KeyAction.CHANGE_SLIME] = KeyCode.Alpha1;
        keyDict[KeyAction.CHANGE_MONSTER1] = KeyCode.Alpha2;
        keyDict[KeyAction.CHANGE_MONSTER2] = KeyCode.Alpha3;
        keyDict[KeyAction.SETTING] = KeyCode.Tab;
    }

    public static void SetFixedKeySetting() //고정키 세팅
    {
        fixedKeyDict[KeyAction.UP] = KeyCode.W;
        fixedKeyDict[KeyAction.DOWN] = KeyCode.S;
        fixedKeyDict[KeyAction.LEFT] = KeyCode.A;
        fixedKeyDict[KeyAction.RIGHT] = KeyCode.D;
        fixedKeyDict[KeyAction.ATTACK] = KeyCode.Mouse0;
    }
}

public static class KeyCodeToString
{
    private static Dictionary<KeyCode, string> keycodeToStringDic;

    public static void Init()
    {
        keycodeToStringDic = new Dictionary<KeyCode, string>();

        keycodeToStringDic.Add(KeyCode.Mouse0, "Left Mouse");
        keycodeToStringDic.Add(KeyCode.Mouse1, "Right Mouse");
        for (int i=0; i<10; ++i)
        {
            keycodeToStringDic.Add((KeyCode)(48 + i), i.ToString());
        }
    }

    public static string GetString(KeyCode key)
    {
        if (keycodeToStringDic.ContainsKey(key))
            return keycodeToStringDic[key];
        else
            return key.ToString();
    }
}