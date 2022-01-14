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
