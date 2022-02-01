using System.Collections.Generic;
using UnityEngine;

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keyDict = new Dictionary<KeyAction, KeyCode>();

    public static void SetDefaultKeySetting()
    {
        keyDict.Add(KeyAction.INVENTORY, KeyCode.I);
        keyDict.Add(KeyAction.STAT, KeyCode.T);
        keyDict.Add(KeyAction.INTERACTION, KeyCode.F);
        keyDict.Add(KeyAction.MANASTONE, KeyCode.E);
        keyDict.Add(KeyAction.SPECIALATTACK, KeyCode.LeftShift);
    }
}