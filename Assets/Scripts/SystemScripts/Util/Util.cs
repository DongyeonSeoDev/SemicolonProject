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

        public static TwoList<T,T> GetLists<T>(List<T> list, Func<T,bool> condition)  //����Ʈ�� ���Ƽ� �ش� ���ǿ� �´� �͵��� �̾Ƽ� �ƴ� �͵�� ������ ������
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