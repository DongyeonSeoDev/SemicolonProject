using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Water;
using DG.Tweening;
using System.Linq;

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
    [SerializeField] private float assimNoticeInterval = 25f;

    #endregion

    #region Mission

    public CanvasGroup missionCvsg;
    public TextMeshProUGUI missionContent;
    public GameObject missionTimeFillObj;
    public Image missionTimeFill;
    [SerializeField] private MulSpriteColorCtrl mscCtrl;  //미션 패널 테두리 관련 쉐이더 컨트롤러 스크립트
    [SerializeField][ColorUsage(true, true)] private Color[] missionLvColors;  //미션 난이도에 따른 미션 패널의 테두리 색상
    private RectTransform missionPanelRt;
    private Vector2 missionPanelPos;

    private Dictionary<MissionType, Mission> allMissionsDic = new Dictionary<MissionType, Mission>();

    private Dictionary<MissionType, int> missionWeightDic = new Dictionary<MissionType, int>();  //각 미션마다 뜰 확률 (가중치)
    private List<MissionType> zeroWeightMsTypeList = new List<MissionType>();  //가중치가 0이된 미션들
    private List<Mission> currentMissions = new List<Mission>();
    private Pair<MissionType, short> prevMission = new Pair<MissionType, short>(MissionType.NONE, 0);

    private readonly int[,] msLvRate = new int[2,3] { {70, 95, 100} , { 60, 90, 100 } };  //층별 미션 난이도 선택 확률 (가중치 70 25 5 | 60 30 10)

    public VertexGradient missionFailVG;   //미션 실패 메시지 그라디언트

    public GameObject[] missionLVStars;  //미션 난이도 표시하는 별 UI 옵젝들

    #endregion

    private StageManager sm;

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
        EventManager.StartListening("StageClear", () =>
        {
            for(int i=0; i<currentMissions.Count; i++)
            {
                currentMissions[i].isEnd = true;
            }
        });
        #endregion

        #region Mission Setting
        allMissionsDic.Add(MissionType.ALLKILL, new AllKillMission("모든 적을 처치하세요"));
        allMissionsDic.Add(MissionType.NOQUIKSLOT, new NoQuikSlotMission("퀵슬롯을 사용하지 않고 클리어하세요"));
        allMissionsDic.Add(MissionType.NODAMAGED, new NoDamagedMission());
        allMissionsDic.Add(MissionType.SIMULABSORPTION, new SimulAbsorptionMission("3마리를 동시에 흡수하세요"));
        allMissionsDic.Add(MissionType.CONSEKILL, new ConseKillMission());
        allMissionsDic.Add(MissionType.TIMELIMITKILL, new TimeLimitKillMission());
        //allMissionsDic.Add(MissionType.NOTRANSFORMATION, new NoTransformationMission("변신하지 않고 클리어하세요"));
        //allMissionsDic.Add(MissionType.ALLABSORPTION, new AllAbsorptionMission("모든 적을 흡수하세요"));

        foreach(MissionType type in allMissionsDic.Keys)
        {
            missionWeightDic.Add(type, 5);
        }

        //튜토리얼 3번방 전용
        allMissionsDic.Add(MissionType.SURVIVAL1, new SurvivalMission("15초 동안 살아남으세요 (15)", 15f, doorBreak =>
        {
            TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto3Clear"));
            Enemy.EnemyManager.Instance.PlayerDeadEvent();
        }));

        //튜토 5번방, 7번방 전용
        allMissionsDic.Add(MissionType.ABSORPTIONTUTORIAL, new AbsorptionTutoMission("모든 적을 흡수하세요"));
        allMissionsDic.Add(MissionType.ABSORPTIONTUTORIAL2, new AbsorptionTutoMission2("모든 적을 흡수하세요"));

        #endregion
    }

    private void Start()
    {
        sm = StageManager.Instance;

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

    private Mission GetRandomMission()  //미션을 랜덤으로 부여받음. 가중치 확률
    {
        int total = 0, weight = 0, i;
        List<MissionType> list = sm.CurrentStageData.missionTypes;
        for(i = 0; i< list.Count; i++)
            total += missionWeightDic[list[i]];
        
        int selNum = Mathf.RoundToInt(total * Random.Range(0f, 1f));

        for(i=0; i< list.Count; i++)
        {
            weight += missionWeightDic[list[i]];
            if(selNum < weight)
            {
                return allMissionsDic[list[i]];
            }
        }

        Debug.Log("문제 발생 : 확인 필요");
        Debug.Log($"total : {total}, selNum : {selNum}, weight : {weight}");
        for (i = 0; i < list.Count; i++) Debug.Log(list[i].ToString());

        return GetRandomMission();
    }

    private void CheckMissionZeroWeight(ref Mission ms)
    {
        MissionType type = ms.missionType;
        missionWeightDic[type]--;
        if (missionWeightDic[type] <= 0)
        {
            if (!zeroWeightMsTypeList.Contains(type))
            {
                zeroWeightMsTypeList.Add(type);

                int count = 0;
                //몬스터 스테이지 세 턴 하고 네 턴째에 다시 5번의 카운트 넣어줌
                System.Action action = null;
                action = () =>
                {
                    if (sm.CurrentAreaType == AreaType.MONSTER)
                    {
                        if (++count >= 4)
                        {
                            missionWeightDic[type] = 5;
                            zeroWeightMsTypeList.Remove(type);
                            EventManager.StopListening(Global.EnterNextMap, action);
                        }
                    }
                };
                EventManager.StartListening(Global.EnterNextMap, action);
            }
        }
    }

    private void MissionRandomInCounter(ref Mission ms)  //미션 랜덤 인카운터
    {
        if(sm.CurrentFloor == 1 && sm.CurrentStageNumber == 3)
        {
            if(!MissionLVInfoTextAsset.Instance.GetValue(ms.missionType)[0] && sm.CurrentStageData.missionTypes.Count > 1)
            {
                ms = allMissionsDic[sm.CurrentStageData.missionTypes.FindRandom(x => MissionLVInfoTextAsset.Instance.GetValue(x)[0])];
            }
        }

        if (ms.missionType != prevMission.first)
        {
            prevMission.first = ms.missionType;
            prevMission.second = 0;
        }
        else if (++prevMission.second >= 2)
        {
            if (sm.CurrentStageData.missionTypes.Count > 1)
            {
                MissionType curType = ms.missionType;
                ms = allMissionsDic[sm.CurrentStageData.missionTypes.FindRandom(x => x != curType)];
            }
        }
    }
    
    private DifficultyLevel GetRandomDLV()  // 미션의 랜덤 난이도 반환
    {
        int ran = Random.Range(0, 100);
        for(int i=0; i<3; i++)
        {
            if(ran < msLvRate[StageManager.Instance.CurrentFloor-1,i])
            {
                return (DifficultyLevel)i;
            }
        }
        return DifficultyLevel.NORMAL;
    }

    private void ApplyIncounter(ref DifficultyLevel lv)
    {
        if(sm.CurrentFloor == 1 && sm.CurrentStageNumber == 3)  //1스테이지의 첫 몬스터 구간이면
        {
            lv = DifficultyLevel.EASY;
        }
    }

    public void EnteredMonsterArea()  //몬스터 구역 들가면 미션 할당
    {
        if (sm.CurrentAreaType == AreaType.MONSTER && sm.CurrentStageData.missionTypes.Count > 0)
        {
            Mission ms = GetRandomMission();
            MissionRandomInCounter(ref ms);
            CheckMissionZeroWeight(ref ms);
            DifficultyLevel lv = GetRandomDLV();
            ApplyIncounter(ref lv);
            ms.SetLv(lv);
            ms.Start();
            currentMissions.Add(ms);

            mscCtrl.SetColor(missionLvColors[(int)ms.missionLevel]);
            missionPanelRt.DOKill();

            missionContent.text = ms.missionName;
            missionCvsg.alpha = 0;
            missionPanelRt.anchoredPosition = missionPanelPos + new Vector2(200, 0);
            missionCvsg.gameObject.SetActive(true);

            missionCvsg.DOFade(1, 0.4f);
            missionPanelRt.DOAnchorPos(missionPanelPos, 0.4f).SetEase(Ease.OutQuart);

            SetMissionStarAmount(ms.missionLevel);
        }
    }

    public void StartMission(MissionType msType, DifficultyLevel lv)  // 어떠한 미션을 등장시켜줌
    {
        Mission ms = allMissionsDic[msType];
        ms.SetLv(lv);
        ms.Start();
        currentMissions.Add(ms);

        mscCtrl.SetColor(missionLvColors[(int)lv]);
        missionPanelRt.DOKill();

        missionContent.text = ms.missionName;
        missionCvsg.alpha = 0;
        missionPanelRt.anchoredPosition = missionPanelPos + new Vector2(200, 0);
        missionCvsg.gameObject.SetActive(true);

        missionCvsg.DOFade(1, 0.4f);
        missionPanelRt.DOAnchorPos(missionPanelPos, 0.4f).SetEase(Ease.OutQuart);

        SetMissionStarAmount(lv);
    }

    public void DisableMission(int index = -1) //현재진행중인 미션 리스트에서 제거하고 UI제거
    {
        if(index >= 0)
           currentMissions.RemoveAt(index);

        missionPanelRt.DOKill();
        missionCvsg.DOFade(0, 0.4f);
        missionPanelRt.DOAnchorPos(missionPanelPos + new Vector2(200, 0), 0.4f).OnComplete(() => missionCvsg.gameObject.SetActive(false));
    }

    private void DeleteAllMissions() //진행중인 모든 미션 종료
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

    public void ShakeMissionPanel(float duration = 0.6f, float strength = 10f) //미션 UI 흔들림
    {
        missionPanelRt.DOShakeAnchorPos(duration, strength);
    }

    public void SetMissionStarAmount(DifficultyLevel lv)  //미션 난이도 별 UI 세팅
    {
        int cnt = (int)lv + 1;
        for(int i=0; i<missionLVStars.Length; i++)
        {
            missionLVStars[i].SetActive(i < cnt);
        }
    }

    public void SetMissionTimeFill(float rate)
    {
        missionTimeFill.fillAmount = rate;
    }
}
