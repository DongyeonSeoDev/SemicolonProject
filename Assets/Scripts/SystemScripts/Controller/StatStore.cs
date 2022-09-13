using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Water;
using System.Text;

public class StatStore : MonoSingleton<StatStore>   
{
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

    public CharType GetCharType(ushort id) => NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id).charType;

    private void SetIncounter(ref List<ushort> list, int cnt)
    {
        CharType type;
        ushort tmp;
        if (cnt > 0)
        {
            //1��ĭ�� ����̳� ����
            type = GetCharType(list[0]);
            if (!(type == CharType.SECRET || type == CharType.MONSTER))
            {
                int idx = list.FindIndex(x => (GetCharType(x) == CharType.MONSTER || GetCharType(x) == CharType.SECRET) && !prevStockIDList.Contains(x));  //�ƹ��� ���� ���������� ���� ����ó�� �ʿ�
                tmp = list[0];
                list[0] = list[idx];
                list[idx] = tmp;
            }

            //2,3��ĭ�� ����
            /*if (cnt > 1)
            {
                ushort tmp2 = 0;
                for (i = 1; i < cnt; i++)
                {
                    type = GetCharType(list[i]);
                    if (type != CharType.STORE)
                    {
                        int idx = list.FindIndex(x => GetCharType(x) == CharType.STORE && x != tmp2);  //�ƹ��� ���� ���������� ���� ����ó�� �ʿ�
                        tmp = list[i];
                        list[i] = list[idx];
                        list[idx] = tmp;
                        tmp2 = list[i];

                        Debug.Log(list[i]);
                        Debug.Log(list[idx]);
                    }
                }
            }*/
        }
    }

    public void EnteredStatArea()  //���� ���� �������� �� ȣ���
    {
        curRechargeCount = 0;
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
        }
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

    public void Purchase(ushort id) //purchasedPropIDList�� ���� id�� ���� ����, �����ϱ� ���� ����Ʈ�� �־�� ��
    {
        if (tweeningQueue.Count > 0) return;

        if (!purchasedPropIDList.Contains(id))
        {
            selectedProp.Buy();
            NGlobal.playerStatUI.PlayerStat.UseStatPoint(GetDataSO(id).purchase);
            NGlobal.playerStatUI.UpdateScrStatUI();
            purchasedPropIDList.Add(id);
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            if (!stat.isUnlock)
            {
                NGlobal.playerStatUI.StatUnlock(stat);
            }
            else
            {
                if (stat.statLv < stat.maxStatLv)
                {
                    stat.statLv++;
                    NGlobal.playerStatUI.InsertPropertyInfo(id);
                }
            }
            Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(id);
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
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=#495FD9><size=120%>");
            sb.Append(NGlobal.playerStatUI.GetStatSOData(prop.ID).statName);
            sb.Append("</size></color> Ư���� ");
            sb.Append(prop.Point);
            sb.Append("����Ʈ�� ");
            sb.Append(prop.IsSellItem ? "<color=#495FD9>�Ǹ�</color>�Ͻðڽ��ϱ�?" : "<color=#495FD9>����</color>�Ͻðڽ��ϱ�?");

            System.Action conf = null;

            if (prop.IsSellItem) conf = () => Sell(selectedProp.ID);
            else conf = () => Purchase(selectedProp.ID);
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
