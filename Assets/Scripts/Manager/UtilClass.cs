using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Global
{
    public static float slideTransitionTime03 = 0.3f;
    public static float fullAlphaTransitionTime04 = 0.4f;
    public static float fullScaleTransitionTime05 = 0.5f;
    public static float fullScaleTransitionTime03 = 0.3f;

    public static Vector3 onePointTwo = new Vector3(1.2f, 1.2f, 1.2f);
    public static Vector3 onePointThree = new Vector3(1.3f, 1.3f, 1.3f);
    public static Vector3 onePointSix = new Vector3(1.6f, 1.6f, 1.6f);
    public static Vector3 zeroPointSeven = new Vector3(0.7f, 0.7f, 0.7f);
    public static Vector3 zeroPointThree = new Vector3(0.3f, 0.3f, 0.3f);
    public static Vector3 Z90 = new Vector3(0, 0, 90);

    private static Color itemSlotOutlineColor;
    public static Color ItemSlotOutlineColor
    {
        set
        {
            if (itemSlotOutlineColor == null) itemSlotOutlineColor = value;
        }
        get { return itemSlotOutlineColor; }
    }
}

public static partial class Global
{
    //Resources ���� �� ���

    public const string itemTypeSpritePath = "System/Sprites/ItemType/";

    public const string foodSpritePath = "System/Sprites/Food/";
    public const string ingredientSpritePath = "System/Sprites/Ingredient/";

    public const string foodDataPath = "System/FoodData/";
    public const string ingredientDataPath = "System/IngredientData/";

    public const string TalkWithChef = "TalkWithChef"; //�丮�� NPC�� ��ȭ���� ���� �̺�Ʈ Ű
    public const string MakeFood = "MakeFood"; //������ ������� ���� �̺�Ʈ Ű
    public const string TryAcquisitionItem = "TryAcquisitionItem"; //������ ȹ�� �õ� �� �̺�Ʈ Ű
    public const string AcquisitionItem = "AcquisitionItem"; //������ ȹ�� �� �̺�Ʈ Ű
    public const string JunkItem = "JunkItem"; //������ ���� �� �̺�Ʈ Ű

    private static Dictionary<string, ActionGroup> stringToActionDict = new Dictionary<string, ActionGroup>();

    private static Sprite[] itemTypeSprites = new Sprite[Enum.GetValues(typeof(ItemType)).Length];
}

public static partial class Global
{
    public static Sprite GetItemTypeSpr(ItemType type)
    {
        int index = (int)type;
        if (!itemTypeSprites[index])
            itemTypeSprites[index] = Resources.Load<Sprite>(itemTypeSpritePath + type.ToString());
        return itemTypeSprites[index];
    }

    public static string GetItemTypeName(ItemType type)
    {
        switch (type)
        {
            case ItemType.CONSUME:
                return "����";
            case ItemType.EQUIP:
                return "����";
            case ItemType.ETC:
                return "����ǰ";
        }
        return "";
    }
}

public static partial class Global
{
    public static void AddAction(string key, Action action)
    {
        if (stringToActionDict.ContainsKey(key))
            stringToActionDict[key].voidAction += action;
        else
            stringToActionDict.Add(key, new ActionGroup(action));
    }

    public static void AddMonoAction(string key, Action<MonoBehaviour> action)
    {
        if (stringToActionDict.ContainsKey(key))
            stringToActionDict[key].monoAction += action;
        else
            stringToActionDict.Add(key, new ActionGroup(action));
    }
    public static void AddAction(string key, Action<object> action)
    {
        if (stringToActionDict.ContainsKey(key))
            stringToActionDict[key].objAction += action;
        else
            stringToActionDict.Add(key, new ActionGroup(action));
    }

    public static void RemoveAction(string key, Action action)
    {
        if (stringToActionDict.ContainsKey(key))
            stringToActionDict[key].voidAction -= action;
    }
    public static void RemoveMonoAction(string key, Action<MonoBehaviour> action)
    {
        if (stringToActionDict.ContainsKey(key))
            stringToActionDict[key].monoAction -= action;
    }
    public static void RemoveAction(string key, Action<object> action)
    {
        if (stringToActionDict.ContainsKey(key))
            stringToActionDict[key].objAction -= action;
    }

    public static void ActionTrigger(string key)
    {
        stringToActionDict[key].ActionTrigger();
    }
    public static void MonoActionTrigger(string key, MonoBehaviour mono)
    {
        stringToActionDict[key].ActionTrigger(mono);
    }
    public static void ActionTrigger(string key, object obj)
    {
        stringToActionDict[key].ActionTrigger(obj);
    }

    public static void RemoveKey(string key)
    {
        if (stringToActionDict.ContainsKey(key))
            stringToActionDict.Remove(key);
    }

    public static void RemoveAllKeys() => stringToActionDict.Clear();
}

public static partial class Util
{
    public static Camera mainCam;

    public static string GetFilePath(string fileName) => string.Concat(Application.persistentDataPath, "/", fileName);

    public static void DelayFunc(Action a, float delay, MonoBehaviour mono)
    {
        mono.StartCoroutine(DelayFuncCo(a, delay));
    }

    public static Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
        return mainCam.WorldToScreenPoint(worldPos);
    }

    public static Pair<List<T>, List<T>> GetLists<T>(List<T> list, Func<T, bool> condition)  //����Ʈ�� ���Ƽ� �ش� ���ǿ� �´� �͵��� �̾Ƽ� �ƴ� �͵�� ������ ������
    {
        Pair<List<T>, List<T>> result = new Pair<List<T>, List<T>>();

        for (int i = 0; i < list.Count; i++)
        {
            if (condition(list[i]))
            {
                result.first.Add(list[i]);
            }
            else
            {
                result.second.Add(list[i]);
            }
        }

        return result;
    }

    private static IEnumerator DelayFuncCo(Action func, float delay)
    {
        yield return new WaitForSeconds(delay);
        func?.Invoke();
    }
}

public static partial class ScriptHelper
{
    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
        foreach (var item in list)
        {
            action(item);
        }
    }
    public static List<T> ToList<T>(this IEnumerable<T> a)
    {
        List<T> results = new List<T>();

        foreach (T item in a)
        {
            results.Add(item);
        }

        return results;
    }
    public static Vector2 RandomVector(Vector2 min, Vector2 max)
    {
        float x = UnityEngine.Random.Range(min.x, max.x);
        float y = UnityEngine.Random.Range(min.y, max.y);

        return new Vector2(x, y);
    }
    public static Vector2 Sum(this Vector2 vec1, Vector2 vec2)
    {
        return new Vector2(vec1.x + vec2.x, vec1.y + vec2.y);
    }
    public static Vector3 Sum(this Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
    }
    public static Vector4 Sum(this Vector4 vec1, Vector4 vec2)
    {
        return new Vector4(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z, vec1.w + vec2.w);
    }
    public static Color SetColorAlpha(this Color color, float a)
    {
        Color newColor = color;
        newColor.a = a;

        return newColor;
    }
    public static bool CompareGameObjectLayer(this LayerMask layerMask, GameObject targetObj) // targetObj�� layer�� layerMask�ȿ� �ִ��� üũ
    {
        int layer = 1 << targetObj.layer;

        return layerMask == (layerMask | layer);
    }
    // ---Limit�żҵ忡 ���� ����---
    // value = 0, min = 1, max = 3�� �� 3�� �����Ѵ�.
    // value = -2, min = 1, max = 3�� �� 1�� �����Ѵ�.
    // value = -5, min = 1, max = 3�� �� 1�� �����Ѵ�.
    // value = 4, min = 1, max = 3�� �� 1�� �����Ѵ�.
    // value = 5, min = 1, max = 3�� �� 2�� �����Ѵ�.
    // value = 8, min = 1, max = 3�� �� 2�� �����Ѵ�.
    // value���� min���� max�� ���̸� �պ��Ѵٰ� �����ϸ� �ȴ�.
    // value���� min������ �������� (min - value) ��ŭ�� max������ ���� ���� ���� value���� �����Ѵ�. �� ������ value���� min�� �̻��� �� �� ���� �ݺ��Ѵ�.
    // value���� max������ �������� (value - max) ��ŭ�� min������ ���ؼ� ���� ���� value���� �����Ѵ�. �� ������ value���� max�� ���ϰ� �� �� ���� �ݺ��Ѵ�.
    public static int Limit(this int value, int min, int max)
    {
        if (value < min)
        {
            return Limit(max - (min - value - 1), min, max);
        }
        else if (value > max)
        {
            return Limit(min + (value - max - 1), min, max);
        }
        else
        {
            return value;
        }
    }
}