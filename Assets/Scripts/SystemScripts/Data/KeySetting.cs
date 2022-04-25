using System.Collections.Generic;
using UnityEngine;

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keyDict = new Dictionary<KeyAction, KeyCode>();
    public static Dictionary<KeyAction, KeyCode> fixedKeyDict = new Dictionary<KeyAction, KeyCode>();

    public static void SetDefaultKeySetting() //커스텀 키셋 가능한 키세팅 기본값 설정. 
    {
        //순서대로 키세팅 UI에 표시됨

        //keyDict[KeyAction.MENU] = KeyCode.Tab;
        keyDict[KeyAction.INVENTORY] = KeyCode.I;
        keyDict[KeyAction.STAT] = KeyCode.T;
        keyDict[KeyAction.MONSTER_COLLECTION] = KeyCode.C;
        //keyDict[KeyAction.CHANGEABLEBODYS] = KeyCode.H;
        keyDict[KeyAction.INTERACTION] = KeyCode.F;
        keyDict[KeyAction.EVENT] = KeyCode.Space;
        keyDict[KeyAction.SPECIALATTACK1] = KeyCode.LeftShift;
        keyDict[KeyAction.SPECIALATTACK2] = KeyCode.Q;
        keyDict[KeyAction.MANASTONE] = KeyCode.E;
        keyDict[KeyAction.CHANGE_SLIME] = KeyCode.Alpha1;
        keyDict[KeyAction.CHANGE_MONSTER1] = KeyCode.Alpha2;
        keyDict[KeyAction.CHANGE_MONSTER2] = KeyCode.Alpha3;
        //keyDict[KeyAction.SETTING] = KeyCode.Tab;
        keyDict[KeyAction.QUIT] = KeyCode.F4;
    }

    public static void SetFixedKeySetting() //고정키 세팅
    {
        fixedKeyDict[KeyAction.UP] = KeyCode.W;
        fixedKeyDict[KeyAction.DOWN] = KeyCode.S;
        fixedKeyDict[KeyAction.LEFT] = KeyCode.A;
        fixedKeyDict[KeyAction.RIGHT] = KeyCode.D;
        fixedKeyDict[KeyAction.ATTACK] = KeyCode.Mouse0;
        fixedKeyDict[KeyAction.SETTING] = KeyCode.Escape;
    }

    public static KeyCode GetKeyCode(KeyAction k)
    {
        KeyCode code;
        if (keyDict.TryGetValue(k, out code))
            return code;
        if (fixedKeyDict.TryGetValue(k, out code))
            return code;
        return KeyCode.None;
    }

    public static SaveDic<KeyAction, bool> InitKeyActionActive
    {
        get
        {
            SaveDic<KeyAction, bool> keyActive = new SaveDic<KeyAction, bool>();
            keyActive[KeyAction.SETTING] = false;
            keyActive[KeyAction.INVENTORY] = false;
            keyActive[KeyAction.STAT] = false;
            keyActive[KeyAction.MONSTER_COLLECTION] = false;
            keyActive[KeyAction.QUIT] = false;
            
            return keyActive;
        }
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
        keycodeToStringDic.Add(KeyCode.Escape, "ESC");
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