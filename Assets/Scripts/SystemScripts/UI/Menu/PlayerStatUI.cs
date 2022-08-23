using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Water;
using TMPro;

public class PlayerStatUI : MonoBehaviour
{
    private Stat playerStat;
    public Stat PlayerStat => playerStat;

    public Pair<Image,Text> statExpPair; // 1: 스탯포인트 경험치 게이지바, 2: 가지고 있는 스탯포인트 텍스트
    public Text statExpText;  // 스탯 포인트 경험치 텍스트
    public Pair<GameObject, Transform> statInfoUIPair, choiceStatInfoUIPair;  //고정 스탯 UI 프리팹과 부모, 선택스탯 UI 프리팹과 부모
    public GameObject invisibleChoiceStatUIPrefab;

    public TextMeshProUGUI statPointTMP;  //화면에 그냥 상시로 보이는 스탯포인트 텍스트
    public GameObject statPointEff;  //화면에 상시로 보이는 스탯포인트 표시 UI 이펙트

    private int prevStatPoint; //마지막으로 스탯창 닫았을 때의 스탯포인트 양
    private int expFullCount = 0; //마지막으로 스탯창 열고 경험치 확인한 후로부터 스탯포인트 경험치가 꽉차서 포인트를 얻은 것이 몇 회 있었는지
    private float prevConfirmExpRate = 0f; //마지막으로 스탯창 열어서 경험치 확인했을 때의 경험치 양
    private float targetExpFillRate;
    private bool isFastChangingExpTxt = false;  //스탯창 열고 스탯포인트 경험치 텍스트가 빠르게 현재 경험치까지 오르는 중인지 (그냥 효과)

    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();  //고정 스탯 UI옵젝들 담음
    private Dictionary<ushort, ChoiceStatInfoElement> choiceStatInfoUIDic = new Dictionary<ushort, ChoiceStatInfoElement>(); //선택 스탯 UI 옵젝들 담음
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: 디폴트, 2: 추가
    public Dictionary<ushort, StatElement> choiceStatDic = new Dictionary<ushort, StatElement>();  //additional있으면 위처럼 Pair로 묶긴 해야함.

    private readonly int propertyNoticeMaxCount = 4;
    private Vector2[] propertyNoticePos;
    private bool isInsertingProperty = false;
    private Queue<ushort> propNoticeQueue = new Queue<ushort>();
    private List<PropertyUI> propertyNoticeList = new List<PropertyUI>();
    private float propIntervalY;
    private float propSizeMoveX;
    public Transform propertyUIParent;  //특성 UI 요소의 부모 위치

    [SerializeField] private int invisibleChoiceStatUICount = 3; //선택 스탯 요소 중 하나 누르고 자세히 보기 상태일 때 자세히 보기창까지 포함해서
    //선택 스탯 스크롤뷰 안에 있어서 스크롤에 포함된 것처럼 보이게 하기 위해서 현재 선택된 버튼 밑에 안보이는 선택 스탯 버튼을 몇 개(변수) 둬서 그렇게 보이도록 할지. 어케할지 몰라서 일단 이렇게 꼼수로 함
    private List<Transform> invisibleChoiceStatUIList = new List<Transform>(); //위 주석에서 말하는 안보이는 선택 스탯 요소 버튼. 안에 들갈 프리팹 위에 있다

    private Transform choiceDetailPar;
    [SerializeField] private Vector2 choiceDetailStartPos;

    public GameObject choiceStatDetailPanel; // 선택 스탯 자세히 보기창
    public Text choiceDetailAbil, choiceDetailGrowth, choiceDetailAcq;  //선택 스탯 자세히 보기창에 있는 능력, 성장방법, 획득방법 설명 텍스트 

    private ushort selectedChoiceBtnId = 9999;

    private int needStatPoint; //(고정)스탯 올리기 버튼에 마우스 댈 때 올리기 위해서 필요한 스탯 포인트 저장

    [SerializeField] private int statOpenCost = 2; //스탯을 얻고나서 스탯을 개방하기 위해서 필요한 스탯 포인트

    private Dictionary<ushort, StatSO> statDataDic;

    public StatSO GetStatSOData(ushort id)
    {
        if(statDataDic.ContainsKey(id))
        {
            return statDataDic[id];
        }
        Debug.Log("존재하지 않는 Stat ID : " + id);
        return null;
    }
    public T GetStatSOData<T>(ushort id) where T : StatSO
    {
        if (statDataDic.ContainsKey(id))
        {
            return statDataDic[id] as T;
        }
        Debug.Log("존재하지 않는 Stat ID : " + id);
        return null;
    }

    private void Awake()
    {
        statDataDic = new Dictionary<ushort, StatSO>(); 

        foreach(StatSO so in Resources.LoadAll<StatSO>("System/StatData/"))
        {
            statDataDic.Add(so.statId, so);
        }

        choiceDetailPar = choiceStatDetailPanel.transform.parent;
        //choiceDetailStartPos = choiceStatDetailPanel.GetComponent<RectTransform>().anchoredPosition;

        propertyNoticePos = new Vector2[propertyNoticeMaxCount];
        GameObject[] arr = new GameObject[propertyNoticeMaxCount];
        for(int i=0; i<propertyNoticeMaxCount; i++)
        {
            propertyNoticePos[i] = propertyUIParent.GetChild(propertyNoticeMaxCount - 1 - i).GetComponent<RectTransform>().anchoredPosition;
            arr[i] = propertyUIParent.GetChild(i).gameObject;
        }
        propSizeMoveX = propertyUIParent.GetChild(0).GetComponent<RectTransform>().rect.width * 0.3f;
        propIntervalY = Mathf.Abs(propertyNoticePos[1].y - propertyNoticePos[0].y);
        for (int i=0; i<arr.Length; i++)
        {
            Destroy(arr[i]);
        }

        EventManager.StartListening("StartCutScene", () => statPointEff.SetActive(false));
        EventManager.StartListening("EndCutScene", () => statPointEff.SetActive(true));
    }

    private void Start()
    {
        InitSet();

        FuncUpdater.Add(() =>
        {
            if (!isInsertingProperty)
            {
                if(propNoticeQueue.Count > 0)
                {
                    isInsertingProperty = true;
                    ushort id = propNoticeQueue.Dequeue();
                    int idx = ExistCurPropNotice(id);
                    Vector2 newPos;
                    if(idx == -1)  //현재 화면에 해당 특성 표시 UI가 없을 경우
                    {
                        if (!choiceStatDic[id].isUnlock)  //특성을 판매한 경우
                        {
                            isInsertingProperty = false;
                            return;
                        }

                        PropertyUI newProp = PoolManager.GetItem<PropertyUI>("PropertyNotice");
                        propertyNoticeList.Add(newProp);
                        newProp.Set(choiceStatDic[id]);
                        newProp.cvsg.alpha = 0f;
                        if(propertyNoticeList.Count > propertyNoticeMaxCount)  //UI칸이 꽉 참
                        {
                            //맨 아래를 없애주고 새로운 것을 맨 위로 올림
                            PropertyUI old = propertyNoticeList[0];
                            propertyNoticeList.RemoveAt(0);
                            newPos = propertyNoticePos[propertyNoticeMaxCount-1];
                            newPos.y += propIntervalY;
                            newProp.rectTr.anchoredPosition = newPos;

                            for(int i=0; i<propertyNoticeMaxCount; i++)
                            {
                                int si = i;
                                propertyNoticeList[si].rectTr.DOAnchorPos(propertyNoticePos[si], 0.3f).SetUpdate(true);
                            }

                            newPos = propertyNoticePos[0];
                            newPos.y -= propIntervalY;
                            old.cvsg.DOFade(0f, 0.3f).SetUpdate(true);
                            old.rectTr.DOAnchorPos(newPos, 0.3f).SetUpdate(true).OnComplete(()=>old.gameObject.SetActive(false));
                            newProp.cvsg.DOFade(1, 0.3f).SetUpdate(true).OnComplete(() => isInsertingProperty = false);
                        }
                        else  //UI 수가 아직 max가 아님
                        {
                            //맨 위 빈 칸에 UI 넣어줌
                            newPos = propertyNoticePos[propertyNoticeList.Count - 1];
                            newPos.y += propIntervalY;
                            newProp.rectTr.anchoredPosition = newPos;
                            newProp.rectTr.DOAnchorPos(propertyNoticePos[propertyNoticeList.Count - 1], 0.3f).SetUpdate(true);
                            newProp.cvsg.DOFade(1, 0.3f).SetUpdate(true).OnComplete(()=>isInsertingProperty = false);
                        }
                    }
                    else  //현재 화면에 해당 특성 표시 UI가 있으면
                    {
                        if (!choiceStatDic[id].isUnlock)  //특성을 판매한 경우
                        {
                            Sequence seq = DOTween.Sequence();
                            seq.SetUpdate(true);
                            PropertyUI old = propertyNoticeList[idx];
                            newPos = propertyNoticePos[idx];
                            newPos.x += propSizeMoveX;
                            seq.Append(old.rectTr.DOAnchorPos(newPos, 0.3f))
                            .Join(old.cvsg.DOFade(0f, 0.3f).OnComplete(() => old.gameObject.SetActive(false)));
                            seq.AppendInterval(0.15f);

                            propertyNoticeList.RemoveAt(idx);
                            bool b = false;

                            for (int i = idx; i < propertyNoticeList.Count; i++)
                            {
                                int si = i;
                                if (b)
                                    seq.Join(propertyNoticeList[si].rectTr.DOAnchorPos(propertyNoticePos[si], 0.3f));
                                else
                                {
                                    seq.Append(propertyNoticeList[si].rectTr.DOAnchorPos(propertyNoticePos[si], 0.3f));
                                    b = true;
                                }
                            }

                            seq.AppendInterval(0.05f);
                            seq.AppendCallback(() => isInsertingProperty = false);

                            return;
                        }

                        propertyNoticeList[idx].NewUpdate();
                        if(idx < propertyNoticeList.Count - 1)  //해당 특성 UI가 맨 위가 아니라면
                        {
                            //맨 위로 보내줌
                            Sequence seq = DOTween.Sequence();
                            seq.SetUpdate(true);
                            PropertyUI p = propertyNoticeList[idx];
                            newPos = propertyNoticePos[idx];
                            newPos.x += propSizeMoveX;
                            seq.Append(p.rectTr.DOAnchorPos(newPos, 0.3f))
                            .Join(p.cvsg.DOFade(0f, 0.3f));
                            seq.AppendInterval(0.15f);

                            propertyNoticeList.Add(propertyNoticeList[idx]);
                            propertyNoticeList.RemoveAt(idx);

                            bool b = false;

                            for(int i=idx; i<propertyNoticeList.Count-1; i++)
                            {
                                int si = i;
                                if (b)
                                    seq.Join(propertyNoticeList[si].rectTr.DOAnchorPos(propertyNoticePos[si], 0.3f));
                                else
                                {
                                    seq.Append(propertyNoticeList[si].rectTr.DOAnchorPos(propertyNoticePos[si], 0.3f));
                                    b = true;
                                }
                            }

                            newPos = propertyNoticePos[propertyNoticeList.Count - 1] + new Vector2(propSizeMoveX, 0);

                            seq.AppendInterval(0.15f).AppendCallback(()=>propertyNoticeList[propertyNoticeList.Count - 1].rectTr.anchoredPosition = newPos);
                            seq.Append(propertyNoticeList[propertyNoticeList.Count - 1].cvsg.DOFade(1f, 0.3f))
                            .Join(propertyNoticeList[propertyNoticeList.Count - 1].rectTr.DOAnchorPos(propertyNoticePos[propertyNoticeList.Count - 1], 0.3f));
                            seq.AppendCallback(() => isInsertingProperty = false).Play();

                            #region 혹시 모를 다른 연출. 일단 주석
                            //일단 주석. => 이 연출로 할 수도 있으므로

                            /*propertyNoticeList[idx].transform.SetAsLastSibling();
                            propertyNoticeList[idx].rectTr.DOAnchorPos(propertyNoticePos[propertyNoticeList.Count - 1], 0.32f).OnComplete(()=> isInsertingProperty = false);

                            propertyNoticeList.Add(propertyNoticeList[idx]);
                            propertyNoticeList.RemoveAt(idx);

                            for (int i = idx; i < propertyNoticeList.Count - 1; i++)
                            {
                                int si = i;
                                propertyNoticeList[si].rectTr.DOAnchorPos(propertyNoticePos[si], 0.3f);
                            }*/
                            #endregion
                        }
                        else
                        {
                            isInsertingProperty = false;
                        }
                    }
                }
            }
        });
    }

    int ExistCurPropNotice(ushort id)  //로그같은 특성 UI들 중에 현재 화면에 해당 id의 특성이 표시되어 있는지
    {
        for (int i = 0; i < propertyNoticeList.Count; i++)
        {
            if (propertyNoticeList[i].ID == id)
            {
                return i;
            }
        }
        return -1;
    }

    public void PlayerRespawnEvent()
    {
        foreach (ChoiceStatInfoElement ui in choiceStatInfoUIDic.Values)
        {
            ui.gameObject.SetActive(false);
        }
        prevConfirmExpRate = 0f;
        expFullCount = 0;
        prevStatPoint = playerStat.currentStatPoint;

        PoolManager.PoolObjSetActiveFalse("PropertyNotice");
        propertyNoticeList.Clear();

        UpdateScrStatUI();
    }

    private void InitSet()
    {
        //playerStat불러오기 및 세팅
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

        List<ushort> statIDList = new List<ushort>();
        List<ushort> propIDList = new List<ushort>();

        for (int i = 0; i < playerStat.eternalStat.AllStats.Count; i++) statIDList.Add(playerStat.eternalStat.AllStats[i].id);
        for (int i = 0; i < playerStat.choiceStat.AllStats.Count; i++) propIDList.Add(playerStat.choiceStat.AllStats[i].id);

        if (GameManager.Instance.savedData.tutorialInfo.isEnded)
        {
            playerStat = GameManager.Instance.savedData.userInfo.playerStat;
            Debug.Log("Player Stat is Loaded");
            SlimeGameManager.Instance.Player.PlayerStat = playerStat;
            playerStat.currentHp = playerStat.MaxHp;
        }
        else
        {
            GameManager.Instance.savedData.userInfo.playerStat = playerStat;
        }

        for (int i = 0; i < playerStat.eternalStat.AllStats.Count; i++)
        {
            if(playerStat.eternalStat.AllStats[i].id == 0) playerStat.eternalStat.AllStats[i].id = statIDList[i];
        }
        for (int i = 0; i < playerStat.choiceStat.AllStats.Count; i++)
        {
            if(playerStat.choiceStat.AllStats[i].id == 0) playerStat.choiceStat.AllStats[i].id = propIDList[i]; 
        }

        prevStatPoint = playerStat.currentStatPoint;

        //고정 스탯 저장
        eternalStatDic.Add(NGlobal.MaxHpID, new Pair<StatElement, StatElement>(playerStat.eternalStat.maxHp, playerStat.additionalEternalStat.maxHp));
        eternalStatDic.Add(NGlobal.MinDamageID, new Pair<StatElement, StatElement>(playerStat.eternalStat.minDamage, playerStat.additionalEternalStat.minDamage));
        eternalStatDic.Add(NGlobal.MaxDamageID, new Pair<StatElement, StatElement>(playerStat.eternalStat.maxDamage, playerStat.additionalEternalStat.maxDamage));
        eternalStatDic.Add(NGlobal.DefenseID, new Pair<StatElement, StatElement>(playerStat.eternalStat.defense, playerStat.additionalEternalStat.defense));
        eternalStatDic.Add(NGlobal.IntellectID, new Pair<StatElement, StatElement>(playerStat.eternalStat.intellect, playerStat.additionalEternalStat.intellect));
        eternalStatDic.Add(NGlobal.SpeedID, new Pair<StatElement, StatElement>(playerStat.eternalStat.speed, playerStat.additionalEternalStat.speed));
        eternalStatDic.Add(NGlobal.AttackSpeedID, new Pair<StatElement, StatElement>(playerStat.eternalStat.attackSpeed, playerStat.additionalEternalStat.attackSpeed));
        eternalStatDic.Add(NGlobal.CriticalRate, new Pair<StatElement, StatElement>(playerStat.eternalStat.criticalRate, playerStat.additionalEternalStat.criticalRate));
        eternalStatDic.Add(NGlobal.CriticalDamage, new Pair<StatElement, StatElement>(playerStat.eternalStat.criticalDamage, playerStat.additionalEternalStat.criticalDamage));

        foreach(ushort key in eternalStatDic.Keys)  //고정 스탯 UI 생성
        {
            StatInfoElement el = Instantiate(statInfoUIPair.first, statInfoUIPair.second).GetComponent<StatInfoElement>();
            el.InitSet(eternalStatDic[key].first);
            statInfoUIDic.Add(key, el);
        }

        //선택 스탯 저장
        choiceStatDic.Add(NGlobal.ProficiencyID, playerStat.choiceStat.proficiency);
        choiceStatDic.Add(NGlobal.MomentomID, playerStat.choiceStat.momentom);
        choiceStatDic.Add(NGlobal.EnduranceID, playerStat.choiceStat.endurance);
        choiceStatDic.Add(NGlobal.FrenzyID, playerStat.choiceStat.frenzy);
        choiceStatDic.Add(NGlobal.ReflectionID, playerStat.choiceStat.reflection);
        choiceStatDic.Add(NGlobal.MucusRechargeID, playerStat.choiceStat.mucusRecharge);
        choiceStatDic.Add(NGlobal.FakeID, playerStat.choiceStat.fake);
        choiceStatDic.Add(NGlobal.MultiShotID, playerStat.choiceStat.multipleShots);

        //선택 스탯 UI 생성
        foreach(ushort key in choiceStatDic.Keys)
        {
            ChoiceStatInfoElement cel = Instantiate(choiceStatInfoUIPair.first, choiceStatInfoUIPair.second).GetComponent<ChoiceStatInfoElement>();
            cel.InitSet(choiceStatDic[key]);
            choiceStatInfoUIDic.Add(key, cel);
        }

        for(int i=0; i<invisibleChoiceStatUICount; i++)
        {
            GameObject o = Instantiate(invisibleChoiceStatUIPrefab, choiceStatInfoUIPair.second);
            invisibleChoiceStatUIList.Add(o.transform);
            o.SetActive(false);
        }

        UpdateScrStatUI();
    }

    public float GetCurrentPlayerStat(ushort id)  //해당 스탯의 최종 수치를 반환  
    {
        if(eternalStatDic.ContainsKey(id))
        {
            return eternalStatDic[id].first.statValue + eternalStatDic[id].second.statValue;
        }

        Debug.LogWarning("Not Exist ID : " + id);
        return 0f;
    }

    public bool IsUnlockStat(ushort id) => eternalStatDic[id].first.isUnlock;
    public bool IsOpenStat(ushort id) => eternalStatDic[id].first.isOpenStat;

    public void UpdateAllStatUI()  //모든 스탯의 현재 스탯과 스탯포인트 사용 횟수를 업뎃함
    {
        foreach(StatInfoElement item in statInfoUIDic.Values)
        {
            item.UpdateUI();
        }
    }

    public void UpdateStatUI(ushort id)  //선택한 스탯의 현재 스탯과 스탯포인트 사용 횟수를 업뎃함
    {
        statInfoUIDic[id].UpdateUI();
    }

    public void UpdateStatExp(bool tweening)  //스탯포인트 경험치바를 업뎃함
    {
        targetExpFillRate = playerStat.currentExp / playerStat.maxExp;
        if (tweening)
        {
            if (targetExpFillRate == prevConfirmExpRate && expFullCount == 0)
            {
                SetAllEternalStatUIUpBtn(true);
                statExpPair.second.text = playerStat.currentStatPoint.ToString();
                return;
            }

            isFastChangingExpTxt = true;
            FuncUpdater.Add("StatPointExpFillup", StatPointExpFillup);
        }
        else
        {
            statExpPair.first.fillAmount = targetExpFillRate;
            statExpText.text = string.Format("{0} / {1}", playerStat.currentExp, playerStat.maxExp);
        }
    }

    private void StatPointExpFillup()  //스탯포인트 경험치바 차오르는 효과 + 텍스트도 같은 수치로 변함. + 다 찰 때마다 스탯포인트 갱신
    {
        if (isFastChangingExpTxt)
        {
            statExpPair.first.fillAmount += Time.unscaledDeltaTime * 1.3f;
            statExpText.text = string.Format("{0} / {1}", (int)(statExpPair.first.fillAmount * playerStat.maxExp), playerStat.maxExp);

            if(expFullCount > 0)  
            {
                if(statExpPair.first.fillAmount >= 1)
                {
                    expFullCount--;
                    statExpPair.first.fillAmount = 0;
                    statExpPair.second.text = (++prevStatPoint).ToString();
                }
            }
            else
            {
                if (statExpPair.first.fillAmount >= targetExpFillRate)
                {
                    statExpPair.first.fillAmount = targetExpFillRate;
                    statExpText.text = string.Format("{0} / {1}", playerStat.currentExp, playerStat.maxExp);
                    prevConfirmExpRate = targetExpFillRate;
                    expFullCount = 0;
                    isFastChangingExpTxt = false;

                    SetAllEternalStatUIUpBtn(true);

                    FuncUpdater.Remove("StatPointExpFillup");
                }
            }
        }
    }

    public void CloseStatPanel()
    {
        CloseChoiceDetail();

        prevStatPoint = playerStat.currentStatPoint;
        prevConfirmExpRate = targetExpFillRate;
        expFullCount = 0;
        isFastChangingExpTxt = false;
        FuncUpdater.Remove("StatPointExpFillup");
    }

    public void UpdateCurStatPoint(bool statUpMark) //현재 가지고있는 스탯포인트 업뎃
    {
        if (!isFastChangingExpTxt)
        {
            statExpPair.second.text = statUpMark ? string.Concat(playerStat.currentStatPoint, "<color=red>-", needStatPoint, "</color>") : playerStat.currentStatPoint.ToString();
        }
    }

    public void OnMouseEnterStatUpBtn(int needStatPoint)  //어떤 스탯의 스탯 올리기 버튼에 마우스대거나 뗄 때
    {
        if (needStatPoint == -5) this.needStatPoint = statOpenCost;
        else this.needStatPoint = needStatPoint;
        UpdateCurStatPoint(needStatPoint != -1);
    }

    public bool CanStatUp(ushort id)
    {
        if(id==NGlobal.MinDamageID)
        {
            if(GetCurrentPlayerStat(id) + eternalStatDic[id].first.upStatValue >= GetCurrentPlayerStat(NGlobal.MaxDamageID))
            {
                UIManager.Instance.RequestSystemMsg("최소데미지가 최대데미지보다 높을 수 없습니다.");
                return false;
            }
        }

        //if (Mathf.Pow(2, eternalStatDic[id].first.upStatCount) <= playerStat.currentStatPoint) return true;
        if (UpStatInfoTextAsset.Instance.GetValue(eternalStatDic[id].first.statLv) <= playerStat.currentStatPoint) return true;
        UIManager.Instance.RequestSystemMsg("스탯 포인트가 부족합니다.");
        return false;
    }
    public bool CanStatOpen() => statOpenCost <= playerStat.currentStatPoint;  

    public void StatUp(ushort id)  //특정 스탯을 레벨업 함
    {
        StatElement eterStat = eternalStatDic[id].first;
        StatElement addiStat = eternalStatDic[id].second;
        int value = UpStatInfoTextAsset.Instance.GetValue(eternalStatDic[id].first.statLv);

        playerStat.currentStatPoint -= value;
        playerStat.accumulateStatPoint += value;

        eterStat.statLv++;
        addiStat.statValue += eterStat.upStatValue;

        UpdateCurStatPoint(false);
        UIManager.Instance.UpdatePlayerHPUI();
        //eternalStatDic[id].second.statValue += stat.upStatValue;
    }
    public void StatOpen(ushort id, bool free = false)  //특정 스탯을 '개방'함 (스탯 레벨 1로 함)  -> 고정스탯 전용
    {
        if (eternalStatDic.ContainsKey(id))
        {
            if (!free)
            {
                if (playerStat.currentStatPoint < statOpenCost) return;
                playerStat.currentStatPoint -= statOpenCost;
            }
            StatElement stat = eternalStatDic[id].first;
            stat.statLv = 1;
            stat.savedStatLv = 1;
            statInfoUIDic[id].OpenStat();

            UpdateCurStatPoint(false);
        }
    }

    public void AddPlayerStatPointExp(float value)  //플레이어 스탯포인트 경험치를 획득함
    {
        playerStat.currentExp += value;
        UIManager.Instance.RequestLogMsg($"경험치를 획득했습니다. (+{value})");
        if(playerStat.currentExp >= playerStat.maxExp)
        {
            int point = Mathf.FloorToInt(playerStat.currentExp / playerStat.maxExp);
            playerStat.currentExp -= point * playerStat.maxExp;
            playerStat.currentStatPoint += point;
            expFullCount += point;

            UIManager.Instance.InsertNoticeQueue($"스탯포인트 {point} 획득");
            UIManager.Instance.RequestLogMsg($"스탯포인트를 획득했습니다. (+{point})");
            EffectManager.Instance.CallFollowTargetGameEffect("StatPntUpEff", Global.GetSlimePos, Vector3.zero, 1.5f);

            UpdateScrStatUI();
        }
    }

    public void StatUnlock(StatElement se, bool log = true)  //특정 스탯을 '발견'함
    {
        se.isUnlock = true;
        if (statInfoUIDic.ContainsKey(se.id))   //고정 스탯 획득
        {
            statInfoUIDic[se.id].UnlockStat();
        }
        else  //선택 스탯 획득
        {
            se.statLv = 1;
            choiceStatInfoUIDic[se.id].gameObject.SetActive(true);
            InsertPropertyInfo(se.id);
        }

        if (log)
        {
            UIManager.Instance.RequestLogMsg("[" + GetStatSOData(se.id).statName + "] 획득");
        }
        
    }

    public void SellStat(StatElement se, bool log = true)  //어떤 특성을 판매함
    {
        if (choiceStatDic.ContainsKey(se.id))
        {
            se.ResetComplete();
            choiceStatInfoUIDic[se.id].gameObject.SetActive(false);

            InsertPropertyInfo(se.id);

            if (log)
            {
                UIManager.Instance.RequestLogMsg($"[{se.StatName}] 특성을 판매하였습니다");
            }
        }
    }

    public void InsertPropertyInfo(ushort id)  //특성(선택스탯)을 얻거나 렙업하면 호출
    {
        propNoticeQueue.Enqueue(id);
    }

    public void DetailViewChoiceStatInfo(ushort id)  //선택 스탯 UI 클릭 후 자세히 보기
    {
        if (selectedChoiceBtnId == id)  // 같은 거 클릭하면 자세히 보기창 닫아줌 (트위닝 적용)
        {
            selectedChoiceBtnId = 9999;
            choiceStatDetailPanel.transform.DOKill();
            choiceStatDetailPanel.transform.DOScale(SVector3.Y0, 0.3f).OnComplete(() =>
            {
                for (int i = 0; i < invisibleChoiceStatUICount; i++)
                {
                    invisibleChoiceStatUIList[i].gameObject.SetActive(false);
                }
                choiceStatDetailPanel.SetActive(false);
            }).SetUpdate(true);

            return;
        }

        selectedChoiceBtnId = id;

        choiceStatDetailPanel.transform.parent = choiceDetailPar;
        choiceStatDetailPanel.GetComponent<RectTransform>().anchoredPosition = choiceDetailStartPos;

        for (int i = 0; i < invisibleChoiceStatUICount; i++)
        {
            invisibleChoiceStatUIList[i].transform.SetAsLastSibling();
        }

        RectTransform rt = choiceStatInfoUIDic[id].GetComponent<RectTransform>();
        Vector2 v = choiceStatDetailPanel.GetComponent<RectTransform>().anchoredPosition;

        int curIdx = rt.transform.GetSiblingIndex();
        for (int i = 0; i < invisibleChoiceStatUICount; i++)
        {
            invisibleChoiceStatUIList[i].gameObject.SetActive(true);
            invisibleChoiceStatUIList[i].transform.SetSiblingIndex(curIdx + i + 1);
        }

        choiceStatDetailPanel.transform.DOKill();
        choiceStatDetailPanel.transform.localScale = SVector3.Y0;
        choiceStatDetailPanel.SetActive(true);
        choiceStatDetailPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(v.x,
            rt.anchoredPosition.y - rt.rect.height * 0.5f - 2f);
        choiceStatDetailPanel.transform.DOScale(Vector3.one, 0.3f).SetUpdate(true);

        ChoiceStatSO data = GetStatSOData<ChoiceStatSO>(id);
        choiceDetailAbil.text = "<b>능력 : </b>" + string.Format(data.detailAbilExplanation, 
            Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().ChoiceDataDict[selectedChoiceBtnId].upTargetStatPerChoiceStat);
        choiceDetailGrowth.text = "<b>성장방법 : </b>" + data.growthWay;
        choiceDetailAcq.text = "<b>획득방법 : </b>" + data.acquisitionWay;

        choiceStatDetailPanel.transform.SetParent(choiceStatInfoUIDic[id].transform);
    }

    public void CloseChoiceDetail()  //선택 스탯 클릭해서 자세히보기 창 열린거 닫아준다. (트위닝 없이 그냥)
    {
        selectedChoiceBtnId = 9999;
        choiceStatDetailPanel.transform.DOKill();
        choiceStatDetailPanel.SetActive(false);
        for (int i = 0; i < invisibleChoiceStatUICount; i++)
        {
            invisibleChoiceStatUIList[i].gameObject.SetActive(false);
        }
    }
    
    public void UpdateScrStatUI()  //플레이 화면에 보이는 스탯 UI 업뎃
    {
        statPointTMP.transform.parent.gameObject.SetActive(playerStat.currentStatPoint > 0);
        statPointTMP.text = string.Concat('+', playerStat.currentStatPoint);
    }

    public void UpdateAllChoiceStatUI()  //모든 선택 스탯 UI 갱신
    {
        foreach(ChoiceStatInfoElement csie in choiceStatInfoUIDic.Values)
        {
            csie.UpdateUI();
        }
    }
    public void UpdateChoiceStatUI(ushort id)  //특정 선택 스탯 UI 갱신
    {
        choiceStatInfoUIDic[id].UpdateUI();
    }

    public void SetAllEternalStatUIUpBtn(bool on) //켜져있는 스탯 상승 버튼 누를 수 있는 상태인지에 대해서 정해줌
    {
        foreach (StatInfoElement item in statInfoUIDic.Values)
            item.SetEnableUpBtn(on);
        
    }

    public void UpdateStat() //스탯창 열고 스탯 정보 UI들 새로 갱신
    {
        UpdateAllStatUI();
        UpdateAllChoiceStatUI();

        SetAllEternalStatUIUpBtn(false);

        statExpText.text = $"{prevConfirmExpRate * playerStat.maxExp} / {playerStat.maxExp}";
        statExpPair.first.fillAmount = prevConfirmExpRate;
        statExpPair.second.text = prevStatPoint.ToString();
    }
}
