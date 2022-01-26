using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Water;

public class Inventory : MonoSingleton<Inventory>
{
    private GameManager gm;

    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    public Transform itemSlotParent;
    public GameObject itemSlotPrefab;

    [SerializeField] private int maxItemSlotCount = 20;

    private void Awake()
    {
        for (int i = 0; i < maxItemSlotCount; i++)
        {
            itemSlots.Add(Instantiate(itemSlotPrefab, itemSlotParent).GetComponent<ItemSlot>());
        }
    }

    private void Start()
    {
        gm = GameManager.Instance;

        int i = 0;
        foreach (ItemInfo item in gm.savedData.userInfo.userItems.keyValueDic.Values)
        {
            int rest = item.count, max = gm.GetItemData(item.id).maxCount;
            while(rest>0)
            {
                itemSlots[i].SetData(item, Mathf.Clamp(rest,1,max));
                rest -= max;
                i++;
            }
            //겹치는 개수 제한 없을 때
            /*if (i < maxItemSlotCount)
            {
                itemSlots[i].SetData(item);
                i++;
            }*/
        }

        DefineAction();
    }

    void DefineAction()
    {
        Global.AddMonoAction(Global.TryAcquisitionItem, x => GetItem((Item)x));
        Global.AddAction(Global.MakeFood, item => GetItem(item as ItemInfo));  //음식 조합

        SlimeEventManager.StartListening("PlayerDead", ResetInventory);
    }

    void ResetInventory()
    {
        gm.savedData.userInfo.userItems.ClearDic();
        itemSlots.ForEach(x =>
        {
            if (x.ExistItem)
            {
                x.ResetData();
            }
        });


    }

    public ItemSlot FindSlot(int id)  // 함수명이 곧 기능 설명
    {
        for(int i=0; i<itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo != null && itemSlots[i].itemInfo.id == id) return itemSlots[i];
        }

        //Debug.Log($"인벤토리에서 id가 {id}인 아이템을 찾지 못함.");
        return null;
    }

    public ItemSlot FindInsertableSlot(int id)  //해당 id의 템이 존재하는 슬롯 가져옴 (더 집어넣을 수 있는 칸이면)
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo != null && itemSlots[i].itemInfo.id == id && itemSlots[i].Count < itemSlots[i].MaxCount)
                return itemSlots[i];
        }

        //Debug.Log($"인벤토리에서 id가 {id}이면서 더 집어넣을 수 있는 칸을 찾지 못함.");
        return null;
    }

    public ItemSlot EmptySlot()  //빈 슬롯 찾음
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo == null)
                return itemSlots[i];
        }
        //Debug.Log("빈 슬롯을 못찾음");
        return null;
    }

    public bool CanCombine(int id, int count) //조합으로 얻는 음식을 넣을 칸이 있는지
    {
        ItemSlot slot = FindInsertableSlot(id);
        if (slot || EmptySlot())
        {
            if(slot)
            {
                if (count <= slot.RestCount) return true;
                count -= slot.RestCount;
            }

            if(itemSlots.FindAll(x => !x.ExistItem).Count * gm.GetItemData(id).maxCount >= count)
               return true;
        }

        return false;
    }
    public void GetItem(Item item) //아이템 주웠을 때
    {
        ItemInfo data = new ItemInfo(item.itemData.id, item.DroppedCnt);

        if(gm.ExistItem(data.id))
        {
            List<ItemSlot> list = GetInsertableSlots(item.itemData.id, item.DroppedCnt);

            if(list.Count == 0)
            {
                UIManager.Instance.RequestSystemMsg("인벤토리 칸 수가 부족합니다.");
                return;
            }
            else
            {
                gm.AddItem(data);
                int count = item.DroppedCnt;
                int tmp;

                list.ForEach(x =>
                {
                    tmp = count;
                    if (x.ExistItem)
                    {
                        count -= x.MaxCount >= x.Count + count ? count : x.RestCount;
                        x.UpdateCount(Mathf.Clamp(tmp+x.Count,1,x.MaxCount));
                    }
                    else
                    {
                        count -= item.itemData.maxCount >= count ? count : item.itemData.maxCount;
                        x.SetData(data, Mathf.Clamp(tmp,1,item.itemData.maxCount));
                    }
                });

                Global.MonoActionTrigger(Global.AcquisitionItem, item);
            }
        }
        else
        {
            ItemSlot slot = EmptySlot();
            if (slot)
            {
                gm.AddItem(data);
                slot.SetData(data, data.count); //몹 드랍템의 개수가 max보다 적으면 이렇게 가능
                Global.MonoActionTrigger(Global.AcquisitionItem, item);
            }
            else
            { 
                UIManager.Instance.RequestSystemMsg("인벤토리 칸 수가 부족합니다.");
                return;
            }
        }
        item.gameObject.SetActive(false);
        GameManager.Instance.droppedItemList.Remove(item);
    }

    public void GetItem(ItemInfo item) //아이템 획득했을 때
    {
        gm.AddItem(item);

        int count = item.count;
        int id = item.id;
        int tmp;

        ItemSlot slot = FindInsertableSlot(id);
        if(slot)
        {
            tmp = slot.RestCount;
            slot.UpdateCount(Mathf.Clamp(count + slot.Count, 1, slot.MaxCount));
            count -= tmp;
        }

        int max = gm.GetItemData(id).maxCount;
        while(count> 0)
        {
            slot = EmptySlot();
            if (slot)
            {
                slot.SetData(item, Mathf.Clamp(count, 1, max));
                count -= max;
            }
            else
            {
                break;
            }
        }
    }

    public void RemoveItem(int id, int count)  //아이템 조짐
    {
        if(gm.ExistItem(id))
        {
            gm.RemoveItem(id, count);
            UIManager.Instance.RequestLeftBottomMsg(string.Format("아이템을 잃었습니다. ({0} -{1})", gm.GetItemData(id).itemName, count));

            ItemSlot slot = FindInsertableSlot(id);
            if (!slot) slot = FindSlot(id);

            int temp;

            while(count>0)
            {
                if (slot.itemInfo == null) slot = FindSlot(id);

                temp = slot.Count;
                slot.UpdateCount(Mathf.Clamp(slot.Count - count, 0, slot.MaxCount-1));
                count -= temp;

                if(slot.Count == 0)
                    slot.ResetData();
            }
        }
    }

    public List<ItemSlot> GetInsertableSlots(int id, int count) //해당 아이디의 템을 count개 만큼 인벤토리에 집어넣을 때 사용한 아이템 슬롯들을 반환
    {
        List<ItemSlot> list = new List<ItemSlot>();

        ItemSlot slot = FindInsertableSlot(id);

        if(slot)
        {
            list.Add(slot);
            count -= slot.MaxCount >= slot.Count + count ? count : slot.RestCount;
        }
        
        while(count > 0)
        {
            ItemSlot ist = EmptySlot();
            ist.SetTempData();

            if(ist)
            {
                list.Add(ist);
                count -= Mathf.Clamp(count, 1, gm.GetItemData(id).maxCount);
            }
            else
            {
                Debug.Log("인벤토리 칸 수 부족으로 " + count + "개 획득 못함");
                break;
            }
        }

        return list;
    }
}
