using UnityEngine;
using UnityEngine.UI;
using Water;
using DG.Tweening;

[DisallowMultipleComponent]
public class ItemSlot : MonoBehaviour
{
    public ItemInfo itemInfo { get; private set; }
    public int Count { get; set; }
    public int MaxCount { get; set; }

    public GameObject root;
    public Image itemImg, itemTypeImg;
    public Text itemNameTxt, itemCountTxt;
    public Button button;
    public CanvasGroup cvsg;

    public void SetData(ItemInfo item, int count)
    {
        this.itemInfo = item;
        this.Count = count;

        ItemSO data = GameManager.Instance.GetItemData(item.id);
        itemImg.sprite = data.GetSprite();
        itemTypeImg.sprite = Global.GetItemTypeSpr(data.itemType);
        itemNameTxt.text = data.itemName;
        itemCountTxt.text = count.ToString();
        MaxCount = data.maxCount;
        root.SetActive(true);
        button.interactable = true;
    }

    public void ResetData()
    {
        button.interactable = false;
        root.SetActive(false);
        this.itemInfo = null;
        this.Count = 0;
    }

    public void UpdateCount(int count)
    {
        this.Count = count;
        itemCountTxt.text = count.ToString();
    }

    
}
