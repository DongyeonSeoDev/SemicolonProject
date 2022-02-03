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
    public bool ExistItem { get; private set; }

    public int RestCount { get { return MaxCount - Count; } }

    public GameObject root;
    public Image itemImg;
    public Text itemCountTxt;
    public Button button;
    public Outline outline;
    public CanvasGroup cvsg;

    public NameInfoFollowingCursor nifc;

    private void Awake()
    {
        button.onClick.AddListener(() => 
        {
            UIManager.Instance.DetailItemSlot(this);
            outline.enabled = true;
            outline.DOColor(new Color(0,1,1,0.3f), 2.5f).SetLoops(-1,LoopType.Yoyo).SetUpdate(true);
        });
        Global.ItemSlotOutlineColor = outline.effectColor;
    }

    public void SetData(ItemInfo item, int count)
    {
        this.itemInfo = item;
        this.Count = count;
        this.ExistItem = true;

        ItemSO data = GameManager.Instance.GetItemData(item.id);
        itemImg.sprite = data.GetSprite();
        //itemTypeImg.sprite = Global.GetItemTypeSpr(data.itemType);
        //itemNameTxt.text = data.itemName;
        itemCountTxt.text = count.ToString();
        MaxCount = data.maxCount;
        GetComponent<UITransition>().transitionEnable = true;
        root.SetActive(true);
        button.interactable = true;

        nifc.explanation = data.itemName;
        nifc.transitionEnable = true;
    }

    public void SetTempData() => itemInfo = new ItemInfo();  //쓸 아이템 슬롯을 미리 찜해두고 일괄처리 할 때를 위해서 씀

    public void ResetData()
    {
        GetComponent<UITransition>().transitionEnable = false;
        nifc.transitionEnable = false;
        button.interactable = false;
        root.SetActive(false);
        this.itemInfo = null;
        this.Count = 0;
        this.ExistItem = false;
    }

    public void UpdateCount(int count)
    {
        this.Count = count;
        itemCountTxt.text = count.ToString();
    }

    
}
