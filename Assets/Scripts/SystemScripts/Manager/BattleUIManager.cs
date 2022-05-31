using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Water;
using DG.Tweening;

public class AbsorptionData
{
    public string mobId;

    public float absorptionRate;
    public float assimilationRate;

    //public string resMsg;

    public AbsorptionData(string id , float absorption , float assimilation)
    {
        mobId = id;
        absorptionRate = absorption;
        assimilationRate = assimilation;
        //resMsg = msg;
    }
}

public class BattleUIManager : MonoSingleton<BattleUIManager>   
{
    #region Monster AbsorptionRate UI of Right
    private List<AbsorptionNotice> curNoticeList;
    private Queue<AbsorptionData> absorptionDataQueue;
    private Queue<AbsorptionNotice> endedAbspNoticeQueue;
    private float nextAbsorptionNoticeTime;
    private float nextEndedNoticeDeleteTime;

    [SerializeField] private List<RectTransform> absorptionNoticeRTList;
    [SerializeField] private Vector2 abspNoticeStartOffset;
    [SerializeField] private float absorptionNoticeDelay = 0.2f;
    [SerializeField] private float absorptionNoticeTime = 2f;

    private bool isMoving;
    public bool IsMoving
    {
        set => isMoving = value; 
    }
    private Vector2 noticeDeletePos;
    private TweenCallback noticeEndTCB;

    //private Dictionary<string, List<Pair<float, bool>>> assimNoticeCheckDic = new Dictionary<string, List<Pair<float, bool>>>();
    private Dictionary<string, List<bool>> assimNoticeCheckDic = new Dictionary<string, List<bool>>();
    [SerializeField] private float assimNoticeInterval = 10f;

    #endregion

    //흡수율 표시창에서 비어있는 다음 칸과 인덱스 가져옴
    public (int, RectTransform) NextEmptyNoticeSlot
    {
        get
        {
            for (int i = 0; i < absorptionNoticeRTList.Count; i++)
            {
                if (!absorptionNoticeRTList[i].gameObject.activeSelf)
                {
                    absorptionNoticeRTList[i].gameObject.SetActive(true);
                    return (i, absorptionNoticeRTList[i]);
                }
            }
            return (-1, null);
        }
    }

    private void Awake()
    {
        curNoticeList = new List<AbsorptionNotice>();
        absorptionDataQueue = new Queue<AbsorptionData>();
        endedAbspNoticeQueue = new Queue<AbsorptionNotice>();
        noticeDeletePos = absorptionNoticeRTList[0].anchoredPosition + new Vector2(0, 200);
        noticeEndTCB = () =>
        {
            curNoticeList[0].gameObject.SetActive(false);
            curNoticeList.RemoveAt(0);

            for(int i=0; i< absorptionNoticeRTList.Count - curNoticeList.Count; i++)
            {
                absorptionNoticeRTList[absorptionNoticeRTList.Count - 1 - i].gameObject.SetActive(false);
            }
            isMoving = false;
        };

        EventManager.StartListening("PlayerDead", () =>
        {
            int i;
            foreach(List<bool> assimChkList in assimNoticeCheckDic.Values)
            {
                for(i=0; i< assimChkList.Count; i++)
                {
                    assimChkList[i] = false;
                }
            }
        });
    }

    private void Start()
    {
        float i, maxAssimRate = PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate;
        foreach(Enemy.EnemyType type in Global.GetEnumArr<Enemy.EnemyType>())
        {
            List<bool> li = new List<bool>();
            for(i = assimNoticeInterval; i<= maxAssimRate; i+= assimNoticeInterval)
            {
                li.Add(false);
            }
            assimNoticeCheckDic.Add(type.ToString(), li);
        }
    }

    private void Update()
    {
        if(nextAbsorptionNoticeTime < Time.time)
        {
            nextAbsorptionNoticeTime = Time.time + absorptionNoticeDelay;
            if(absorptionDataQueue.Count > 0 && !absorptionNoticeRTList[absorptionNoticeRTList.Count - 1].gameObject.activeSelf && !isMoving)
            {
                isMoving = true;
                (int, RectTransform) slot = NextEmptyNoticeSlot;
                AbsorptionNotice an = PoolManager.GetItem<AbsorptionNotice>("ApsorptionNotice");
                curNoticeList.Add(an);
                an.ShowNotice(slot.Item2.anchoredPosition + abspNoticeStartOffset, slot.Item2.anchoredPosition, absorptionDataQueue.Dequeue(), slot.Item1);
            }
        }

        if(nextEndedNoticeDeleteTime < Time.time)
        {
            nextEndedNoticeDeleteTime = Time.time + 0.35f;
            if(endedAbspNoticeQueue.Count > 0 && !isMoving)
            {
                isMoving = true;
                RectTransform rt = endedAbspNoticeQueue.Dequeue().GetComponent<RectTransform>();

                rt.GetComponent<CanvasGroup>().DOFade(0, 0.27f);
                rt.DOAnchorPos(noticeDeletePos, 0.27f).OnComplete(noticeEndTCB);

                for(int i=1; i<curNoticeList.Count; i++)
                {
                    curNoticeList[i].GetComponent<RectTransform>().DOAnchorPos(absorptionNoticeRTList[i-1].anchoredPosition, 0.27f);
                }
            }
        }
    }

    public void InsertAbsorptionInfo(string id, float absorptionRate, float assimilationRate)  //새로 흡수율 알림 UI를 보여줄 정보를 큐에 넣어줌
    {
        if(assimilationRate > 0f && assimilationRate <= PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate)
        {
            bool needMsg = false;
            if (assimilationRate >= assimNoticeInterval)
            {
                int index = (int)(assimilationRate / assimNoticeInterval) - 1;
                if (!assimNoticeCheckDic[id][index])
                {
                    assimNoticeCheckDic[id][index] = true;
                    needMsg = true;
                }
            }
            
            if (!needMsg) return;
        }

        absorptionDataQueue.Enqueue(new AbsorptionData(id, absorptionRate, assimilationRate));
    }

    public void InsertEndedNotice(AbsorptionNotice ui)  //흡수율 표시창에서 이제 알림을 사라지게 할 요소를 큐에 넣어줌
    {
        endedAbspNoticeQueue.Enqueue(ui);
    }

    public bool HasBody(string id)  //어떤 몬스터의 몸을 가지고 있는 상태인가? (변신할 수 있는 몹 슬롯에 넣어져있는지)
    {
        foreach(string key in PlayerEnemyUnderstandingRateManager.Instance.MountedObjList)
        {
            if (id == key) return true;
        }
        return false;
    }
}
