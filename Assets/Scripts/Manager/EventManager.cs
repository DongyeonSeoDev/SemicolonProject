using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EventManager
{
    #region Dictionary 선언
    private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();
    private static Dictionary<string, Action<float>> float_eventDictionary = new Dictionary<string, Action<float>>();
    private static Dictionary<string, Action<float, float, float>> float_float_float_eventDictionary = new Dictionary<string, Action<float, float, float>>();
    private static Dictionary<string, Action<string>> str_eventDictionary = new Dictionary<string, Action<string>>();
    private static Dictionary<string, Action<string, bool>> str_bool_eventDictionary = new Dictionary<string, Action<string, bool>>();
    private static Dictionary<string, Action<string, int>> str_int_eventDictionary = new Dictionary<string, Action<string, int>>();
    private static Dictionary<string, Action<Vector2>> vec2_EventDictionary = new Dictionary<string, Action<Vector2>>();
    private static Dictionary<string, Action<Vector2, float, float>> vec2_float_float_eventDictionary = new Dictionary<string, Action<Vector2, float, float>>();
    private static Dictionary<string, Action<GameObject>> gmo_EventDictionary = new Dictionary<string, Action<GameObject>>();
    private static Dictionary<string, Action<GameObject, int>> gmo_int_EventDictionary = new Dictionary<string, Action<GameObject, int>>();
    #endregion

    #region StargetListening함수
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
    public static void StartListening(string eventName, Action<float> listener)
    {
        Action<float> thisEvent;

        if (float_eventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            float_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            float_eventDictionary.Add(eventName, listener);
        }
    }
    public static void StartListening(string eventName, Action<float, float, float> listener)
    {
        Action<float, float, float> thisEvent;

        if (float_float_float_eventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            float_float_float_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            float_float_float_eventDictionary.Add(eventName, listener);
        }
    }
    public static void StartListening(string eventName, Action<string> listener)
    {
        Action<string> thisEvent;

        if (str_eventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            str_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            str_eventDictionary.Add(eventName, listener);
        }
    }
    public static void StartListening(string eventName, Action<string, bool> listener)
    {
        Action<string, bool> thisEvent;

        if (str_bool_eventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            str_bool_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            str_bool_eventDictionary.Add(eventName, listener);
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
    public static void StartListening(string eventName, Action<Vector2, float, float> listener)
    {
        Action<Vector2, float, float> thisEvent;

        if (vec2_float_float_eventDictionary.TryGetValue(eventName, out thisEvent)) // ���� �̸��� DIctionary�ִ��� üũ
        {
            thisEvent += listener;                   // ���� �̸����� �� ����
            vec2_float_float_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            vec2_float_float_eventDictionary.Add(eventName, listener);
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
    #endregion
    #region StopListening 함수
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
    public static void StopListening(string eventName, Action<float> listener)
    {
        Action<float> thisEvent;

        if (float_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            float_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            float_eventDictionary.Remove(eventName);
        }
    }
    public static void StopListening(string eventName, Action<float, float, float> listener)
    {
        Action<float, float, float> thisEvent;

        if (float_float_float_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            float_float_float_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            float_float_float_eventDictionary.Remove(eventName);
        }
    }
    public static void StopListening(string eventName, Action<string> listener)
    {
        Action<string> thisEvent;

        if (str_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            str_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            str_eventDictionary.Remove(eventName);
        }
    }
    public static void StopListening(string eventName, Action<string, bool> listener)
    {
        Action<string, bool> thisEvent;

        if (str_bool_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            str_bool_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            str_bool_eventDictionary.Remove(eventName);
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
    public static void StopListening(string eventName, Action<Vector2, float, float> listener)
    {
        Action<Vector2, float, float> thisEvent;

        if (vec2_float_float_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            vec2_float_float_eventDictionary[eventName] = thisEvent;
        }
        else
        {
            vec2_float_float_eventDictionary.Remove(eventName);
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
    #endregion
    #region TriggerEvent 함수
    public static void TriggerEvent(string eventName)
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke();
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, float float_param)
    {
        Action<float> thisEvent;

        if (float_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(float_param);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, float float_param1, float float_param2, float float_param3)
    {
        Action<float, float, float> thisEvent;

        if (float_float_float_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(float_param1, float_param2, float_param3);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, string str_param)
    {
        Action<string> thisEvent;

        if (str_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(str_param);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, string str_param, bool bool_param)
    {
        Action<string, bool> thisEvent;

        if (str_bool_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(str_param, bool_param);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, string str_param, int int_param)
    {
        Action<string, int> thisEvent;

        if (str_int_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(str_param, int_param);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, Vector2 param)
    {
        Action<Vector2> thisEvent;

        if (vec2_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(param);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, Vector2 vec2_param, float float_param1, float float_param2)
    {
        Action<Vector2, float, float> thisEvent;

        if (vec2_float_float_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(vec2_param, float_param1, float_param2);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, GameObject param)
    {
        Action<GameObject> thisEvent;

        if (gmo_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(param);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    public static void TriggerEvent(string eventName, GameObject gmo_param, int int_param)
    {
        Action<GameObject, int> thisEvent;

        if (gmo_int_EventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(gmo_param, int_param);
        }
        else
        {
            Debug.LogWarning("The Linked ActionsNum is zero of The '" + eventName + "' Event but you tried 'TriggerEvent'");
        }
    }
    #endregion
}
