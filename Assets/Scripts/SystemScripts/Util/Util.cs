using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

namespace Water
{
    public static class KeySetting
    {
        public static Dictionary<KeyAction, KeyCode> keyDict = new Dictionary<KeyAction, KeyCode>();

        public static void SetDefaultKeySetting()
        {
            keyDict.Add(KeyAction.INVENTORY, KeyCode.I);
            
        }
    }

    public static class Util
    {

        public static string GetFilePath(string fileName) => string.Concat(Application.persistentDataPath, "/", fileName);

        public static Pair<List<T>,List<T>> GetLists<T>(List<T> list, Func<T,bool> condition)  //����Ʈ�� ���Ƽ� �ش� ���ǿ� �´� �͵��� �̾Ƽ� �ƴ� �͵�� ������ ������
        {
            Pair<List<T>, List<T>> result = new Pair<List<T>, List<T>>();

            for(int i=0; i<list.Count; i++)
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
    }


    public partial class Global
    {
        //Resources ���� �� ���

        public const string itemTypeSpritePath = "System/Sprites/ItemType/";

        public const string foodSpritePath = "System/Sprites/Food/";
        public const string ingredientSpritePath = "System/Sprites/Ingredient/";

        public const string foodDataPath = "System/FoodData/";
        public const string ingredientDataPath = "System/IngredientData/";

        public const string TalkWithChef = "TalkWithChef"; //�丮�� NPC�� ��ȭ���� ���� �̺�Ʈ Ű
        public const string MakeFood = "MakeFood"; //������ ������� ���� �̺�Ʈ Ű
        public const string AcquisitionItem = "AcquisitionItem"; //������ ȹ�� �� �̺�Ʈ Ű

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
            switch(type)
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
}