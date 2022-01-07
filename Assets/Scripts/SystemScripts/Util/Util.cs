using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

        public static TwoList<T,T> GetLists<T>(List<T> list, Func<T,bool> condition)
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
}