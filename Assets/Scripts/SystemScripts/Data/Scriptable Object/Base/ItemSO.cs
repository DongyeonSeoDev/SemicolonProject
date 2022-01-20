using UnityEngine;
using Water;

public abstract class ItemSO : ScriptableObject
{
    [SerializeField] protected Sprite itemSprite;

    public ItemType itemType;

    //public bool isUseable = true;

    public int id;
    public int maxCount; //해당 아이템을 최대 몇 개까지 인벤토리에 겹치게 해서 가질 수 있는지

    public string itemName;

    [TextArea]
    public string explanation;

    public virtual Sprite GetSprite()
    {
        return itemSprite;
    }

    public abstract void Use();
}
