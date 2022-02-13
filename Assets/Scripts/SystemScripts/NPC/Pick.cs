using UnityEngine;
using Water;

public class Pick : InteractionObj
{
    public float pickSuccessProbability = 50f;

    [SerializeField] protected ItemSO _itemData;
    public ItemSO itemData { get { return _itemData; } }

    public FakeSpriteOutline fsOut;
    //protected int droppedCount = 1;
    //public int DroppedCnt { get { return droppedCount; } }

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = _itemData.GetSprite();
        objName = _itemData.itemName;
    }

    private void Start()
    {
        GameManager.Instance.pickList.Add(this);
        fsOut.gameObject.SetActive(false);
    }

    public void FollowEffect()
    {
        PoolManager.GetItem("ItemFollowEffect").GetComponent<ItemCloneEffect>().Set(GetComponent<SpriteRenderer>().sprite, SlimeGameManager.Instance.CurrentPlayerBody.transform, transform.position, Quaternion.identity);
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

        fsOut.gameObject.SetActive(on);
    }

    public override void Interaction()
    {
        if (!Inventory.Instance.CanCombine(_itemData.id, 1))
        {
            UIManager.Instance.RequestSystemMsg("인벤토리를 비워주세요.");
            return;
        }
        
        if (Random.Range(0f, 100f) < pickSuccessProbability)
        {
            UIManager.Instance.RequestSystemMsg("채집에 성공하였습니다.", Util.Change255To1Color(114, 168, 255, 255));
            Global.MonoActionTrigger(Global.PickupPlant, this);
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("채집에 실패하였습니다.", Util.Change255To1Color(123, 0, 226, 255));
            gameObject.SetActive(false);
        }
    }
}
