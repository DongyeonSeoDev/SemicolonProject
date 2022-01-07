using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IngredientCount
{
    public Ingredient ingredient; //������ ����� ���� �ʿ� ���
    public int needCount; //������ ����� ���ؼ� �ʿ��� ��� ����
}

[Serializable]
public class TwoList<T,U>
{
    public List<T> firstList = new List<T>();
    public List<U> secondList = new List<U>();
}
