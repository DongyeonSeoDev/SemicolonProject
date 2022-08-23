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

    public Pair<Image,Text> statExpPair; // 1: ��������Ʈ ����ġ ��������, 2: ������ �ִ� ��������Ʈ �ؽ�Ʈ
    public Text statExpText;  // ���� ����Ʈ ����ġ �ؽ�Ʈ
    public Pair<GameObject, Transform> statInfoUIPair, choiceStatInfoUIPair;  //���� ���� UI �����հ� �θ�, ���ý��� UI �����հ� �θ�
    public GameObject invisibleChoiceStatUIPrefab;

    public TextMeshProUGUI statPointTMP;  //ȭ�鿡 �׳� ��÷� ���̴� ��������Ʈ �ؽ�Ʈ
    public GameObject statPointEff;  //ȭ�鿡 ��÷� ���̴� ��������Ʈ ǥ�� UI ����Ʈ

    private int prevStatPoint; //���������� ����â �ݾ��� ���� ��������Ʈ ��
    private int expFullCount = 0; //���������� ����â ���� ����ġ Ȯ���� �ķκ��� ��������Ʈ ����ġ�� ������ ����Ʈ�� ���� ���� �� ȸ �־�����
    private float prevConfirmExpRate = 0f; //���������� ����â ��� ����ġ Ȯ������ ���� ����ġ ��
    private float targetExpFillRate;
    private bool isFastChangingExpTxt = false;  //����â ���� ��������Ʈ ����ġ �ؽ�Ʈ�� ������ ���� ����ġ���� ������ ������ (�׳� ȿ��)

    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();  //���� ���� UI������ ����
    private Dictionary<ushort, ChoiceStatInfoElement> choiceStatInfoUIDic = new Dictionary<ushort, ChoiceStatInfoElement>(); //���� ���� UI ������ ����
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: ����Ʈ, 2: �߰�
    public Dictionary<ushort, StatElement> choiceStatDic = new Dictionary<ushort, StatElement>();  //additional������ ��ó�� Pair�� ���� �ؾ���.

    private readonly int propertyNoticeMaxCount = 4;
    private Vector2[] propertyNoticePos;
    private bool isInsertingProperty = false;
    private Queue<ushort> propNoticeQueue = new Queue<ushort>();
    private List<PropertyUI> propertyNoticeList = new List<PropertyUI>();
    private float propIntervalY;
    private float propSizeMoveX;
    public Transform propertyUIParent;  //Ư�� UI ����� �θ� ��ġ

    [SerializeField] private int invisibleChoiceStatUICount = 3; //���� ���� ��� �� �ϳ� ������ �ڼ��� ���� ������ �� �ڼ��� ����â���� �����ؼ�
    //���� ���� ��ũ�Ѻ� �ȿ� �־ ��ũ�ѿ� ���Ե� ��ó�� ���̰� �ϱ� ���ؼ� ���� ���õ� ��ư �ؿ� �Ⱥ��̴� ���� ���� ��ư�� �� ��(����) �ּ� �׷��� ���̵��� ����. �������� ���� �ϴ� �̷��� �ļ��� ��
    private List<Transform> invisibleChoiceStatUIList = new List<Transform>(); //�� �ּ����� ���ϴ� �Ⱥ��̴� ���� ���� ��� ��ư. �ȿ� �鰥 ������ ���� �ִ�

    private Transform choiceDetailPar;
    [SerializeField] private Vector2 choiceDetailStartPos;

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
                    if(idx == -1)  //���� ȭ�鿡 �ش� Ư�� ǥ�� UI�� ���� ���
                    {
                        if (!choiceStatDic[id].isUnlock)  //Ư���� �Ǹ��� ���
                        {
                            isInsertingProperty = false;
                            return;
                        }

                        PropertyUI newProp = PoolManager.GetItem<PropertyUI>("PropertyNotice");
                        propertyNoticeList.Add(newProp);
                        newProp.Set(choiceStatDic[id]);
                        newProp.cvsg.alpha = 0f;
                        if(propertyNoticeList.Count > propertyNoticeMaxCount)  //UIĭ�� �� ��
                        {
                            //�� �Ʒ��� �����ְ� ���ο� ���� �� ���� �ø�
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
                        else  //UI ���� ���� max�� �ƴ�
                        {
                            //�� �� �� ĭ�� UI �־���
                            newPos = propertyNoticePos[propertyNoticeList.Count - 1];
                            newPos.y += propIntervalY;
                            newProp.rectTr.anchoredPosition = newPos;
                            newProp.rectTr.DOAnchorPos(propertyNoticePos[propertyNoticeList.Count - 1], 0.3f).SetUpdate(true);
                            newProp.cvsg.DOFade(1, 0.3f).SetUpdate(true).OnComplete(()=>isInsertingProperty = false);
                        }
                    }
                    else  //���� ȭ�鿡 �ش� Ư�� ǥ�� UI�� ������
                    {
                        if (!choiceStatDic[id].isUnlock)  //Ư���� �Ǹ��� ���
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
                        if(idx < propertyNoticeList.Count - 1)  //�ش� Ư�� UI�� �� ���� �ƴ϶��
                        {
                            //�� ���� ������
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

                            #region Ȥ�� �� �ٸ� ����. �ϴ� �ּ�
                            //�ϴ� �ּ�. => �� ����� �� ���� �����Ƿ�

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

    int ExistCurPropNotice(ushort id)  //�αװ��� Ư�� UI�� �߿� ���� ȭ�鿡 �ش� id�� Ư���� ǥ�õǾ� �ִ���
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
        //playerStat�ҷ����� �� ����
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
        choiceStatDic.Add(NGlobal.ProficiencyID, playerStat.choiceStat.proficiency);
        choiceStatDic.Add(NGlobal.MomentomID, playerStat.choiceStat.momentom);
        choiceStatDic.Add(NGlobal.EnduranceID, playerStat.choiceStat.endurance);
        choiceStatDic.Add(NGlobal.FrenzyID, playerStat.choiceStat.frenzy);
        choiceStatDic.Add(NGlobal.ReflectionID, playerStat.choiceStat.reflection);
        choiceStatDic.Add(NGlobal.MucusRechargeID, playerStat.choiceStat.mucusRecharge);
        choiceStatDic.Add(NGlobal.FakeID, playerStat.choiceStat.fake);
        choiceStatDic.Add(NGlobal.MultiShotID, playerStat.choiceStat.multipleShots);

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

        UpdateScrStatUI();
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

    public bool IsUnlockStat(ushort id) => eternalStatDic[id].first.isUnlock;
    public bool IsOpenStat(ushort id) => eternalStatDic[id].first.isOpenStat;

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

    private void StatPointExpFillup()  //��������Ʈ ����ġ�� �������� ȿ�� + �ؽ�Ʈ�� ���� ��ġ�� ����. + �� �� ������ ��������Ʈ ����
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

    public void UpdateCurStatPoint(bool statUpMark) //���� �������ִ� ��������Ʈ ����
    {
        if (!isFastChangingExpTxt)
        {
            statExpPair.second.text = statUpMark ? string.Concat(playerStat.currentStatPoint, "<color=red>-", needStatPoint, "</color>") : playerStat.currentStatPoint.ToString();
        }
    }

    public void OnMouseEnterStatUpBtn(int needStatPoint)  //� ������ ���� �ø��� ��ư�� ���콺��ų� �� ��
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
                UIManager.Instance.RequestSystemMsg("�ּҵ������� �ִ뵥�������� ���� �� �����ϴ�.");
                return false;
            }
        }

        //if (Mathf.Pow(2, eternalStatDic[id].first.upStatCount) <= playerStat.currentStatPoint) return true;
        if (UpStatInfoTextAsset.Instance.GetValue(eternalStatDic[id].first.statLv) <= playerStat.currentStatPoint) return true;
        UIManager.Instance.RequestSystemMsg("���� ����Ʈ�� �����մϴ�.");
        return false;
    }
    public bool CanStatOpen() => statOpenCost <= playerStat.currentStatPoint;  

    public void StatUp(ushort id)  //Ư�� ������ ������ ��
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
            stat.savedStatLv = 1;
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
            expFullCount += point;

            UIManager.Instance.InsertNoticeQueue($"��������Ʈ {point} ȹ��");
            UIManager.Instance.RequestLogMsg($"��������Ʈ�� ȹ���߽��ϴ�. (+{point})");
            EffectManager.Instance.CallFollowTargetGameEffect("StatPntUpEff", Global.GetSlimePos, Vector3.zero, 1.5f);

            UpdateScrStatUI();
        }
    }

    public void StatUnlock(StatElement se, bool log = true)  //Ư�� ������ '�߰�'��
    {
        se.isUnlock = true;
        if (statInfoUIDic.ContainsKey(se.id))   //���� ���� ȹ��
        {
            statInfoUIDic[se.id].UnlockStat();
        }
        else  //���� ���� ȹ��
        {
            se.statLv = 1;
            choiceStatInfoUIDic[se.id].gameObject.SetActive(true);
            InsertPropertyInfo(se.id);
        }

        if (log)
        {
            UIManager.Instance.RequestLogMsg("[" + GetStatSOData(se.id).statName + "] ȹ��");
        }
        
    }

    public void SellStat(StatElement se, bool log = true)  //� Ư���� �Ǹ���
    {
        if (choiceStatDic.ContainsKey(se.id))
        {
            se.ResetComplete();
            choiceStatInfoUIDic[se.id].gameObject.SetActive(false);

            InsertPropertyInfo(se.id);

            if (log)
            {
                UIManager.Instance.RequestLogMsg($"[{se.StatName}] Ư���� �Ǹ��Ͽ����ϴ�");
            }
        }
    }

    public void InsertPropertyInfo(ushort id)  //Ư��(���ý���)�� ��ų� �����ϸ� ȣ��
    {
        propNoticeQueue.Enqueue(id);
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
        choiceDetailAbil.text = "<b>�ɷ� : </b>" + string.Format(data.detailAbilExplanation, 
            Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().ChoiceDataDict[selectedChoiceBtnId].upTargetStatPerChoiceStat);
        choiceDetailGrowth.text = "<b>������ : </b>" + data.growthWay;
        choiceDetailAcq.text = "<b>ȹ���� : </b>" + data.acquisitionWay;

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
    
    public void UpdateScrStatUI()  //�÷��� ȭ�鿡 ���̴� ���� UI ����
    {
        statPointTMP.transform.parent.gameObject.SetActive(playerStat.currentStatPoint > 0);
        statPointTMP.text = string.Concat('+', playerStat.currentStatPoint);
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

    public void SetAllEternalStatUIUpBtn(bool on) //�����ִ� ���� ��� ��ư ���� �� �ִ� ���������� ���ؼ� ������
    {
        foreach (StatInfoElement item in statInfoUIDic.Values)
            item.SetEnableUpBtn(on);
        
    }

    public void UpdateStat() //����â ���� ���� ���� UI�� ���� ����
    {
        UpdateAllStatUI();
        UpdateAllChoiceStatUI();

        SetAllEternalStatUIUpBtn(false);

        statExpText.text = $"{prevConfirmExpRate * playerStat.maxExp} / {playerStat.maxExp}";
        statExpPair.first.fillAmount = prevConfirmExpRate;
        statExpPair.second.text = prevStatPoint.ToString();
    }
}
