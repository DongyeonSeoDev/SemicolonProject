using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public static partial class Global
{
    public const string GAME_SAVE_FILE = "SaveFile4";  //Test File Name

    public const string SAVE_FILE_1 = "SaveFile_16";
    public const string SAVE_FILE_2 = "SaveFile_17";
    public const string SAVE_FILE_3 = "SaveFile_18";

    public static string GetSaveFileName(int index)
    {
        switch(index)
        {
            case 0: return SAVE_FILE_1;
            case 1: return SAVE_FILE_2;
            case 2: return SAVE_FILE_3;
            default:
                Debug.Log("존재하지 않는 인덱스 : " + index);
                return SAVE_FILE_1;
        }
    }
}

public static partial class Global
{
    public const float ImprAndRecoEffDelay = 3.4f;
    public const float ImprAndRecoInteractDelay = 0.4f;
}

public static partial class Global
{
    public const string EnterNextMap = "EnterNextMap"; //다음 맵 갈 때 이벤트 키

    public const string OriginBodyID = "origin";

    public const int playerLayer = 3;

    public const float cameraPlaneDistance = 20f;

    public const string PickupPlant = "PickupPlant";

    public const string TAResSysDefaultPath = "System/TextAssets/";

    public static Player CurrentPlayer => SlimeGameManager.Instance.Player;

    public static Transform GetSlimePos => SlimeGameManager.Instance.CurrentPlayerBody.transform;

    public static Vector2 noticeMsgOriginRectPos = new Vector2(0, -229.61f);
    public static Vector3 damageTextMove = new Vector3(0, 0.8f, 0);
    public static Vector3 worldTxtMove = new Vector3(0, 0.6f, 0);
    
    private static string[] keyActionNameArr;
    private static string[] mobSpeciesKoArr;
    //private static Sprite[] speciesSpriteArr = new Sprite[EnumCount<EnemySpecies>()];

    public static string[] TextAssetsToStringArr(string path, char criteria = '\n') => Resources.Load<TextAsset>(path).text.Split(criteria);

    public static string ToKeyActionName(KeyAction keyAction)
    {
        if(keyActionNameArr==null)
        {
            TextAsset kta = Resources.Load<TextAsset>(TAResSysDefaultPath+"KeyActionName-ko");
            keyActionNameArr = kta.text.Split('\n');
        }
        return keyActionNameArr[(int)keyAction];
    }

    /*public static string ToMonsterSpeciesStr(this EnemySpecies species)
    {
        if(mobSpeciesKoArr == null)
        {
            mobSpeciesKoArr = TextAssetsToStringArr(TAResSysDefaultPath + "species");
        }
        return mobSpeciesKoArr[(int)species];
    }*/

   /* public static Sprite ToEnemySpeciesSprite(this EnemySpecies species)
    {
        if (speciesSpriteArr[(int)species] == null)
            speciesSpriteArr[(int)species] = Resources.Load<Sprite>("System/Sprites/MobSpeciesIcon/" + species.ToString());  //지금은 걍 몹 이미지 하나 가져와서 쓰지만 몬스터 종마다 대표 이미지가 필요해보임
        return speciesSpriteArr[(int)species];
    }*/

    public static Sprite GetMonsterBodySprite(string id)
    {
        Sprite spr = MonsterCollection.Instance.GetMonsterInfo(id).bodyImg;
        if (!spr) spr = MonsterCollection.Instance.notExistBodySpr;
        return spr;
    }

    public static string GetMonsterName(string id)
    {
        string str = MonsterCollection.Instance.GetMonsterInfo(id).bodyName;
        if (string.IsNullOrEmpty(str)) str = "몬스터 이름";
        return str;
    }

    public static string AreaTypeToString(AreaType type)
    {
        string k = "AreaType"; 
        if(!TypeToTextAsset.HasKey(k))
        {
            TypeToTextAsset.Register(k, TAResSysDefaultPath + "AreaTexts");
        }
        return TypeToTextAsset.GetText(k, (int)type);
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

    public static T[] GetEnumArr<T>() => (T[])Enum.GetValues(typeof(T));

    public static void TriggerEvent(this string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            EventManager.TriggerEvent(key);
        }
    }

    public static string AlternateIfEmpty(this string str, string replace) => !string.IsNullOrEmpty(str) ? str : replace;  // ??사용은 string 값이 ""가 아니라 null일 때

    public static string ToColorStr(this string str, string colorCode)
    {
        return string.Format("<color={0}>{1}</color>", colorCode, str);
    }
    public static string ToColorStr(this string str, string colorCode, Func<bool> condition)
    {
        return condition() ? string.Format("<color={0}>{1}</color>", colorCode, str) : str;
    }

    public static void UpdateSizeDelay(this CustomContentsSizeFilter filter)
    {
        Util.DelayFunc(() => filter.UpdateSize(), 0.1f, null, true, false);
    }

    public static Vector2 GetRightAngleCoord(Vector2 p1, Vector2 p2, bool p1PosXMaintain)
    {
        Vector2 p3 = new Vector2();

        if (p1PosXMaintain)
        {
            p3.x = p1.x;
            p3.y = p2.y;
        }
        else
        {
            p3.x = p2.x;
            p3.y = p1.y;
        }

        return p3;
    }
}
