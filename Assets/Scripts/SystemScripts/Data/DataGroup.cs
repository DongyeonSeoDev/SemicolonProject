using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[Serializable]
public struct StageFork
{
    public int stageNumber;
    public AreaType[] nextStageTypes;
}

[Serializable]
public struct MonsterLearningInfo
{
    public bool meet;
    public bool kill;
    public bool assimilation;
    public bool ownProperty;
}

[Serializable]
public class SubtitleData
{
    public SingleSubtitleData[] subData;

    public bool unscaledTime;  //timescale 0�̾ ����Ǵ���

    #region Property
    public string[] Dialogs
    {
        get
        {
            string[] strs = new string[subData.Length];
            for(int i= 0; i < subData.Length; i++)
            {
                strs[i] = subData[i].dialog;
            }
            return strs;
        }
    }
    public float[] SecondPerLits
    {
        get
        {
            float[] fs = new float[subData.Length];
            for(int i= 0; i < subData.Length; i++)
            {
                fs[i] = subData[i].secondPerLit;
            }
            return fs;
        }
    }
    public float[] Durations
    {
        get
        {
            float[] fs = new float[subData.Length];
            for (int i = 0; i < subData.Length; i++)
            {
                fs[i] = subData[i].duration;
            }
            return fs;
        }
    }
    public float[] NextLogIntervals
    {
        get
        {
            float[] fs = new float[subData.Length];
            for (int i = 0; i < subData.Length; i++)
            {
                fs[i] = subData[i].nextLogInterval;
            }
            return fs;
        }
    }

    public string[] EndActionIDArr
    {
        get
        {
            string[] strs = new string[subData.Length];
            for (int i = 0; i < subData.Length; i++)
            {
                strs[i] = subData[i].endActionId;
            }
            return strs;
        }
    }
    #endregion
}

[Serializable]
public class SingleSubtitleData
{
    public string dialog; //���
    public float secondPerLit; //���ڴ� ��� �ӵ�
    public float duration; //��� �� ��µǰ� ��µ� ��� �۾��� ������� ���� �����ִ� �ð�
    public float nextLogInterval; //��簡 �� ��µǰ� ��� �۾� ������� ���� ��簡 ������ �������� ������
    public string endActionId;

    //public bool unscaledTime;  //timescale 0�̾ ����Ǵ���

    /*public SingleSubtitleData() { }
    public SingleSubtitleData(string dialog, float secondPerLit, float duration, float nextLogInterval, string endActionId)
    {
        this.dialog = dialog;
        this.secondPerLit = secondPerLit;
        this.duration = duration;
        this.nextLogInterval = nextLogInterval;
        this.endActionId = endActionId;
    }*/
}

[Serializable]
public class IngredientCount
{
    public Ingredient ingredient; //������ ����� ���� �ʿ� ���
    public int needCount; //������ ����� ���ؼ� �ʿ��� ��� ����
}

/*[Serializable]
public class MonsterInfo
{
    public string id;
    public int understandingRate; //���ص� (��ȭ��)
    public float absorptionRate; //��� Ȯ��
    public bool isSaveBody; //�� ������ ���� �����ߴ���

    public MonsterInfo() { }
    public MonsterInfo(string id, int understandingRate, float absorptionRate)
    {
        this.id = id;
        this.understandingRate = understandingRate;
        this.absorptionRate = absorptionRate;
    }
}*/

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
    public string id;
    public int count;

    public ItemInfo() { }
    public ItemInfo(string id, int count)
    {
        this.id = id;
        this.count = count;
    }
}

public class NoticeUISet
{
    public string msg;
    public float fontSize;
    public bool changeVertexGradient;
    public VertexGradient vg;
    public Action endAction;

    public float existTime;

    public NoticeUISet(string msg, float fontSize, Action endAction, float existTime = 2.5f)
    {
        this.msg = msg;
        this.fontSize = fontSize;
        this.endAction = endAction;
        changeVertexGradient = false;
        this.existTime = existTime;
    }

    public NoticeUISet(string msg, float fontSize,VertexGradient vg ,Action endAction, float existTime = 2.5f)
    {
        this.msg = msg;
        this.fontSize = fontSize;
        this.vg = vg;
        this.endAction = endAction;
        changeVertexGradient = true;
        this.existTime = existTime;
    }
}

public class UIMsgQueue
{
    public Queue<NoticeUISet> noticeQueue = new Queue<NoticeUISet>();
    public bool isNoticing = false;
    public float noticeCheckElapsed;
    public VertexGradient defaultNoticeMsgVG;

    public bool IsCheckTime
    {
        get
        {
            if (noticeCheckElapsed < Time.unscaledTime)
            {
                noticeCheckElapsed = Time.unscaledTime + 0.25f;
                return true;
            }
            return false;
        }
    }

    public bool CanMakeNotice => noticeQueue.Count > 0 && !isNoticing;

    public NoticeUISet NewNotice
    {
        get
        {
            isNoticing = true;
            NoticeUISet nus = noticeQueue.Dequeue();
            nus.endAction += () => isNoticing = false;
            return nus;
        }
    }
}

[Serializable]
public class NPCInfo
{
    public string id;
    public int talkId;

    public string npcName;

    public List<Single<List<TalkElement>>> talkContents;
    public List<Pair<int, string>> dialogsEndEvKeyInfo;
    //public List<string> curDialogsEndEvKey;
    //public List<NPCDiglogSet> talkContents;

    public string CurTalkEndEventKey
    {
        get
        {
            if (dialogsEndEvKeyInfo != null)
            {
                for (int i = 0; i < dialogsEndEvKeyInfo.Count; i++)
                {
                    if (dialogsEndEvKeyInfo[i].first == talkId)
                    {
                        return dialogsEndEvKeyInfo[i].second;
                    }
                }
            }
            return string.Empty;
        }
    }
}

/*[Serializable]
public class NPCDiglogSet
{
    public string talkEndEventKey;
    public List<TalkElement> dialogs;
}*/

[Serializable]
public class TalkElement
{
    [TextArea]
    public string message;
    //public Sprite npcTalkSpr;
    //public AudioClip talkSound;
    //public UnityEvent talkEndEvent;

    public string talkEndEventKey;
}


[Serializable]
public class Single<T>
{
    public T value;

    public Single(T t)
    {
        this.value = t;
    }
}

public class AbsorptionData
{
    public string mobId;

    public float absorptionRate;
    public float assimilationRate;

    public KillNoticeType killNoticeType;

    //public string resMsg;

    public AbsorptionData(string id, float absorption, float assimilation, KillNoticeType type)
    {
        mobId = id;
        absorptionRate = absorption;
        assimilationRate = assimilation;
        killNoticeType = type;
        //resMsg = msg;
    }
}

[Serializable]
public class KeyActionData
{
    public KeyAction keyAction;
    public Sprite keySprite;
}

public class HeadUIData
{
    private Vector3 offset;  //�÷��̾� �Ӹ��κ��� ��ŭ���� ������������
    private Vector3 curOffset;  //���� �������ִ� offset
    private RectTransform headRt;  //UI RectTrm  

    private bool twComp; // true�� ������� �ִ� ����
    private float headUIOffTime;  //�Ӹ� �� UI�� ������� �ð�

    private float moveSpeed;

    private event Action uiOffEvent; 

    public void Update()
    {
        if (headRt.gameObject.activeSelf)
        {
            Transform target = Global.GetSlimePos;
            if (target)
            {
                headRt.anchoredPosition = Util.WorldToScreenPosForScreenSpace(target.position + curOffset, Util.WorldCvs);
            }

            if (!twComp && Time.time > headUIOffTime && !KeyActionManager.Instance.IsNoticingGetMove)
            {
                twComp = true;
                uiOffEvent?.Invoke();
            }

            curOffset.y += Time.deltaTime * (!twComp ? moveSpeed : -moveSpeed);
            curOffset.y = Mathf.Clamp(curOffset.y, 1, offset.y);
        }
    }

    public HeadUIData(RectTransform rt, Vector3 offset, float speed = 1.5f)
    {
        this.offset = offset;
        headRt = rt;
        moveSpeed = speed;
    }

    public void Set(float duration, Vector2 startOffset, Action compAction)
    {
        headUIOffTime = Time.time + duration;
        curOffset = startOffset;
        twComp = false;

        uiOffEvent = compAction;
        headRt.gameObject.SetActive(true);
    }
}

[Serializable]
public class InitAcquisitionData
{
    public string objName;
    public string explanation;
    public string addiExplanation;
    public Sprite UISpr;
    public UnityEvent endEvent;
}

namespace Water
{

    [Serializable]
    public class CheckGameStringKeys
    {
        public List<string> poolKeyList = new List<string>();
        public List<Pair<string, EventKeyCheck>> eventKeyList = new List<Pair<string, EventKeyCheck>>();
    }

    public class ActionGroup
    {
        public Action<MonoBehaviour> monoAction;
        public Action<object> objAction;

        public EventKeyCheck ekc;


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


        public void ActionTrigger(MonoBehaviour mono)
        {
            monoAction?.Invoke(mono);
        }
        public void ActionTrigger(object obj)
        {
            objAction?.Invoke(obj);
        }
    }


    [Serializable]
    public class PoolBaseData
    {
        public GameObject poolObj;
        public Transform parent;
        public int defaultSpawnCount;
        public string poolKey;
    }
}