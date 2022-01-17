using UnityEngine;
using Water;

public abstract class ItemSO : ScriptableObject
{
    [SerializeField] protected Sprite itemSprite;

    public ItemType itemType;

    public int id;
    public string itemName;

    [TextArea]
    public string explanation;

    public virtual Sprite GetSprite()
    {
        return itemSprite;
    }

    public abstract void Use();
}
