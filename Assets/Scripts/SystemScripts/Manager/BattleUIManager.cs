using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Water;
using DG.Tweening;

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

    #region Mission

    public CanvasGroup missionCvsg;
    public TextMeshProUGUI missionContent;
    private RectTransform missionPanelRt;
    private Vector2 missionPanelPos;
    private Dictionary<MissionType, Mission> allMissionsDic = new Dictionary<MissionType, Mission>();
    private List<Mission> currentMissions = new List<Mission>();
    private Pair<MissionType, short> prevMission = new Pair<MissionType, short>(MissionType.NONE, 0);

    #endregion


    //흡수율 표시창에서 비어있는 다음 칸과 인덱스 가져옴
    public RectTransform NextEmptyNoticeSlot
    {
        get
        {
            for (int i = 0; i < absorptionNoticeRTList.Count; i++)
            {
                if (!absorptionNoticeRTList[i].gameObject.activeSelf)
                {
                    absorptionNoticeRTList[i].gameObject.SetActive(true);
                    return absorptionNoticeRTList[i];
                }
            }
            return null;
        }
    }

    private void Awake()
    {
        #region Value Setting
        curNoticeList = new List<AbsorptionNotice>();
        absorptionDataQueue = new Queue<AbsorptionData>();
        endedAbspNoticeQueue = new Queue<AbsorptionNotice>();
        noticeDeletePos = absorptionNoticeRTList[0].anchoredPosition + new Vector2(0, 200);
        missionPanelRt = missionCvsg.GetComponent<RectTransform>();
        missionPanelPos = missionPanelRt.anchoredPosition;
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
        #endregion

        #region Event Setting
        EventManager.StartListening("PlayerDead", () =>
        {
            DeleteAllMissions();
            int i;
            foreach(List<bool> assimChkList in assimNoticeCheckDic.Values)
            {
                for(i=0; i< assimChkList.Count; i++)
                {
                    assimChkList[i] = false;
                }
            }
        });
        EventManager.StartListening("StartNextStage", EnteredMonsterArea);
        EventManager.StartListening("GotoNextStage_LoadingStart", DeleteAllMissions);
        #endregion

        #region Mission Setting
        allMissionsDic.Add(MissionType.ALLKILL, new AllKillMission("모든 적을 처치하세요"));
        allMissionsDic.Add(MissionType.NOTRANSFORMATION, new NoTransformationMission("변신하지 않고 클리어하세요"));
        allMissionsDic.Add(MissionType.NOQUIKSLOT, new NoQuikSlotMission("퀵슬롯을 사용하지 않고 클리어하세요"));
        allMissionsDic.Add(MissionType.ALLABSORPTION, new AllAbsorptionMission("모든 적을 흡수하세요"));
        //allMissionsDic.Add(MissionType.SURVIVAL, new SurvivalMission("30초 동안 살아남으세요", 30f));
        #endregion
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
                RectTransform slot = NextEmptyNoticeSlot;
                AbsorptionNotice an = PoolManager.GetItem<AbsorptionNotice>("ApsorptionNotice");
                curNoticeList.Add(an);
                an.ShowNotice(slot.anchoredPosition + abspNoticeStartOffset, slot.anchoredPosition, absorptionDataQueue.Dequeue());
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

        for(int i=0; i<currentMissions.Count; i++)
        {
            currentMissions[i].Update();
            if(currentMissions[i].isEnd)
            {
                currentMissions[i].End();
                DisableMission(i);
                i--;
            }
        }
    }

    public void InsertAbsorptionInfo(string id, float absorptionRate, float assimilationRate, KillNoticeType type = KillNoticeType.FAIL)  //새로 흡수율 알림 UI를 보여줄 정보를 큐에 넣어줌
    {
        if( type == KillNoticeType.UNDERSTANDING && assimilationRate <= PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate)
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

        absorptionRate = Mathf.Clamp(absorptionRate, 0f, 100f);
        absorptionDataQueue.Enqueue(new AbsorptionData(id, absorptionRate, assimilationRate, type));
    }

    public void InsertEndedNotice(AbsorptionNotice ui)  //흡수율 표시창에서 이제 알림을 사라지게 할 요소를 큐에 넣어줌
    {
        endedAbspNoticeQueue.Enqueue(ui);
    }

    
    public void EnteredMonsterArea()
    {
        if (StageManager.Instance.CurrentAreaType == AreaType.MONSTER && StageManager.Instance.CurrentStageData.missionTypes.Count > 0)
        {
            Mission ms = allMissionsDic[StageManager.Instance.CurrentStageData.missionTypes[Random.Range(0, StageManager.Instance.CurrentStageData.missionTypes.Count)]];

            //랜덤 인카운터
            if (ms.missionType != prevMission.first)
            {
                prevMission.first = ms.missionType;
                prevMission.second = 0;
            }
            else if(++prevMission.second >= 2)
            {
                if(StageManager.Instance.CurrentStageData.missionTypes.Count > 1)
                {
                    ms = allMissionsDic[StageManager.Instance.CurrentStageData.missionTypes.FindRandom(x=>x!=ms.missionType)];
                }
            }

            currentMissions.Add(ms);
            ms.Start();

            missionPanelRt.DOKill();

            missionContent.text = ms.missionName;
            missionCvsg.alpha = 0;
            missionPanelRt.anchoredPosition = missionPanelPos + new Vector2(200, 0);
            missionCvsg.gameObject.SetActive(true);

            missionCvsg.DOFade(1, 0.4f);
            missionPanelRt.DOAnchorPos(missionPanelPos, 0.4f).SetEase(Ease.OutQuart);
        }
    }

    public void DisableMission(int index = -1)
    {
        if(index >= 0)
           currentMissions.RemoveAt(index);

        missionPanelRt.DOKill();
        missionCvsg.DOFade(0, 0.4f);
        missionPanelRt.DOAnchorPos(missionPanelPos + new Vector2(200, 0), 0.4f).OnComplete(() => missionCvsg.gameObject.SetActive(false));
    }

    private void DeleteAllMissions()
    {
        if (currentMissions.Count > 0)
        {
            for (int i = 0; i < currentMissions.Count; i++)
            {
                currentMissions[i].End(true);
            }
            currentMissions.Clear();
            DisableMission();
        }
    }

    public void ShakeMissionPanel(float duration = 0.6f, float strength = 10f)
    {
        missionPanelRt.DOShakeAnchorPos(duration, strength);
    }
}
