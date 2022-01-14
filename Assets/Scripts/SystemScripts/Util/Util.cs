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

        public static void ExecuteListInList<T>(List<T> allList, List<T> includedList, Action<T> include, Action<T> notInclude) //첫번째 인자 리스트중에 두번째 인자 리스트가 포함되어 있을 때 포함한 것과 포함되지 않은 것들의 처리
        {
            allList.ForEach(x =>
            {
                if(includedList.Contains(x))
                    include?.Invoke(x);
                else
                    notInclude?.Invoke(x);
                
            });
        }

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

        public const string foodSpritePath = "System/Sprites/Food/";
        public const string ingredientSpritePath = "System/Sprites/Ingredient/";

        public const string foodDataPath = "System/FoodData/";
        public const string ingredientDataPath = "System/IngredientData/";

        public const string TalkWithChef = "TalkWithChef"; //요리사 NPC와 대화했을 때의 이벤트 키
        public const string MakeFood = "MakeFood"; //음식을 만들었을 때의 이벤트 키

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