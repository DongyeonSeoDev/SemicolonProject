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
        [MenuItem("Resource Manage/Set ID")]
        static void SetResourceID()
        {
            int index = 0;
            int offset = 5;

            foreach (Food food in Resources.LoadAll<Food>("System/FoodData/"))
            {
                food.id = index;
                index += offset;
            }

            foreach (Ingredient ingredient in Resources.LoadAll<Ingredient>("System/IngredientData/"))
            {
                ingredient.id = index;
                index += offset;
            }
        }

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

        public static TwoList<T,T> GetLists<T>(List<T> list, Func<T,bool> condition)  //리스트를 돌아서 해당 조건에 맞는 것들을 뽑아서 아닌 것들과 나눠서 리턴함
        {
            TwoList<T, T> result = new TwoList<T, T>();

            for(int i=0; i<list.Count; i++)
            {
                if (condition(list[i]))
                {
                    result.firstList.Add(list[i]);
                }
                else
                {
                    result.secondList.Add(list[i]);
                }
            }

            return result;
        }
    }

    public class Global
    {
        public const string foodSpritePath = "System/Sprites/Food/";
        public const string ingredientSpritePath = "System/Sprites/Ingredient/";

       
    }
}