using System.Collections.Generic;
using UnityEngine;

public static partial class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keyDict = new Dictionary<KeyAction, KeyCode>();

    public static void SetDefaultKeySetting()
    {
        keyDict.Add(KeyAction.INVENTORY, KeyCode.I);
        keyDict.Add(KeyAction.STAT, KeyCode.T);
        //keyDict.Add(KeyAction.INTERACTION, KeyCode.Space);
    }
}