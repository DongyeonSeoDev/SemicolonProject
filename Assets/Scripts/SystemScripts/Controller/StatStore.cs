using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StatStore : MonoSingleton<StatStore>   
{
    [SerializeField] private int maxStockAmount = 3;  //상점에서 판매중인 특성을 몇 개까지 보여주는지
    [SerializeField] private int maxRechargeCount = 2; //판매중인 특성 목록 새로 갱신 몇 번까지 가능한지
    [SerializeField] private int rechargeNeedPoint = 1; //리롤에 필요한 스탯포인트

    public Pair<GameObject, Transform> storePropertyPrefInfo;

    private List<StoreProperty> storeProperties = new List<StoreProperty>();
    private List<ushort> allPropIDList = new List<ushort>();
    private List<ushort> purchasedPropIDList = new List<ushort>();
    private List<ushort> prevStockIDList = new List<ushort>();  //바로 이전에 팔던 특성들의 ID 리스트
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

    public void Renewal()  //리롤
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
