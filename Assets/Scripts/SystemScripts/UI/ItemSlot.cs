using UnityEngine;
using UnityEngine.UI;
using Water;

[DisallowMultipleComponent]
public class ItemSlot : MonoBehaviour
{
    public ItemInfo itemInfo { get; private set; }

    public Image itemImg, itemTypeImg;
    public Text itemNameTxt, itemCountTxt;

    public void SetData(ItemInfo item)
    {
        this.itemInfo = item;

        ItemSO data = GameManager.Instance.GetItemData(item.id);
        itemImg.sprite = data.GetSprite();
        itemTypeImg.sprite = Global.GetItemTypeSpr(item.itemType);
        itemNameTxt.text = data.itemName;
        itemCountTxt.text = item.count.ToString();
    }
}
