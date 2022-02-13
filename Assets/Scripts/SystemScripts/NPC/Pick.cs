using UnityEngine;
using Water;

public class Pick : InteractionObj
{
    public float pickSuccessProbability = 50f;

    [SerializeField] protected ItemSO _itemData;
    public ItemSO itemData { get { return _itemData; } }

    //protected int droppedCount = 1;
    //public int DroppedCnt { get { return droppedCount; } }

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = _itemData.GetSprite();
        objName = _itemData.itemName;
    }

    public void FollowEffect()
    {
        PoolManager.GetItem("ItemFollowEffect").GetComponent<ItemCloneEffect>().Set(GetComponent<SpriteRenderer>().sprite, SlimeGameManager.Instance.CurrentPlayerBody.transform, transform.position, Quaternion.identity);
    }

    public override void Interaction()
    {
        if (Inventory.Instance.CanCombine(_itemData.id, 1))
        {
            Global.MonoActionTrigger(Global.PickupPlant, this);
        }
        else
            UIManager.Instance.RequestSystemMsg("인벤토리를 비워주세요.");
    }
}
