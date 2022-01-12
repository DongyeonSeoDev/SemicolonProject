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

    public class Global
    {
        public const string foodSpritePath = "System/Sprites/Food/";
        public const string ingredientSpritePath = "System/Sprites/Ingredient/";

       
    }
}