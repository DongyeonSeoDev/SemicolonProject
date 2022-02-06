using System.Collections.Generic;
using UnityEngine;

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keyDict = new Dictionary<KeyAction, KeyCode>();

    public static void SetDefaultKeySetting()
    {
        keyDict[KeyAction.INVENTORY] = KeyCode.I;
        keyDict[KeyAction.STAT] = KeyCode.T;
        keyDict[KeyAction.INTERACTION] = KeyCode.F;
        keyDict[KeyAction.MANASTONE] = KeyCode.E;
        keyDict[KeyAction.DRAIN] = KeyCode.Q;
        keyDict[KeyAction.SPECIALATTACK] = KeyCode.LeftShift;
        keyDict[KeyAction.SETTING] = KeyCode.Tab;
    }
}