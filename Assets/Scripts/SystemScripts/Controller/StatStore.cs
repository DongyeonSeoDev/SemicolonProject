using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Water;
using System.Text;

public class StatStore : MonoSingleton<StatStore>   
{
    [System.Serializable]
    public class StatBox  //�� ���ݿ� ���� Ư�� �̱� ī�� ����. 
    {
        public int needPoint;

        public string boxName; //UI�� ǥ���� �̸�
        [TextArea] public string explanation;

        public Pair<CharType, float> minusInfo;  //���Ȱ��� Ư��
        public Pair<CharType, float> plusInfo; //���Ȱ��� Ư��
        public Pair<CharType, float> mobInfo; //���� Ư��
        public Pair<CharType, float> SecretInfo;  //��� Ư��
    }

    public StatBox normalBox;
    public StatBox rareBox;
    public StatBox relicBox;

    [SerializeField] private int maxStockAmount = 3;  //�������� �Ǹ����� Ư���� �� ������ �����ִ���
    [SerializeField] private int maxRechargeCount = 2; //�Ǹ����� Ư�� ��� ���� ���� �� ������ ��������
    [SerializeField] private int rechargeNeedPoint = 2; //���ѿ� �ʿ��� ��������Ʈ

    public Pair<GameObject, Transform> storePropertyPrefInfo;
    public Transform userPropPrefParent;

    private List<StoreProperty> storeProperties = new List<StoreProperty>();
    private List<ushort> allPropIDList = new List<ushort>();
    private List<ushort> purchasedPropIDList = new List<ushort>();
    private List<ushort> prevStockIDList = new List<ushort>();  //�ٷ� ������ �ȴ� Ư������ ID ����Ʈ
    private int curRechargeCount;
    private int availableCount;   //���� ���ų� ������ �ƴ� Ư������ ��

    private StoreProperty selectedProp;

    private Queue<bool> tweeningQueue = new Queue<bool>();

    public NameInfoFollowingCursor updateNifc;

    #region Panel

    public TextMeshProUGUI stockUpdateTMP;
    public Text[] userPointTexts;
    public RectTransform buyPanel, sellPanel;
    public Scrollbar sellScrollBar;
    private Vector2 panelOriginPos;
    private Vector2 rightPos, leftPos;
    private bool isBuyPanel;  //���� ���� �г��� ����� �ִ���

    #endregion

    public PlayerChoiceStatControl pcsCtrl { get; private set; }

    private void Awake()
    {
        for(ushort i = NGlobal.CStatStartID; i<= NGlobal.CStatEndID; i+= NGlobal.StatIDOffset)
        {
            allPropIDList.Add(i);
        }
        for(int i=0; i< maxStockAmount; i++)
        {
            storeProperties.Add(Instantiate(storePropertyPrefInfo.first, storePropertyPrefInfo.second).GetComponent<StoreProperty>());
        }
        panelOriginPos = buyPanel.anchoredPosition;
        rightPos = panelOriginPos + new Vector2(150, 0);
        leftPos = panelOriginPos - new Vector2(150, 0);

        updateNifc.explanation = rechargeNeedPoint.ToString() + "����Ʈ �Ҹ�";
    }

    private void Start()
    {
        pcsCtrl = Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>();
    }

    public CharType GetCharType(ushort id) => NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id).charType;

    private void ChangeIndex(ref List<ushort> list, int targetIdx, System.Func<ushort, bool> change, System.Predicate<ushort> find)
    { 
        if(change(list[targetIdx]))
        {
            int idx = list.FindIndex(find);
            if(idx != -1)  //�ش� ���ǿ� �����ϴ� �ε����� ������
            {
                ushort tmp = list[targetIdx];
                list[targetIdx] = list[idx];
                list[idx] = tmp;
            }
        }
    }

    private void SetIncounter(ref List<ushort> list, in int cnt)
    {
        if (cnt > 0)
        {
            //1�� ĭ�� ���ͳ� ��� Ư��
            ChangeIndex(ref list, 0, id =>
            {
                ChoiceStatSO data = GetDataSO(id);
                return !(GetCharType(id) != CharType.STORE && (data.needStatID == 0 || NGlobal.playerStatUI.IsUnlockStat(data.needStatID)));
            }, id =>
            {
                ChoiceStatSO data = GetDataSO(id);
                return GetCharType(id) != CharType.STORE && (data.needStatID == 0 || NGlobal.playerStatUI.IsUnlockStat(data.needStatID));
            });
            if(cnt > 1)
            {
                //2�� ĭ�� ���� + ���� Ư��
                ChangeIndex(ref list, 1,
                    id =>
                    {
                        ChoiceStatSO data = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);
                        return !(GetCharType(id) == CharType.STORE && data.plusStat && NGlobal.playerStatUI.IsUnlockStat(data.needStatID));
                    },
                    id =>
                    {
                        ChoiceStatSO data = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);
                        return GetCharType(id) == CharType.STORE && data.plusStat && NGlobal.playerStatUI.IsUnlockStat(data.needStatID);
                    });
                if (cnt > 2)
                {
                    //3�� ĭ�� ���� - ���� Ư��
                    ChangeIndex(ref list, 2,
                    id =>
                    {
                        ChoiceStatSO data = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);
                        return !(GetCharType(id) == CharType.STORE && !data.plusStat && NGlobal.playerStatUI.IsUnlockStat(data.needStatID));
                    },
                    id =>
                    {
                        ChoiceStatSO data = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);
                        return GetCharType(id) == CharType.STORE && !data.plusStat && NGlobal.playerStatUI.IsUnlockStat(data.needStatID);
                    });
                }
            }
        }
    }

    public void EnteredStatArea()  //���� ���� �������� �� ȣ���
    {
        #region �ּ�
        /*curRechargeCount = 0;
        purchasedPropIDList.Clear();
        prevStockIDList.Clear();
        stockUpdateTMP.SetText(string.Concat("����(", maxRechargeCount, ')'));

        List<ushort> list = allPropIDList.FindAllRandom(id =>
        {
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            return stat.statLv < stat.maxStatLv;
        }, 20);

        availableCount = list.Count;
        int i, cnt = maxStockAmount;

        if (list.Count < maxStockAmount) cnt = list.Count;

        //Ư�� ī�� ��ī����
        SetIncounter(ref list, cnt);

        //ī�� ����
        for (i = 0; i < cnt; i++)
        {
            storeProperties[i].Renewal(list[i], true);
            storeProperties[i].gameObject.SetActive(true);
            prevStockIDList.Add(list[i]);
        }
        for (i = cnt; i < maxStockAmount; i++)
        {
            storeProperties[i].gameObject.SetActive(false);
        }*/
        #endregion

        //New

        storeProperties[0].SetRandomCard(normalBox);
        storeProperties[1].SetRandomCard(rareBox);
        storeProperties[2].SetRandomCard(relicBox);
    }

    public void Renewal()  //�����ϱ� ���� ����Ʈ�� �־�� ��. �ִ� ���� ���� Ƚ����ŭ �����ϸ� �� �̻� ���� �Ұ�
    {
        if (tweeningQueue.Count > 0) return;

        if (curRechargeCount < maxRechargeCount)
        {
            if (NGlobal.playerStatUI.PlayerStat.currentStatPoint >= rechargeNeedPoint)
            {
                if(availableCount <= maxStockAmount)
                {
                    UIManager.Instance.RequestSystemMsg("������ �� �ִ� �ٸ� Ư���� �����ϴ�");
                    return; 
                }

                tweeningQueue.Enqueue(false);
                int i, cnt = maxStockAmount;

                for(i=0; i<storeProperties.Count; i++)
                {
                    int si = i;
                    storeProperties[si].childCvsg.DOFade(0, 0.4f).SetUpdate(true);
                }

                NGlobal.playerStatUI.PlayerStat.UseStatPoint(rechargeNeedPoint);
                UpdateUserPoint();
                NGlobal.playerStatUI.UpdateScrStatUI();
                curRechargeCount++;
                purchasedPropIDList.Clear();
                stockUpdateTMP.SetText(string.Concat("����(", maxRechargeCount-curRechargeCount, ')'));

                List<ushort> list = allPropIDList.FindAllRandom(id =>
                {
                    StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
                    return stat.statLv < NGlobal.playerStatUI.GetStatSOData(id).maxStatLv && !prevStockIDList.Contains(id);
                }, 15);

                if (list.Count < maxStockAmount) cnt = list.Count;

                SetIncounter(ref list, cnt);

                prevStockIDList.Clear();

                Util.DelayFunc(() =>
                {
                    for (i = 0; i < cnt; i++)
                    {
                        storeProperties[i].Renewal(list[i], true);
                        storeProperties[i].gameObject.SetActive(true);
                        prevStockIDList.Add(list[i]);
                    }
                    for (i = cnt; i < maxStockAmount; i++)
                    {
                        storeProperties[i].gameObject.SetActive(false);
                    }

                    for (i = 0; i < storeProperties.Count; i++)
                    {
                        int si = i;
                        storeProperties[si].childCvsg.DOFade(1, 0.35f).SetUpdate(true);
                    }

                    availableCount = allPropIDList.FindAll(id => NGlobal.playerStatUI.choiceStatDic[id].statLv < NGlobal.playerStatUI.GetStatSOData(id).maxStatLv).Count;

                    Util.DelayFunc(() => tweeningQueue.Dequeue(), 0.42f, this, true);

                }, 0.5f, this, true);
            }
            else
            {
                UIManager.Instance.RequestSystemMsg("����Ʈ�� �����մϴ�");
            }
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("���� ������ �ִ� 2������ �����մϴ�");
        }
    }

    //old
    public void Purchase(ushort id) //purchasedPropIDList�� ���� id�� ���� ����, �����ϱ� ���� ����Ʈ�� �־�� ��
    {
        if (tweeningQueue.Count > 0) return;

        if (!purchasedPropIDList.Contains(id))
        {
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            ChoiceStatSO so = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);

            if (stat.statLv >= stat.maxStatLv)  //==�񱳷� �ص� ��
            {
                UIManager.Instance.RequestSystemMsg("�ش� Ư���� �� �̻� ������ �� �����ϴ�");
                return;
            }

            //0 ���Ϸ� �������� ���� ���ϰ� ��
            if (!so.plusStat && so.charType==CharType.STORE && so.needStatID > 0)
            {
                float value = stat.isUnlock ? stat.UpStatValue : pcsCtrl.ChoiceDataDict[id].firstValue;
                if(NGlobal.playerStatUI.GetCurrentPlayerStat(so.needStatID) - value < NGlobal.playerStatUI.GetStatSOData<EternalStatSO>(so.needStatID).minStatValue)
                {
                    UIManager.Instance.RequestSystemMsg("�ش� Ư���� �����ϱ⿡�� " + NGlobal.playerStatUI.GetStatSOData(so.needStatID).statName + " ������ �ʹ� �����ϴ�");
                    return;
                }
            }

            switch(id)  //����ó��
            {
                case NGlobal.StrongID:
                    if(NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MinDamageID)
                        + pcsCtrl.ChoiceDataDict[id].upTargetStatPerChoiceStat
                        >= NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxDamageID))
                    {
                        UIManager.Instance.RequestSystemMsg("�ִ뵥������ ���Ƽ� �ش� Ư���� ������ �� �����ϴ�");
                        return;
                    }
                    break;
                case NGlobal.FeebleID:
                    if(NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxDamageID)
                        - pcsCtrl.ChoiceDataDict[id].upTargetStatPerChoiceStat
                        <= NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MinDamageID))
                    {
                        UIManager.Instance.RequestSystemMsg("�ּҵ������� ���Ƽ� �ش� Ư���� ������ �� �����ϴ�");
                        return;
                    }
                    break;
            }

            selectedProp.Buy();
            NGlobal.playerStatUI.PlayerStat.currentStatPoint -= selectedProp.Point;
            NGlobal.playerStatUI.UpdateScrStatUI();
            purchasedPropIDList.Add(id);
            
            if (!stat.isUnlock)
            {
                NGlobal.playerStatUI.StatUnlock(stat);
            }
            else
            {
                stat.statLv++;
                NGlobal.playerStatUI.StatUp(id);
            }
            Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(id);
        }
    }

    //new
    public void Purchase(StatBox box)
    {
        if (tweeningQueue.Count > 0) return;

        float sel = 100f * Random.Range(0f, 1f);
        List<Triple<CharType, bool, float>> rateList = new List<Triple<CharType, bool, float>>();
        rateList.Add(new Triple<CharType, bool, float>( box.minusInfo.first, false, box.minusInfo.second));
        rateList.Add(new Triple<CharType, bool, float>(box.plusInfo.first, true, box.plusInfo.second));  //�߰��� true�� ������ ó�� ���� �� ����/���� Ư�� Ÿ�� ������ �ʾƼ� �����ϱ� ����
        rateList.Add(new Triple<CharType, bool, float>(box.mobInfo.first, false, box.mobInfo.second));
        rateList.Add(new Triple<CharType, bool, float>(box.SecretInfo.first, false, box.SecretInfo.second));

        for(int i=1; i<rateList.Count; i++) rateList[i].third += rateList[i-1].third;

        for(int i=0; i<rateList.Count; i++)
        {
            if(sel < rateList[i].third)
            {
                CharType type = rateList[i].first;
                bool plus = rateList[i].second;

                List<ushort> list = allPropIDList.FindAllRandom(id =>
                {
                    ChoiceStatSO data = GetDataSO(id);
                    return data.charType == type && (data.needStatID == 0 || NGlobal.playerStatUI.IsUnlockStat(data.needStatID))
                            && data.maxStatLv > NGlobal.playerStatUI.choiceStatDic[data.statId].statLv;
                });

                if(type == CharType.STORE)
                {
                    list = list.FindAll(id =>
                    {
                        ChoiceStatSO data = GetDataSO(id);
                        return data.plusStat == rateList[i].second;
                    });
                }

                selectedProp.Buy();
                NGlobal.playerStatUI.PlayerStat.currentStatPoint -= selectedProp.Point;
                NGlobal.playerStatUI.UpdateScrStatUI();

                StatElement stat = NGlobal.playerStatUI.choiceStatDic[list[0]];

                if (!stat.isUnlock)
                {
                    NGlobal.playerStatUI.StatUnlock(stat);
                }
                else
                {
                    stat.statLv++;
                    NGlobal.playerStatUI.StatUp(stat.id);
                }
                Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(stat.id);

                break;
            }
        }
    }

    public void Sell(ushort id) //� ������ ������ �־�� �� �� ����. ������ ���� �ȾƼ� �޴� ����Ʈ ����
    {
        if (tweeningQueue.Count > 0) return;

        StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
        if (stat.isUnlock)
        {
            NGlobal.playerStatUI.PlayerStat.GetStatPoint(selectedProp.Point);
            NGlobal.playerStatUI.SellStat(stat);
            selectedProp.Sell();
            NGlobal.playerStatUI.UpdateScrStatUI();
            Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(id);
        }
    }

    public void ShowStockList()  //NPC�� ���� �ɰ� ���� UI ������
    {
        TimeManager.TimePause();
        buyPanel.anchoredPosition = panelOriginPos;
        buyPanel.GetComponent<CanvasGroup>().alpha = 1;
        buyPanel.gameObject.SetActive(true);
        sellPanel.gameObject.SetActive(false);
        isBuyPanel = true;
        UIManager.Instance.OnUIInteract(UIType.STORE);
        UpdateUserPoint();
    }

    public void OnClickStoreProp(StoreProperty prop)  //�����ϰų� �� Ư���� Ŭ����
    {
        selectedProp = prop;
        if (!prop.IsSellItem && prop.Point > NGlobal.playerStatUI.PlayerStat.currentStatPoint)
        {
            UIManager.Instance.RequestSystemMsg("����Ʈ�� �����մϴ�");
        }
        else
        {
            ChoiceStatSO so = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(prop.ID);

            /*if(!prop.IsSellItem && so.needStatID > 0 && !NGlobal.playerStatUI.IsUnlockStat(so.needStatID))
            {
                UIManager.Instance.RequestSystemMsg("�ش� Ư���� �����ϱ����� ������ �ر����� ���߽��ϴ�");
                return;
            }*/

            StringBuilder sb = new StringBuilder();
            if (prop.IsSellItem)
            {
                sb.Append("<color=#495FD9><size=120%>");
                sb.Append(so.statName);
                sb.Append("</size></color> Ư���� ");
                sb.Append(prop.Point);
                sb.Append("����Ʈ�� ");
                sb.Append(prop.IsSellItem ? "<color=#495FD9>�Ǹ�</color>�Ͻðڽ��ϱ�?" : "<color=#495FD9>����</color>�Ͻðڽ��ϱ�?");
            }
            else
            {
                sb.Append("<color=#495FD9><size=120%>");
                sb.Append(prop.box.boxName);
                sb.Append("</size></color>�� ");
                sb.Append(prop.Point);
                sb.Append("����Ʈ�� �����Ͻðڽ��ϱ�?");
            }

            System.Action conf = null;

            if (prop.IsSellItem) conf = () => Sell(selectedProp.ID);
            else conf = () => Purchase(selectedProp.box);
            conf += UpdateUserPoint;

            UIManager.Instance.RequestWarningWindow(conf, sb.ToString());
        }
    }

    public void OnChangePanel()  //����/�Ǹ� �г� ��ȯ
    {
        if (tweeningQueue.Count > 0) return;

        tweeningQueue.Enqueue(false);
        isBuyPanel = !isBuyPanel;

        if(!isBuyPanel)
        {
            PoolManager.PoolObjSetActiveFalse("StoreProp");
            for(int i=0; i<allPropIDList.Count; i++)
            {
                if(NGlobal.playerStatUI.choiceStatDic[allPropIDList[i]].isUnlock)
                {
                    StoreProperty sp = PoolManager.GetItem<StoreProperty>("StoreProp");
                    sp.Renewal(allPropIDList[i], false);
                }
            }

            Util.DelayFunc(() => sellScrollBar.value = 0, 0.1f, this, true);
        }

        RectTransform hiddenRt = isBuyPanel ? sellPanel : buyPanel;
        RectTransform appearRt = isBuyPanel ? buyPanel : sellPanel;

        appearRt.anchoredPosition = rightPos;
        appearRt.GetComponent<CanvasGroup>().alpha = 0;
        appearRt.gameObject.SetActive(true);

        hiddenRt.gameObject.SetActive(false);
        appearRt.DOAnchorPos(panelOriginPos, 0.4f).SetUpdate(true);
        appearRt.GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetUpdate(true).OnComplete(()=>tweeningQueue.Dequeue());
    }

    public void UpdateUserPoint()
    {
        string pointTxt = "<color=yellow>" + NGlobal.playerStatUI.PlayerStat.currentStatPoint.ToString() + "</color> POINT";
        for(int i=0; i<userPointTexts.Length; i++)
            userPointTexts[i].text = pointTxt;
    }

    public ChoiceStatSO GetDataSO(ushort id) => NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);   
}
