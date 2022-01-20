using System.Collections;
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
        if (itemSlots.Count == 0)
        {
            for (int i = 0; i < maxItemSlotCount; i++)
            {
                itemSlots.Add(Instantiate(itemSlotPrefab, itemSlotParent).GetComponent<ItemSlot>());
            }
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

        Global.AddMonoAction(Global.AcquisitionItem, x=> GetItem((Item)x));
        Global.AddAction(Global.MakeFood, item =>
        {
            ItemInfo info = (ItemInfo)item;
            ItemSlot slot = FindInsertableSlot(info.id);
            if (slot)
            {
                slot.UpdateCount(slot.Count + info.count);
            }
            else
            {
                EmptySlot().SetData(info, info.count);
            }
        });
    }

    public ItemSlot FindSlot(int id)
    {
        for(int i=0; i<itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo != null && itemSlots[i].itemInfo.id == id) return itemSlots[i];
        }

        Debug.Log($"�κ��丮���� id�� {id}�� �������� ã�� ����.");
        return null;
    }

    public ItemSlot FindInsertableSlot(int id)
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo != null && itemSlots[i].itemInfo.id == id && itemSlots[i].Count < itemSlots[i].MaxCount)
                return itemSlots[i];
        }

        Debug.Log($"�κ��丮���� id�� {id}�̸鼭 �� ������� �� �ִ� ĭ�� ã�� ����.");
        return null;
    }

    public ItemSlot EmptySlot()
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo == null)
                return itemSlots[i];
        }
        Debug.Log("�� ������ ��ã��");
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
                Debug.Log("�κ��丮 ĭ ����");
                return;
            }
            else
            {
                gm.AddItem(data);
                int count = item.DroppedCnt;

                list.ForEach(x =>
                {
                    if (x.ExistItem)
                    {
                        count -= x.MaxCount >= x.Count + count ? count : (x.MaxCount - x.Count);
                        x.UpdateCount(Mathf.Clamp(count+x.Count,1,x.MaxCount));
                    }
                    else
                    {
                        count -= item.itemData.maxCount >= count ? count : item.itemData.maxCount;
                        x.SetData(data, Mathf.Clamp(count,1,item.itemData.maxCount));
                    }
                });
            }
        }
        else
        {
            ItemSlot slot = EmptySlot();
            if (slot)
            {
                gm.AddItem(data);
                slot.SetData(data, data.count); //�� ������� ������ max���� ������ �̷��� ����
            }
            else
            {
                Debug.Log("�κ��丮 ĭ ����");
                return;
            }
        }
        item.gameObject.SetActive(false);
    }



    public void RemoveItem(int id, int count)
    {
        if(gm.ExistItem(id))
        {
            ItemSlot slot = FindInsertableSlot(id);
            if (!slot) slot = FindSlot(id);

            int temp;

            while(count>0)
            {
                if (slot.itemInfo == null) slot = FindSlot(id);

                temp = slot.Count;
                slot.UpdateCount(Mathf.Clamp(slot.Count - count, 0, slot.MaxCount-1));
                count -= Mathf.Clamp(temp, 1, count);

                if(slot.Count == 0)
                    slot.ResetData();
            }
        }
    }

    public List<ItemSlot> GetInsertableSlots(int id, int count)
    {
        List<ItemSlot> list = new List<ItemSlot>();

        ItemSlot slot = FindInsertableSlot(id);

        if(slot)
        {
            list.Add(slot);
            count -= slot.MaxCount >= slot.Count + count ? count : (slot.MaxCount - slot.Count);
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
}
