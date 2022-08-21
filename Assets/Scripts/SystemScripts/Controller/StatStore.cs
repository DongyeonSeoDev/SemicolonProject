using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StatStore : MonoSingleton<StatStore>   
{
    [SerializeField] private int maxStockAmount = 3;  //상점에서 판매중인 특성을 몇 개까지 보여주는지
    [SerializeField] private int maxRechargeCount = 2; //판매중인 특성 목록 새로 갱신 몇 번까지 가능한지
    [SerializeField] private int rechargeNeedPoint = 1; //리롤에 필요한 스탯포인트
    [SerializeField] private int propCost = 2;  //가격
    [SerializeField] private int sellCost = 1;  //레벨 1당 판매가격

    public Pair<GameObject, Transform> storePropertyPrefInfo;
    public Transform userPropPrefParent;

    private List<StoreProperty> storeProperties = new List<StoreProperty>();
    private List<ushort> allPropIDList = new List<ushort>();
    private List<ushort> purchasedPropIDList = new List<ushort>();
    private List<ushort> prevStockIDList = new List<ushort>();  //바로 이전에 팔던 특성들의 ID 리스트
    private int curRechargeCount;
    private int availableCount;   //아직 없거나 만렙이 아닌 특성들의 수

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

    public void EnteredStatArea()  //상점 구역 입장했을 때 호출됨
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

    public void Renewal()  //리롤하기 위한 포인트가 있어야 함. 최대 리롤 가능 횟수만큼 리롤하면 더 이상 리롤 불가
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
                UIManager.Instance.RequestSystemMsg("포인트 부족");
            }
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("리롤 더 못해");
        }
    }

    public void Purchase(ushort id) //purchasedPropIDList에 없는 id만 구매 가능, 구매하기 위한 포인트가 있어야 함
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

    public void Sell(ushort id) //어떤 스탯을 가지고 있어야 팔 수 있음. 레벨에 따라서 팔아서 받는 포인트 증가
    {
        StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
        if (stat.isUnlock)
        {
            NGlobal.playerStatUI.SellStat(stat);
        }
    }

    public void ShowStockList()  //NPC와 말을 걸고 상점 UI 보여줌
    {
        TimeManager.TimePause();
        UIManager.Instance.OnUIInteract(UIType.STORE);
    }

    public void ShowCharInfo(ushort id)  //상점에서 파는 특성 중에서 어떤 특성을 클릭함
    {
        //Test Code
        if(NGlobal.playerStatUI.PlayerStat.currentStatPoint >= propCost)
        {
            Purchase(id);
            NGlobal.playerStatUI.UpdateScrStatUI();
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("포인트 부족");
        }
    }
    
}
