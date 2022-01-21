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

        public static Pair<List<T>,List<T>> GetLists<T>(List<T> list, Func<T,bool> condition)  //리스트를 돌아서 해당 조건에 맞는 것들을 뽑아서 아닌 것들과 나눠서 리턴함
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
        //Resources 폴더 속 경로

        public const string itemTypeSpritePath = "System/Sprites/ItemType/";

        public const string foodSpritePath = "System/Sprites/Food/";
        public const string ingredientSpritePath = "System/Sprites/Ingredient/";

        public const string foodDataPath = "System/FoodData/";
        public const string ingredientDataPath = "System/IngredientData/";

        public const string TalkWithChef = "TalkWithChef"; //요리사 NPC와 대화했을 때의 이벤트 키
        public const string MakeFood = "MakeFood"; //음식을 만들었을 때의 이벤트 키
        public const string AcquisitionItem = "AcquisitionItem"; //아이템 획득 시 이벤트 키

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
                    return "음식";
                case ItemType.EQUIP:
                    return "마석";
                case ItemType.ETC:
                    return "전리품";
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