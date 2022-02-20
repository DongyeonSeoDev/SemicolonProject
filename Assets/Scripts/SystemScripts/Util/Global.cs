using UnityEngine;

public static partial class Global
{
    public const int playerLayer = 3;

    public const float cameraPlaneDistance = 20f;

    public const string PickupPlant = "PickupPlant";

    public static Vector2 noticeMsgOriginRectPos = new Vector2(0, -229.61f);
    
    private static string[] keyActionNameArr;

    public static string ToKeyActionName(KeyAction keyAction)
    {
        if(keyActionNameArr==null)
        {
            TextAsset kta = Resources.Load<TextAsset>("System/TextAssets/Key/KeyActionName-ko");
            keyActionNameArr = kta.text.Split('\n');
        }
        return keyActionNameArr[(int)keyAction];
    }
}