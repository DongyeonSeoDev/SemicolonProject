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
        foreach(ItemInfo item in gm.savedData.userInfo.userItems.keyValueDic.Values)
        {
            if(i<maxItemSlotCount)
            {
                itemSlots[i].SetData(item);
                i++;
            }
        }
    }

    public ItemSlot FindSlot(int id)
    {
        for(int i=0; i<itemSlots.Count; ++i)
        {
            if (itemSlots[i].itemInfo.id == id) return itemSlots[i];
        }

        Debug.Log($"인벤토리에서 id가 {id}인 아이템을 찾지 못함.");
        return null;
    }
}
