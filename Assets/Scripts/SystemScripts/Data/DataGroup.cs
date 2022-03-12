using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[Serializable]
public class IngredientCount
{
    public Ingredient ingredient; //음식을 만들기 위한 필요 재료
    public int needCount; //음식을 만들기 위해서 필요한 재료 개수
}

[Serializable]
public class MonsterInfo
{
    public string id;
    public int understandingRate; //이해도 (동화율)
    public float absorptionRate; //흡수 확률
    public bool isSaveBody; //이 몬스터의 몸을 저장했는지

    public MonsterInfo() { }
    public MonsterInfo(string id, int understandingRate, float absorptionRate)
    {
        this.id = id;
        this.understandingRate = understandingRate;
        this.absorptionRate = absorptionRate;
    }
}

[Serializable]
public class SkillInfo
{
    public SkillType skillType;

    public string skillName;
    [TextArea] public string skillExplanation;

    public Sprite skillSpr;
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
public class Triple<A,B,C>
{
    public A first;
    public B second;
    public C third;

    public Triple() { }
    public Triple(A a, B b, C c)
    {
        first = a;
        second = b;
        third = c;
    }
}

[Serializable]
public class GameUIFields
{
    public RectTransform rectTrm;
    public Vector3 originPos;
    public CanvasGroup cvsg;
    public UIType _UItype;
    public GameUI childGameUI;
    public Transform transform;
    public GameUI self;
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

[Serializable]
public class NoticeUISet
{
    public string msg;
    public float fontSize;
    public bool changeVertexGradient;
    public VertexGradient vg;
    public Action endAction;

    public NoticeUISet(string msg, float fontSize, Action endAction)
    {
        this.msg = msg;
        this.fontSize = fontSize;
        this.endAction = endAction;
        changeVertexGradient = false;
    }

    public NoticeUISet(string msg, float fontSize,VertexGradient vg ,Action endAction)
    {
        this.msg = msg;
        this.fontSize = fontSize;
        this.vg = vg;
        this.endAction = endAction;
        changeVertexGradient = true;
    }
}

[Serializable]
public class NPCInfo
{
    public short id;
    public short talkId;

    public string npcName;
    public List<Pair<List<string>, List<TalkEffect>>> talkContents; 
}

[Serializable]
public class TalkEffect
{
    //example
    public Sprite npcTalkSpr;
    public AudioClip talkSound;
    public UnityEvent talkEvent;
}



[Serializable]
public class CheckGameStringKeys
{
    public List<string> poolKeyList = new List<string>();
    public List<Pair<string,EventKeyCheck>> eventKeyList = new List<Pair<string, EventKeyCheck>>();
}

public class ActionGroup
{
    public Action voidAction;
    public Action<MonoBehaviour> monoAction;
    public Action<object> objAction;

    public EventKeyCheck ekc;

    public ActionGroup(Action voidAction)
    {
        this.voidAction = voidAction;
        ekc = EventKeyCheck.VOID;
    }
    public ActionGroup(Action<MonoBehaviour> monoAction)
    {
        this.monoAction = monoAction;
        ekc = EventKeyCheck.MONO;
    }
    public ActionGroup(Action<object> objAction)
    {
        this.objAction = objAction;
        ekc = EventKeyCheck.OBJECT;
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