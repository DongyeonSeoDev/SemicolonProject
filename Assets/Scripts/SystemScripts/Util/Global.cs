using UnityEngine;
using System.Collections.Generic;

public static partial class Global
{
    public const string saveFileName_1 = "SaveFile1";

    public const int playerLayer = 3;

    public const float cameraPlaneDistance = 20f;

    public const string PickupPlant = "PickupPlant";

    public static Vector2 noticeMsgOriginRectPos = new Vector2(0, -229.61f);
    public static Vector3 damageTextMove = new Vector3(0, 2.5f, 0);
    
    private static string[] keyActionNameArr;

    private static Dictionary<string, Sprite> idToBodySprDict = new Dictionary<string, Sprite>();

    public static string ToKeyActionName(KeyAction keyAction)
    {
        if(keyActionNameArr==null)
        {
            TextAsset kta = Resources.Load<TextAsset>("System/TextAssets/Key/KeyActionName-ko");
            keyActionNameArr = kta.text.Split('\n');
        }
        return keyActionNameArr[(int)keyAction];
    }


    public static Sprite GetMonsterBodySprite(string id)
    {
        if(idToBodySprDict.ContainsKey(id))
        {
            return idToBodySprDict[id];
        }
        else
        {
            Sprite spr = Resources.Load<Sprite>("System/Sprites/MonsterBody/" + id);
            if (!spr)
            {
                return MonsterCollection.Instance.notExistBodySpr;
            }
            idToBodySprDict.Add(id, spr);
            return spr;
        }
    }
}