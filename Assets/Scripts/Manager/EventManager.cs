using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EventManager
{
    private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();
    private static Dictionary<string, Action<string, int>> str_int_eventDictionary = new Dictionary<string, Action<string, int>>();
    private static Dictionary<string, Action<Vector2>> vec2_EventDictionary = new Dictionary<string, Action<Vector2>>();
    private static Dictionary<string, Action<GameObject>> gmo_EventDictionary = new Dictionary<string, Action<GameObject>>();
    private static Dictionary<string, Action<GameObject, int>> gmo_int_EventDictionary = new Dictionary<string, Action<GameObject, int>>();

    public static void StartListening(string eventName, Action listener)
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Add(eventName, listener);
        }
    }
    public static void StartListening(string eventName, Action<string, int> listener)
    {
        Action<string, int> thisEvent;

        if (str_int_eventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            str_int_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            str_int_eventDictionary.Add(eventName, listener);
        }
    }
    public static void StartListening(string eventName, Action<Vector2> listener)
    {
        Action<Vector2> thisEvent;

        if (vec2_EventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            vec2_EventDictionary[eventName] = thisEvent;
        }
        else
        {
            vec2_EventDictionary.Add(eventName, listener);
        }
    }
    public static void StartListening(string eventName, Action<GameObject> listener)
    {
        Action<GameObject> thisEvent;

        if (gmo_EventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            gmo_EventDictionary[eventName] = thisEvent;
        }
        else
        {
            gmo_EventDictionary.Add(eventName, listener);
        }
    }
    public static void StartListening(string eventName, Action<GameObject, int> listener)
    {
        Action<GameObject, int> thisEvent;

        if (gmo_int_EventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            gmo_int_EventDictionary[eventName] = thisEvent;
        }
        else
        {
            gmo_int_EventDictionary.Add(eventName, listener);
        }
    }

    public static void StopListening(string eventName, Action listener)
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Remove(eventName);
        }
    }
    public static void StopListening(string eventName, Action<string, int> listener)
    {
        Action<string, int> thisEvent;

        if (str_int_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            str_int_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            str_int_eventDictionary.Remove(eventName);
        }
    }
    public static void StopListening(string eventName, Action<Vector2> listener)
    {
        Action<Vector2> thisEvent;

        if (vec2_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            vec2_EventDictionary[eventName] = thisEvent;
        }
        else
        {
            vec2_EventDictionary.Remove(eventName);
        }
    }
    public static void StopListening(string eventName, Action<GameObject> listener)
    {
        Action<GameObject> thisEvent;

        if (gmo_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            gmo_EventDictionary[eventName] = thisEvent;
        }
        else
        {
            gmo_EventDictionary.Remove(eventName);
        }
    }
    public static void StopListening(string eventName, Action<GameObject, int> listener)
    {
        Action<GameObject, int> thisEvent;

        if (gmo_int_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            gmo_int_EventDictionary[eventName] = thisEvent;
        }
        else
        {
            gmo_int_EventDictionary.Remove(eventName);
        }
    }
    public static void TriggerEvent(string eventName)
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke();
        }
    }
    public static void TriggerEvent(string eventName, string str_param, int int_param)
    {
        Action<string, int> thisEvent;

        if (str_int_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(str_param, int_param);
        }
    }
    public static void TriggerEvent(string eventName, Vector2 param)
    {
        Action<Vector2> thisEvent;

        if (vec2_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(param);
        }
    }
    public static void TriggerEvent(string eventName, GameObject param)
    {
        Action<GameObject> thisEvent;

        if (gmo_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(param);
        }
    }
    public static void TriggerEvent(string eventName, GameObject gmo_param, int int_param)
    {
        Action<GameObject, int> thisEvent;

        if (gmo_int_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(gmo_param, int_param);
        }
    }
}
