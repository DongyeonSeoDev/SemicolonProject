using UnityEngine;
using System.Collections.Generic;

public static partial class Global
{
    public const string EnterNextMap = "EnterNextMap"; //���� �� �� �� �̺�Ʈ Ű

    public const string saveFileName_1 = "SaveFile1";

    public const string OriginBodyID = "origin";

    public const int playerLayer = 3;

    public const float cameraPlaneDistance = 20f;

    public const string PickupPlant = "PickupPlant";

    public static Player CurrentPlayer => SlimeGameManager.Instance.Player;

    public static Transform GetSlimePos => SlimeGameManager.Instance.CurrentPlayerBody.transform;

    public static Vector2 noticeMsgOriginRectPos = new Vector2(0, -229.61f);
    public static Vector3 damageTextMove = new Vector3(0, 0.8f, 0);
    public static Vector3 worldTxtMove = new Vector3(0, 0.6f, 0);
    
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


    public static Sprite GetMonsterBodySprite(string id)
    {
        Sprite spr = MonsterCollection.Instance.GetMonsterInfo(id).bodyImg;
        if (!spr) spr = MonsterCollection.Instance.notExistBodySpr;
        return spr;
    }

    public static string GetMonsterName(string id)
    {
        string str = MonsterCollection.Instance.GetMonsterInfo(id).bodyName;
        if (string.IsNullOrEmpty(str)) str = "���� �̸�";
        return str;
    }

    public static string AreaTypeToString(AreaType type)
    {
        switch(type)
        {
            case AreaType.MONSTER:
                return "���� ����";
            case AreaType.RANDOM:
                return "���� ����";
            case AreaType.CHEF:
                return "�丮 ����";
            case AreaType.PLANTS:
                return "ä�� ����";
            case AreaType.BOSS:
                return "���� ����";
            case AreaType.START:
                return "���� ����";
        }
        return string.Empty;
    }

    public static DoorDirType ReverseDoorDir(DoorDirType type)
    {
        switch(type)
        {
            case DoorDirType.FRONT:
                return DoorDirType.BACK;
            case DoorDirType.BACK:
                return DoorDirType.FRONT;
            case DoorDirType.RIGHT:
                return DoorDirType.LEFT;
            case DoorDirType.LEFT:
                return DoorDirType.RIGHT;
        }
        return DoorDirType.BACK;
    }
}