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
public class Pair<T,U>
{
    public T first;
    public U second;

    public Pair() { }
    public Pair(T t, U u)
    {
        first = t;
        second = u;
    }
}

[Serializable]
public class ItemInfo
{
    public int id;
    public int count;

    public ItemInfo() { }
    public ItemInfo(int id, int count)
    {
        this.id = id;
        this.count = count;
    }
}

public class ActionGroup
{
    public Action voidAction;
    public Action<MonoBehaviour> monoAction;
    public Action<object> objAction;

    public ActionGroup(Action voidAction)
    {
        this.voidAction = voidAction;
    }
    public ActionGroup(Action<MonoBehaviour> monoAction)
    {
        this.monoAction = monoAction;
    }
    public ActionGroup(Action<object> objAction)
    {
        this.objAction = objAction;
    }

    public void ActionTrigger()
    {
        voidAction?.Invoke();
    }
    public void ActionTrigger(MonoBehaviour mono)
    {
        monoAction?.Invoke(mono);
    }
    public void ActionTrigger(object obj)
    {
        objAction?.Invoke(obj);
    }
}
