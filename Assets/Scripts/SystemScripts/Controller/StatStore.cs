using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Text;

public class StatStore : MonoSingleton<StatStore>   
{
    [SerializeField] private int maxStockAmount = 3;  //�������� �Ǹ����� Ư���� �� ������ �����ִ���
    [SerializeField] private int maxRechargeCount = 2; //�Ǹ����� Ư�� ��� ���� ���� �� ������ ��������
    [SerializeField] private int rechargeNeedPoint = 1; //���ѿ� �ʿ��� ��������Ʈ
    [SerializeField] private int propCost = 10;  //����
    public int PropCost => propCost;
    [SerializeField] private int sellCost = 3;  //���� 1�� �ǸŰ���
    public int SellCost => sellCost;

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

    #region Panel

    public RectTransform buyPanel, sellPanel;
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
    }

    public void EnteredStatArea()  //���� ���� �������� �� ȣ���
    {
        curRechargeCount = 0;
        purchasedPropIDList.Clear();
        prevStockIDList.Clear();

        List<ushort> list = allPropIDList.FindAllRandom(id =>
        {
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            return stat.statLv < stat.maxStatLv;
        }, 15);

        availableCount = list.Count;

        int i, cnt = maxStockAmount;

        if (list.Count < maxStockAmount) cnt = list.Count;

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
                tweeningQueue.Enqueue(false);
                NGlobal.playerStatUI.PlayerStat.UseStatPoint(rechargeNeedPoint);
                NGlobal.playerStatUI.UpdateScrStatUI();
                curRechargeCount++;
                purchasedPropIDList.Clear();

                List<ushort> list = allPropIDList.FindAllRandom(id =>
                {
                    StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
                    return stat.statLv < stat.maxStatLv && !prevStockIDList.Contains(id);
                }, 15);

                prevStockIDList.Clear();

                int i, cnt = maxStockAmount;

                if (list.Count < maxStockAmount) cnt = list.Count; 

                for (i = 0; i < cnt; i++)
                {
                    storeProperties[i].Renewal(list[i], true);
                    storeProperties[i].gameObject.SetActive(true);
                    prevStockIDList.Add(list[i]);
                }
                for(i=cnt; i<maxStockAmount; i++)
                {
                    storeProperties[i].gameObject.SetActive(false);
                }
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
        if (!purchasedPropIDList.Contains(id))
        {
            selectedProp.Buy();
            NGlobal.playerStatUI.PlayerStat.UseStatPoint(propCost);
            NGlobal.playerStatUI.UpdateScrStatUI();
            purchasedPropIDList.Add(id);
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            if (!stat.isUnlock)
            {
                NGlobal.playerStatUI.StatUnlock(stat);
                NGlobal.playerStatUI.InsertPropertyInfo(id);
            }
            else
            {
                if (stat.statLv < stat.maxStatLv)
                {
                    stat.statLv++;
                    NGlobal.playerStatUI.InsertPropertyInfo(id);
                }
            }
        }
    }

    public void Sell(ushort id) //� ������ ������ �־�� �� �� ����. ������ ���� �ȾƼ� �޴� ����Ʈ ����
    {
        StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
        if (stat.isUnlock)
        {
            NGlobal.playerStatUI.SellStat(stat);
        }
        selectedProp.Sell();
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
            sb.Append(NGlobal.playerStatUI.GetStatSOData(prop.ID).statName);
            sb.Append(" Ư���� ");
            sb.Append(prop.Point);
            sb.Append("����Ʈ�� ");
            sb.Append(prop.IsSellItem ? "�Ǹ��Ͻðڽ��ϱ�?" : "�����Ͻðڽ��ϱ�?");

            System.Action conf = null;

            if (prop.IsSellItem) conf = () => Sell(selectedProp.ID);
            else conf = () => Purchase(selectedProp.ID);

            UIManager.Instance.RequestWarningWindow(conf, sb.ToString());
        }
    }

    public void OnChangePanel()  //����/�Ǹ� �г� ��ȯ
    {
        if (tweeningQueue.Count > 0) return;

        tweeningQueue.Enqueue(false);
        isBuyPanel = !isBuyPanel;

        RectTransform hiddenRt = isBuyPanel ? sellPanel : buyPanel;
        RectTransform appearRt = isBuyPanel ? buyPanel : sellPanel;

        appearRt.anchoredPosition = rightPos;
        appearRt.GetComponent<CanvasGroup>().alpha = 0;
        appearRt.gameObject.SetActive(true);

        hiddenRt.DOAnchorPos(leftPos, 0.3f).SetUpdate(true);
        hiddenRt.GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetUpdate(true).OnComplete(()=>hiddenRt.gameObject.SetActive(false));
        appearRt.DOAnchorPos(panelOriginPos, 0.4f).SetUpdate(true);
        appearRt.GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetUpdate(true).OnComplete(()=>tweeningQueue.Dequeue());
    }
}
