using UnityEngine;
using System.Collections.Generic;
using System;

public static partial class Global
{
    public const string EnterNextMap = "EnterNextMap"; //���� �� �� �� �̺�Ʈ Ű

    public const string saveFileName_1 = "SaveFile1";

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
    private static Sprite[] speciesSpriteArr = new Sprite[EnumCount<EnemySpecies>()];

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

    public static string ToMonsterSpeciesStr(this EnemySpecies species)
    {
        if(mobSpeciesKoArr == null)
        {
            mobSpeciesKoArr = TextAssetsToStringArr(TAResSysDefaultPath + "species");
        }
        return mobSpeciesKoArr[(int)species];
    }

    public static Sprite ToEnemySpeciesSprite(this EnemySpecies species)
    {
        if (speciesSpriteArr[(int)species] == null)
            speciesSpriteArr[(int)species] = Resources.Load<Sprite>("System/Sprites/MobSpeciesIcon/" + species.ToString());
        return speciesSpriteArr[(int)species];
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

   /* public static string StateAbnorToString(StateAbnormality state)
    {
        switch(state)
        {
            case StateAbnormality.Pain:
                return "����";
            case StateAbnormality.Poverty:
                return "����";
            case StateAbnormality.Scar:
                return "��ó";
            case StateAbnormality.Blind:
                return "�Ǹ�";
            default:
                Debug.Log("���� �������� ���� : " + state.ToString());
                return string.Empty;
        }
    }*/

    public static T[] GetEnumArr<T>() => (T[])Enum.GetValues(typeof(T));

    public static void TriggerEvent(this string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            EventManager.TriggerEvent(key);
        }
    }

    public static string AlternateIfEmpty(this string str, string replace) => !string.IsNullOrEmpty(str) ? str : replace;  // ??����� string ���� ""�� �ƴ϶� null�� ��

   
}