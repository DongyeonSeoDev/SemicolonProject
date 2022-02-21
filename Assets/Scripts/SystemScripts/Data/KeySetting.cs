using System.Collections.Generic;
using UnityEngine;

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keyDict = new Dictionary<KeyAction, KeyCode>();
    public static Dictionary<KeyAction, KeyCode> fixedKeyDict = new Dictionary<KeyAction, KeyCode>();

    public static void SetDefaultKeySetting()
    {
        keyDict[KeyAction.INVENTORY] = KeyCode.I;
        keyDict[KeyAction.STAT] = KeyCode.T;
        keyDict[KeyAction.MONSTER_COLLECTION] = KeyCode.C;
        keyDict[KeyAction.INTERACTION] = KeyCode.F;
        keyDict[KeyAction.MANASTONE] = KeyCode.E;
        keyDict[KeyAction.DRAIN] = KeyCode.Q;
        keyDict[KeyAction.SPECIALATTACK] = KeyCode.LeftShift;
        keyDict[KeyAction.SETTING] = KeyCode.Tab;
    }

    public static void SetFixedKeySetting()
    {
        fixedKeyDict[KeyAction.UP] = KeyCode.W;
        fixedKeyDict[KeyAction.DOWN] = KeyCode.S;
        fixedKeyDict[KeyAction.LEFT] = KeyCode.A;
        fixedKeyDict[KeyAction.RIGHT] = KeyCode.D;
        fixedKeyDict[KeyAction.ATTACK] = KeyCode.Mouse0;
    }
}