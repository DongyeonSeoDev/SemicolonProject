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

    [SerializeField] private int maxItemSlotCount = 30;

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
            if (i < maxItemSlotCount)
            {
                itemSlots[i].SetData(item);
                i++;
            }
        }

        Global.AddMonoAction(Global.AcquisitionItem, x=> GetItem((Item)x));
    }

    public ItemSlot FindSlot(int id)
    {
        for(int i=0; i<itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo.id == id) return itemSlots[i];
        }

        Debug.Log($"�κ��丮���� id�� {id}�� �������� ã�� ����.");
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

    public void GetItem(Item item)
    {
        ItemInfo data = new ItemInfo(item.itemData.id, item.DroppedCnt);

        if(gm.ExistItem(data.id))
        {
            gm.AddItem(data);
            FindSlot(data.id).UpdateCount(gm.GetItemCount(data.id));
        }
        else
        {
            if (gm.InventoryItemCount < maxItemSlotCount)
            {
                gm.AddItem(data);
                EmptySlot().SetData(data);
            }
            else
            {
                //�κ��丮 ĭ ����
                return;
            }
        }
        item.gameObject.SetActive(false);
    }

    public void RemoveItem(int id, bool empty)
    {
        if(empty)
        {
            FindSlot(id).ResetData();
        }
        else
        {
            FindSlot(id).UpdateCount(gm.GetItemCount(id));
        }
    }

}
