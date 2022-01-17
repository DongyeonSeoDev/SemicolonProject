using UnityEngine;
using UnityEngine.UI;
using Water;
using DG.Tweening;

[DisallowMultipleComponent]
public class ItemSlot : MonoBehaviour
{
    public ItemInfo itemInfo { get; private set; }

    public GameObject root;
    public Image itemImg, itemTypeImg;
    public Text itemNameTxt, itemCountTxt;
    public Button button;
    public CanvasGroup cvsg;

    public void SetData(ItemInfo item)
    {
        this.itemInfo = item;

        ItemSO data = GameManager.Instance.GetItemData(item.id);
        itemImg.sprite = data.GetSprite();
        itemTypeImg.sprite = Global.GetItemTypeSpr(data.itemType);
        itemNameTxt.text = data.itemName;
        itemCountTxt.text = item.count.ToString();
        root.SetActive(true);
    }

    public void ResetData()
    {
        root.SetActive(false);
        this.itemInfo = null;
    }

    public void UpdateCount(int count) => itemCountTxt.text = count.ToString();

    
}
