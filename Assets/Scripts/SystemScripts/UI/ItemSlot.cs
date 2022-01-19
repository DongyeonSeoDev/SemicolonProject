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
    public bool existItem { get; private set; }

    public int RestCount { get { return MaxCount - Count; } }

    public GameObject root;
    public Image itemImg;
    public Text itemCountTxt;
    public Button button;
    public CanvasGroup cvsg;

    public void SetData(ItemInfo item, int count)
    {
        this.itemInfo = item;
        this.Count = count;
        this.existItem = true;

        ItemSO data = GameManager.Instance.GetItemData(item.id);
        itemImg.sprite = data.GetSprite();
        //itemTypeImg.sprite = Global.GetItemTypeSpr(data.itemType);
        //itemNameTxt.text = data.itemName;
        itemCountTxt.text = count.ToString();
        MaxCount = data.maxCount;
        GetComponent<UITransition>().transitionEnable = true;
        root.SetActive(true);
        button.interactable = true;
    }

    public void SetTempData() => itemInfo = new ItemInfo();  //�� ������ ������ �̸� ���صΰ� �ϰ�ó�� �� ���� ���ؼ� ��

    public void ResetData()
    {
        GetComponent<UITransition>().transitionEnable = false;
        button.interactable = false;
        root.SetActive(false);
        this.itemInfo = null;
        this.Count = 0;
        this.existItem = false;
    }

    public void UpdateCount(int count)
    {
        this.Count = count;
        itemCountTxt.text = count.ToString();
    }

    
}
