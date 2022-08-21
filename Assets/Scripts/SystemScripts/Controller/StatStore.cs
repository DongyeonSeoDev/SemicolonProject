using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StatStore : MonoSingleton<StatStore>   
{
    [SerializeField] private int maxStockAmount = 3;  //�������� �Ǹ����� Ư���� �� ������ �����ִ���
    [SerializeField] private int maxRechargeCount = 2; //�Ǹ����� Ư�� ��� ���� ���� �� ������ ��������
    [SerializeField] private int rechargeNeedPoint = 1; //���ѿ� �ʿ��� ��������Ʈ
    [SerializeField] private int propCost = 2;  //����
    [SerializeField] private int sellCost = 1;  //���� 1�� �ǸŰ���

    public Pair<GameObject, Transform> storePropertyPrefInfo;
    public Transform userPropPrefParent;

    private List<StoreProperty> storeProperties = new List<StoreProperty>();
    private List<ushort> allPropIDList = new List<ushort>();
    private List<ushort> purchasedPropIDList = new List<ushort>();
    private List<ushort> prevStockIDList = new List<ushort>();  //�ٷ� ������ �ȴ� Ư������ ID ����Ʈ
    private int curRechargeCount;
    private int availableCount;   //���� ���ų� ������ �ƴ� Ư������ ��

    #region Detail

    public Image detailImg;
    public TextMeshProUGUI detailTMP;
    public Text explanationTxt;

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
    }

    public void EnteredStatArea()  //���� ���� �������� �� ȣ���
    {
        curRechargeCount = 0;
        purchasedPropIDList.Clear();

        List<ushort> list = allPropIDList.FindAllRandom(id =>
        {
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            return stat.statLv < stat.maxStatLv;
        }, 15);

        availableCount = list.Count;

        for (int i = 0; i < maxStockAmount; i++)
        {
            storeProperties[i].Renewal(list[i]);
            prevStockIDList.Add(list[i]);
        }
    }

    public void Renewal()  //�����ϱ� ���� ����Ʈ�� �־�� ��. �ִ� ���� ���� Ƚ����ŭ �����ϸ� �� �̻� ���� �Ұ�
    {
        if (curRechargeCount < maxRechargeCount)
        {
            if (NGlobal.playerStatUI.PlayerStat.currentStatPoint >= rechargeNeedPoint)
            {
                NGlobal.playerStatUI.PlayerStat.currentStatPoint -= rechargeNeedPoint;
                curRechargeCount++;
                purchasedPropIDList.Clear();

                List<ushort> list = allPropIDList.FindAllRandom(id =>
                {
                    StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
                    return stat.statLv < stat.maxStatLv && !prevStockIDList.Contains(id);
                }, 15);

                for (int i = 0; i < maxStockAmount; i++)
                {
                    storeProperties[i].Renewal(list[i]);
                    prevStockIDList.Add(list[i]);
                }
            }
            else
            {
                UIManager.Instance.RequestSystemMsg("����Ʈ ����");
            }
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("���� �� ����");
        }
    }

    public void Purchase(ushort id) //purchasedPropIDList�� ���� id�� ���� ����, �����ϱ� ���� ����Ʈ�� �־�� ��
    {
        if (!purchasedPropIDList.Contains(id))
        {
            NGlobal.playerStatUI.PlayerStat.currentStatPoint -= propCost;
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
    }

    public void ShowStockList()  //NPC�� ���� �ɰ� ���� UI ������
    {
        TimeManager.TimePause();
        UIManager.Instance.OnUIInteract(UIType.STORE);
    }

    public void ShowCharInfo(ushort id)  //�������� �Ĵ� Ư�� �߿��� � Ư���� Ŭ����
    {
        //Test Code
        if(NGlobal.playerStatUI.PlayerStat.currentStatPoint >= propCost)
        {
            Purchase(id);
            NGlobal.playerStatUI.UpdateScrStatUI();
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("����Ʈ ����");
        }
    }
    
}
