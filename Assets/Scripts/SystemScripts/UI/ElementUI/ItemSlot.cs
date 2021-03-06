using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public ItemInfo itemInfo { get; private set; }
    public int Count { get; set; }
    public int MaxCount { get; set; }
    public bool ExistItem { get; private set; }

    public int RestCount { get => MaxCount - Count; } 

    public ItemType ItemTypePt  
    {
        get
        {
            if (itemInfo == null)
            {
                return ItemType.NONE;
            }

            return GameManager.Instance.GetItemData(itemInfo.id).itemType;
        }
    }
    public string ItemName
    {
        get
        {
            if (itemInfo == null) return "";
            return GameManager.Instance.GetItemData(itemInfo.id).itemName;
        }
    }

    public GameObject root;
    public Image itemImg;
    public Text itemCountTxt;
    public Button button;
    //public Outline outline;
    public CanvasGroup cvsg;

    public NameInfoFollowingCursor nifc;

    private Image dragImg;

    private void Awake()
    {
        button.onClick.AddListener(() => 
        {
            UIManager.Instance.DetailItemSlot(this);
            //outline.enabled = true;
            //outline.DOColor(new Color(0,1,1,0.3f), 2.5f).SetLoops(-1,LoopType.Yoyo).SetUpdate(true);
        });
       // Global.ItemSlotOutlineColor = outline.effectColor;
    }

    private void Start()
    {
        dragImg = Inventory.Instance.dragImage;
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

    public void SetTempData() => itemInfo = new ItemInfo();  //?? ?????? ?????? ???? ???????? ???????? ?? ???? ?????? ??

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

    #region On~~
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(ExistItem)
        {
            Inventory.Instance.BeginDrg(true, this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        /*if (Inventory.Instance.IsDragging)
            dragImg.transform.position = eventData.position;*/

        if (Inventory.Instance.IsDragging)
            dragImg.transform.position = Util.MousePositionForScreenSpace;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!Inventory.Instance.IsDragging) return;

        Inventory.Instance.Exchange(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Inventory.Instance.IsDragging)
            Inventory.Instance.BeginDrg(false);
    }

    
    #endregion
}
