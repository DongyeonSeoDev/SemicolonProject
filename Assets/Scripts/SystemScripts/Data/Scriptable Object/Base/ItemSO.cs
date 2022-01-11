using UnityEngine;
using Water;

public abstract class ItemSO : ScriptableObject
{
    [SerializeField] protected Sprite itemSprite;

    public int id;
    public string itemName;

    public virtual Sprite GetSprite()
    {
        return itemSprite;
    }

    public abstract void Use();
}
