using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IngredientCount
{
    public Ingredient ingredient; //음식을 만들기 위한 필요 재료
    public int needCount; //음식을 만들기 위해서 필요한 재료 개수
}

[Serializable]
public class TwoList<T,U>
{
    public List<T> firstList = new List<T>();
    public List<U> secondList = new List<U>();
}
