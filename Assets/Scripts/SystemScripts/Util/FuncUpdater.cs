using System;

public static class FuncUpdater
{
    public static void Add(Action action)
    {
        GameManager.Instance.UpdateEvent += action;
    }

    public static void Remove(Action action)
    {
        GameManager.Instance.UpdateEvent -= action;
    }

    public static void Add(string key, Action action)
    {
        GameManager.Instance.AddUpdateAction(key, action);
    }
    public static void Remove(string key, Action action)
    {
        GameManager.Instance.RemoveUpdateAction(key, action);
    }

    public static void Remove(string key)
    {
        GameManager.Instance.RemoveUpdateAction(key);   
    }
}
