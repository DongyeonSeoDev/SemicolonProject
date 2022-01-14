using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

namespace Water
{
    public static class Util
    {

        public static string GetFilePath(string fileName) => string.Concat(Application.persistentDataPath, "/", fileName);

        public static void ExecuteListInList<T>(List<T> allList, List<T> includedList, Action<T> include, Action<T> notInclude) //ù��° ���� ����Ʈ�߿� �ι�° ���� ����Ʈ�� ���ԵǾ� ���� �� ������ �Ͱ� ���Ե��� ���� �͵��� ó��
        {
            allList.ForEach(x =>
            {
                if(includedList.Contains(x))
                    include?.Invoke(x);
                else
                    notInclude?.Invoke(x);
                
            });
        }

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

        public const string foodSpritePath = "System/Sprites/Food/";
        public const string ingredientSpritePath = "System/Sprites/Ingredient/";

        public const string foodDataPath = "System/FoodData/";
        public const string ingredientDataPath = "System/IngredientData/";

        public const string TalkWithChef = "TalkWithChef"; //�丮�� NPC�� ��ȭ���� ���� �̺�Ʈ Ű
        public const string MakeFood = "MakeFood"; //������ ������� ���� �̺�Ʈ Ű

        private static Dictionary<string, ActionGroup> stringToActionDict = new Dictionary<string, ActionGroup>();
    }


    public partial class Global
    {
        public static void AddAction(string key, Action action)
        {
            if (stringToActionDict.ContainsKey(key))
                stringToActionDict[key].voidAction += action;
            else
                stringToActionDict.Add(key, new ActionGroup(action));
        }

        public static void AddAction(string key, Action<MonoBehaviour> action)
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
        public static void RemoveAction(string key, Action<MonoBehaviour> action)
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
        public static void ActionTrigger(string key, MonoBehaviour mono)
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
    }
}