using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StatStore : MonoSingleton<StatStore>   
{
    [SerializeField] private int maxStockAmount = 3;  //�������� �Ǹ����� Ư���� �� ������ �����ִ���
    [SerializeField] private int maxRechargeCount = 2; //�Ǹ����� Ư�� ��� ���� ���� �� ������ ��������
    [SerializeField] private int rechargeNeedPoint = 1; //���ѿ� �ʿ��� ��������Ʈ

    public Pair<GameObject, Transform> storePropertyPrefInfo;

    private List<StoreProperty> storeProperties = new List<StoreProperty>();
    private List<ushort> allPropIDList = new List<ushort>();
    private List<ushort> purchasedPropIDList = new List<ushort>();
    private List<ushort> prevStockIDList = new List<ushort>();  //�ٷ� ������ �ȴ� Ư������ ID ����Ʈ
    private int curRechargeCount;

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

    public void EnteredStatArea()
    {
        curRechargeCount = 0;
        purchasedPropIDList.Clear();

        List<ushort> list = allPropIDList.FindAllRandom(id =>
        {
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            return stat.statLv < stat.maxStatLv;
        }, 15);

        for (int i = 0; i < maxStockAmount; i++)
        {
            storeProperties[i].Renewal(list[i]);
            prevStockIDList.Add(list[i]);
        }
    }

    public void Renewal()  //����
    {
        curRechargeCount++;
        purchasedPropIDList.Clear();
    }

    public void Purchase(ushort id) 
    {
        purchasedPropIDList.Add(id);
        StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
        if (!stat.isUnlock)
        {
            NGlobal.playerStatUI.StatUnlock(stat);
        }
        else
        {
            if(stat.statLv < stat.maxStatLv)
            {
                stat.statLv++;
                NGlobal.playerStatUI.InsertPropertyInfo(id);
            }
        }
    }

    public void Sell(ushort id)
    {
        StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
        if (stat.isUnlock)
        {
            NGlobal.playerStatUI.SellStat(stat);
        }
    }
}
