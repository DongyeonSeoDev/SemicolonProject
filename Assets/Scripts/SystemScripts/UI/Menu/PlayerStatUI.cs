using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatUI : MonoBehaviour
{
    private Stat playerStat;
    public Stat PlayerStat => playerStat;

    public Pair<Image,Text> statExpPair; // 1: 스탯포인트 경험치 게이지바, 2: 가지고 있는 스탯포인트 텍스트
    public Pair<GameObject, Transform> statInfoUIPair, choiceStatInfoUIPair;  //고정 스탯 UI 프리팹과 부모, 선택스탯 UI 프리팹과 부모
    public GameObject invisibleChoiceStatUIPrefab;

    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();  //고정 스탯 UI옵젝들 담음
    private Dictionary<ushort, ChoiceStatInfoElement> choiceStatInfoUIDic = new Dictionary<ushort, ChoiceStatInfoElement>(); //선택 스탯 UI 옵젝들 담음
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: 디폴트, 2: 추가
    public Dictionary<ushort, StatElement> choiceStatDic = new Dictionary<ushort, StatElement>();  //additional있으면 위처럼 Pair로 묶긴 해야함.

    [SerializeField] private int invisibleChoiceStatUICount = 3; //선택 스탯 요소 중 하나 누르고 자세히 보기 상태일 때 자세히 보기창까지 포함해서
    //선택 스탯 스크롤뷰 안에 있어서 스크롤에 포함된 것처럼 보이게 하기 위해서 현재 선택된 버튼 밑에 안보이는 선택 스탯 버튼을 몇 개(변수) 둬서 그렇게 보이도록 할지. 어케할지 몰라서 일단 이렇게 꼼수로 함
    private List<Transform> invisibleChoiceStatUIList = new List<Transform>(); //위 주석에서 말하는 안보이는 선택 스탯 요소 버튼. 안에 들갈 프리팹 위에 있다

    private Transform choiceDetailPar;
    private Vector2 choiceDetailStartPos;

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
        choiceDetailStartPos = choiceStatDetailPanel.GetComponent<RectTransform>().anchoredPosition;
    }

    private void Start()
    {
        InitSet();
        EventManager.StartListening("PlayerDead", () =>
        {
            foreach(ChoiceStatInfoElement ui in choiceStatInfoUIDic.Values)
            {
                ui.gameObject.SetActive(false);
            }
        });
    }

    private void InitSet()
    {
        //playerStat불러오기 및 세팅
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

        if (GameManager.Instance.savedData.tutorialInfo.isEnded)
        {
            playerStat = GameManager.Instance.savedData.userInfo.playerStat;
            Debug.Log("Player Stat is Loaded");
            SlimeGameManager.Instance.Player.PlayerStat = playerStat;
        }
        else
        {
            GameManager.Instance.savedData.userInfo.playerStat = playerStat;
        }

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
        choiceStatDic.Add(NGlobal.PatienceID, playerStat.choiceStat.patience);
        choiceStatDic.Add(NGlobal.MomentomID, playerStat.choiceStat.momentom);
        choiceStatDic.Add(NGlobal.EnduranceID, playerStat.choiceStat.endurance);

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
        float rate = playerStat.currentExp / playerStat.maxExp;
        if(tweening)
        {
            
            statExpPair.first.DOFillAmount(rate, 0.4f).SetUpdate(true);
        }
        else
        {
            statExpPair.first.fillAmount = rate;
        }
    }

    public void UpdateCurStatPoint(bool statUpMark) //현재 가지고있는 스탯포인트 업뎃
    {
        statExpPair.second.text = statUpMark ? string.Concat(playerStat.currentStatPoint, "<color=red>-", needStatPoint,"</color>") : playerStat.currentStatPoint.ToString();
    }

    public void OnMouseEnterStatUpBtn(int needStatPoint)  //어떤 스탯의 스탯 올리기 버튼에 마우스대거나 뗄 때
    {
        if (needStatPoint == -5) needStatPoint = statOpenCost;

        this.needStatPoint = needStatPoint;
        UpdateCurStatPoint(needStatPoint != -1);
    }

    public bool CanStatUp(ushort id) => Mathf.Pow(2, eternalStatDic[id].first.upStatCount) <= playerStat.currentStatPoint;
    public bool CanStatOpen() => statOpenCost <= playerStat.currentStatPoint;  

    public void StatUp(ushort id)  //특정 스탯을 레벨업 함
    {
        
        StatElement stat = eternalStatDic[id].first;
        int value = (int)Mathf.Pow(2, stat.upStatCount);
        playerStat.currentStatPoint -= value;
        playerStat.accumulateStatPoint += value;
        stat.statLv++;
        stat.statValue += stat.upStatValue;
        UpdateCurStatPoint(false);
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

            UIManager.Instance.InsertNoticeQueue($"스탯포인트 {point} 획득");
            UIManager.Instance.RequestLogMsg($"스탯포인트를 획득했습니다. (+{point})");
        }
    }

    public void StatUnlock(StatElement se)  //특정 스탯을 '발견'함
    {
        se.isUnlock = true;
        if (statInfoUIDic.ContainsKey(se.id))   //고정 스탯 획득
        {
            statInfoUIDic[se.id].UnlockStat();
        }
        else  //선택 스탯 획득
        {
            choiceStatInfoUIDic[se.id].gameObject.SetActive(true);
        }
        UIManager.Instance.RequestLogMsg("[" + GetStatSOData(se.id).statName + "] 획득");
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
        choiceDetailAbil.text = "능력 : " + NGlobal.GetChoiceStatAbilExplanation(selectedChoiceBtnId);
        choiceDetailGrowth.text = "성장방법 : " + data.growthWay;
        choiceDetailAcq.text = "획득방법 : " + data.acquisitionWay;

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

    public void UpdateStat() //스탯창 열고 스탯 정보 UI들 새로 갱신
    {
        UpdateAllStatUI();
        //UpdateStatExp(false);
        UpdateCurStatPoint(false);
        statExpPair.first.fillAmount = 0f;

        UpdateAllChoiceStatUI();
    }

 
    public void Save()  //안쓰면 나중에 지울 것
    {
        //튜토리얼이 끝났다면 플레이어의 스탯정보를 저장
        /*if (GameManager.Instance.savedData.tutorialInfo.isEnded)
        {
            GameManager.Instance.savedData.userInfo.playerStat = playerStat;
        }*/
    }
}
