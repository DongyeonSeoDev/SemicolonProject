using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatUI : MonoBehaviour
{
    private Stat playerStat;
    public Stat PlayerStat => playerStat;

    public Pair<Image,Text> statExpPair; // 1: ��������Ʈ ����ġ ��������, 2: ������ �ִ� ��������Ʈ �ؽ�Ʈ
    public Pair<GameObject, Transform> statInfoUIPair, choiceStatInfoUIPair;  //���� ���� UI �����հ� �θ�, ���ý��� UI �����հ� �θ�
    public GameObject invisibleChoiceStatUIPrefab;

    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();  //���� ���� UI������ ����
    private Dictionary<ushort, ChoiceStatInfoElement> choiceStatInfoUIDic = new Dictionary<ushort, ChoiceStatInfoElement>(); //���� ���� UI ������ ����
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: ����Ʈ, 2: �߰�
    public Dictionary<ushort, StatElement> choiceStatDic = new Dictionary<ushort, StatElement>();  //additional������ ��ó�� Pair�� ���� �ؾ���.

    [SerializeField] private int invisibleChoiceStatUICount = 3; //���� ���� ��� �� �ϳ� ������ �ڼ��� ���� ������ �� �ڼ��� ����â���� �����ؼ�
    //���� ���� ��ũ�Ѻ� �ȿ� �־ ��ũ�ѿ� ���Ե� ��ó�� ���̰� �ϱ� ���ؼ� ���� ���õ� ��ư �ؿ� �Ⱥ��̴� ���� ���� ��ư�� �� ��(����) �ּ� �׷��� ���̵��� ����. �������� ���� �ϴ� �̷��� �ļ��� ��
    private List<Transform> invisibleChoiceStatUIList = new List<Transform>(); //�� �ּ����� ���ϴ� �Ⱥ��̴� ���� ���� ��� ��ư. �ȿ� �鰥 ������ ���� �ִ�

    private Transform choiceDetailPar;
    private Vector2 choiceDetailStartPos;

    public GameObject choiceStatDetailPanel; // ���� ���� �ڼ��� ����â
    public Text choiceDetailAbil, choiceDetailGrowth, choiceDetailAcq;  //���� ���� �ڼ��� ����â�� �ִ� �ɷ�, ������, ȹ���� ���� �ؽ�Ʈ 

    private ushort selectedChoiceBtnId = 9999;

    private int needStatPoint; //(����)���� �ø��� ��ư�� ���콺 �� �� �ø��� ���ؼ� �ʿ��� ���� ����Ʈ ����

    [SerializeField] private int statOpenCost = 2; //������ ����� ������ �����ϱ� ���ؼ� �ʿ��� ���� ����Ʈ

    private Dictionary<ushort, StatSO> statDataDic;

    public StatSO GetStatSOData(ushort id)
    {
        if(statDataDic.ContainsKey(id))
        {
            return statDataDic[id];
        }
        Debug.Log("�������� �ʴ� Stat ID : " + id);
        return null;
    }
    public T GetStatSOData<T>(ushort id) where T : StatSO
    {
        if (statDataDic.ContainsKey(id))
        {
            return statDataDic[id] as T;
        }
        Debug.Log("�������� �ʴ� Stat ID : " + id);
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
        //playerStat�ҷ����� �� ����
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

        //���� ���� ����
        eternalStatDic.Add(NGlobal.MaxHpID, new Pair<StatElement, StatElement>(playerStat.eternalStat.maxHp, playerStat.additionalEternalStat.maxHp));
        eternalStatDic.Add(NGlobal.MinDamageID, new Pair<StatElement, StatElement>(playerStat.eternalStat.minDamage, playerStat.additionalEternalStat.minDamage));
        eternalStatDic.Add(NGlobal.MaxDamageID, new Pair<StatElement, StatElement>(playerStat.eternalStat.maxDamage, playerStat.additionalEternalStat.maxDamage));
        eternalStatDic.Add(NGlobal.DefenseID, new Pair<StatElement, StatElement>(playerStat.eternalStat.defense, playerStat.additionalEternalStat.defense));
        eternalStatDic.Add(NGlobal.IntellectID, new Pair<StatElement, StatElement>(playerStat.eternalStat.intellect, playerStat.additionalEternalStat.intellect));
        eternalStatDic.Add(NGlobal.SpeedID, new Pair<StatElement, StatElement>(playerStat.eternalStat.speed, playerStat.additionalEternalStat.speed));
        eternalStatDic.Add(NGlobal.AttackSpeedID, new Pair<StatElement, StatElement>(playerStat.eternalStat.attackSpeed, playerStat.additionalEternalStat.attackSpeed));
        eternalStatDic.Add(NGlobal.CriticalRate, new Pair<StatElement, StatElement>(playerStat.eternalStat.criticalRate, playerStat.additionalEternalStat.criticalRate));
        eternalStatDic.Add(NGlobal.CriticalDamage, new Pair<StatElement, StatElement>(playerStat.eternalStat.criticalDamage, playerStat.additionalEternalStat.criticalDamage));

        foreach(ushort key in eternalStatDic.Keys)  //���� ���� UI ����
        {
            StatInfoElement el = Instantiate(statInfoUIPair.first, statInfoUIPair.second).GetComponent<StatInfoElement>();
            el.InitSet(eternalStatDic[key].first);
            statInfoUIDic.Add(key, el);
        }

        //���� ���� ����
        choiceStatDic.Add(NGlobal.PatienceID, playerStat.choiceStat.patience);
        choiceStatDic.Add(NGlobal.MomentomID, playerStat.choiceStat.momentom);
        choiceStatDic.Add(NGlobal.EnduranceID, playerStat.choiceStat.endurance);

        //���� ���� UI ����
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

    public float GetCurrentPlayerStat(ushort id)  //�ش� ������ ���� ��ġ�� ��ȯ  
    {
        if(eternalStatDic.ContainsKey(id))
        {
            return eternalStatDic[id].first.statValue + eternalStatDic[id].second.statValue;
        }

        Debug.LogWarning("Not Exist ID : " + id);
        return 0f;
    }

    public void UpdateAllStatUI()  //��� ������ ���� ���Ȱ� ��������Ʈ ��� Ƚ���� ������
    {
        foreach(StatInfoElement item in statInfoUIDic.Values)
        {
            item.UpdateUI();
        }
    }

    public void UpdateStatUI(ushort id)  //������ ������ ���� ���Ȱ� ��������Ʈ ��� Ƚ���� ������
    {
        statInfoUIDic[id].UpdateUI();
    }

    public void UpdateStatExp(bool tweening)  //��������Ʈ ����ġ�ٸ� ������
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

    public void UpdateCurStatPoint(bool statUpMark) //���� �������ִ� ��������Ʈ ����
    {
        statExpPair.second.text = statUpMark ? string.Concat(playerStat.currentStatPoint, "<color=red>-", needStatPoint,"</color>") : playerStat.currentStatPoint.ToString();
    }

    public void OnMouseEnterStatUpBtn(int needStatPoint)  //� ������ ���� �ø��� ��ư�� ���콺��ų� �� ��
    {
        if (needStatPoint == -5) needStatPoint = statOpenCost;

        this.needStatPoint = needStatPoint;
        UpdateCurStatPoint(needStatPoint != -1);
    }

    public bool CanStatUp(ushort id) => Mathf.Pow(2, eternalStatDic[id].first.upStatCount) <= playerStat.currentStatPoint;
    public bool CanStatOpen() => statOpenCost <= playerStat.currentStatPoint;  

    public void StatUp(ushort id)  //Ư�� ������ ������ ��
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
    public void StatOpen(ushort id, bool free = false)  //Ư�� ������ '����'�� (���� ���� 1�� ��)  -> �������� ����
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

    public void AddPlayerStatPointExp(float value)  //�÷��̾� ��������Ʈ ����ġ�� ȹ����
    {
        playerStat.currentExp += value;
        UIManager.Instance.RequestLogMsg($"����ġ�� ȹ���߽��ϴ�. (+{value})");
        if(playerStat.currentExp >= playerStat.maxExp)
        {
            int point = Mathf.FloorToInt(playerStat.currentExp / playerStat.maxExp);
            playerStat.currentExp -= point * playerStat.maxExp;
            playerStat.currentStatPoint += point;

            UIManager.Instance.InsertNoticeQueue($"��������Ʈ {point} ȹ��");
            UIManager.Instance.RequestLogMsg($"��������Ʈ�� ȹ���߽��ϴ�. (+{point})");
        }
    }

    public void StatUnlock(StatElement se)  //Ư�� ������ '�߰�'��
    {
        se.isUnlock = true;
        if (statInfoUIDic.ContainsKey(se.id))   //���� ���� ȹ��
        {
            statInfoUIDic[se.id].UnlockStat();
        }
        else  //���� ���� ȹ��
        {
            choiceStatInfoUIDic[se.id].gameObject.SetActive(true);
        }
        UIManager.Instance.RequestLogMsg("[" + GetStatSOData(se.id).statName + "] ȹ��");
    }

    public void DetailViewChoiceStatInfo(ushort id)  //���� ���� UI Ŭ�� �� �ڼ��� ����
    {
        if (selectedChoiceBtnId == id)  // ���� �� Ŭ���ϸ� �ڼ��� ����â �ݾ��� (Ʈ���� ����)
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
        choiceDetailAbil.text = "�ɷ� : " + NGlobal.GetChoiceStatAbilExplanation(selectedChoiceBtnId);
        choiceDetailGrowth.text = "������ : " + data.growthWay;
        choiceDetailAcq.text = "ȹ���� : " + data.acquisitionWay;

        choiceStatDetailPanel.transform.SetParent(choiceStatInfoUIDic[id].transform);
    }

    public void CloseChoiceDetail()  //���� ���� Ŭ���ؼ� �ڼ������� â ������ �ݾ��ش�. (Ʈ���� ���� �׳�)
    {
        selectedChoiceBtnId = 9999;
        choiceStatDetailPanel.transform.DOKill();
        choiceStatDetailPanel.SetActive(false);
        for (int i = 0; i < invisibleChoiceStatUICount; i++)
        {
            invisibleChoiceStatUIList[i].gameObject.SetActive(false);
        }
    }
    
    public void UpdateAllChoiceStatUI()  //��� ���� ���� UI ����
    {
        foreach(ChoiceStatInfoElement csie in choiceStatInfoUIDic.Values)
        {
            csie.UpdateUI();
        }
    }
    public void UpdateChoiceStatUI(ushort id)  //Ư�� ���� ���� UI ����
    {
        choiceStatInfoUIDic[id].UpdateUI();
    }

    public void UpdateStat() //����â ���� ���� ���� UI�� ���� ����
    {
        UpdateAllStatUI();
        //UpdateStatExp(false);
        UpdateCurStatPoint(false);
        statExpPair.first.fillAmount = 0f;

        UpdateAllChoiceStatUI();
    }

 
    public void Save()  //�Ⱦ��� ���߿� ���� ��
    {
        //Ʃ�丮���� �����ٸ� �÷��̾��� ���������� ����
        /*if (GameManager.Instance.savedData.tutorialInfo.isEnded)
        {
            GameManager.Instance.savedData.userInfo.playerStat = playerStat;
        }*/
    }
}
