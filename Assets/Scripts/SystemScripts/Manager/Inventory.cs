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

        foreach(ItemInfo item in gm.savedData.userInfo.userItems.keyValueDic.Values)
        {

        }
    }
}
