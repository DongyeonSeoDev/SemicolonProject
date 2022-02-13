using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            //��ġ�� ���� ���� ���� ��
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
        Global.AddAction(Global.MakeFood, item => GetItem(item as ItemInfo));  //���� ����
        Global.AddMonoAction(Global.PickupPlant, item =>
        {
            Pick plant = item as Pick;
            GetItem(new ItemInfo(plant.itemData.id,1));
            plant.FollowEffect();
            plant.gameObject.SetActive(false);
        });

        EventManager.StartListening("PlayerDead", ResetInventory);
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

    public ItemSlot FindSlot(int id)  // �Լ����� �� ��� ����
    {
        for(int i=0; i<itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo != null && itemSlots[i].itemInfo.id == id) return itemSlots[i];
        }

        //Debug.Log($"�κ��丮���� id�� {id}�� �������� ã�� ����.");
        return null;
    }

    public ItemSlot FindInsertableSlot(int id)  //�ش� id�� ���� �����ϴ� ���� ������ (�� ������� �� �ִ� ĭ�̸�)
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo != null && itemSlots[i].itemInfo.id == id && itemSlots[i].Count < itemSlots[i].MaxCount)
                return itemSlots[i];
        }

        //Debug.Log($"�κ��丮���� id�� {id}�̸鼭 �� ������� �� �ִ� ĭ�� ã�� ����.");
        return null;
    }

    public ItemSlot EmptySlot()  //�� ���� ã��
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo == null)
                return itemSlots[i];
        }
        //Debug.Log("�� ������ ��ã��");
        return null;
    }

    public bool CanCombine(int id, int count) //�������� ��� ������ ���� ĭ�� �ִ���
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
    public void GetItem(Item item) //������ �ֿ��� ��
    {
        ItemInfo data = new ItemInfo(item.itemData.id, item.DroppedCnt);

        if(gm.ExistItem(data.id))
        {
            List<ItemSlot> list = GetInsertableSlots(item.itemData.id, item.DroppedCnt);

            if(list.Count == 0)
            {
                UIManager.Instance.RequestSystemMsg("�κ��丮 ĭ ���� �����մϴ�.");
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
                slot.SetData(data, data.count); //�� ������� ������ max���� ������ �̷��� ����
                Global.MonoActionTrigger(Global.AcquisitionItem, item);
            }
            else
            { 
                UIManager.Instance.RequestSystemMsg("�κ��丮 ĭ ���� �����մϴ�.");
                return;
            }
        }
        
    }

    public void GetItem(ItemInfo item) //������ ȹ������ �� (�� �� �Լ��� ȣ�� ���� �̸� �κ��丮�� ���� �� �ִ��� �˻��ؾ���. CanCombine�Լ�)
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

        Global.ActionTrigger("GetItem", id);
    }

    public void RemoveItem(int id, int count)  //������ ����
    {
        if(gm.ExistItem(id))
        {
            gm.RemoveItem(id, count);
            UIManager.Instance.RequestLeftBottomMsg(string.Format("�������� �Ҿ����ϴ�. ({0} -{1})", gm.GetItemData(id).itemName, count));

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

    public List<ItemSlot> GetInsertableSlots(int id, int count) //�ش� ���̵��� ���� count�� ��ŭ �κ��丮�� ������� �� ����� ������ ���Ե��� ��ȯ
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
                Debug.Log("�κ��丮 ĭ �� �������� " + count + "�� ȹ�� ����");
                break;
            }
        }

        return list;
    }

    #region ����
    public void SortActiveItems()
    {
        int index = 0;
        for(int i=0; i<itemSlots.Count; i++)
        {
            if(itemSlots[i].ExistItem)
            {
                itemSlots[i].transform.SetSiblingIndex(index);
            }
        }
    }
    public void SortInverse()
    {
        int index = 0;
        for (int i = itemSlots.Count-1; i >= 0; i--)
        {
            itemSlots[i].transform.SetSiblingIndex(index);
        }
    }
    public void SortType()
    {
        itemSlots.Sort((x, y) => y.ItemTypePt.CompareTo(x.ItemTypePt));
        SortActiveItems();
    }
    public void SortName()
    {
        itemSlots.Sort((x, y) => y.ItemName.CompareTo(x.ItemName));
        SortActiveItems();
    }
    public void SortTypeAndName()
    {
        itemSlots.Sort((x, y) =>
        {
            switch (y.ItemTypePt.CompareTo(x.ItemTypePt))
            {
                case -1: return -1;
                case 1: return 1;
                case 0: return y.ItemName.CompareTo(x.ItemName);
                default: return y.ItemName.CompareTo(x.ItemName);
            }
        });
        SortActiveItems();
    }
    #endregion

    #region ����
    public void FilterType(int itemType)
    { 
        itemSlots.ForEach(x => x.gameObject.SetActive(true));

        if ((ItemType)itemType == ItemType.NONE)
            return;

        itemSlots.FindAll(x => x.ItemTypePt != (ItemType)itemType).ForEach(x => x.gameObject.SetActive(false));
    }
    #endregion
}
