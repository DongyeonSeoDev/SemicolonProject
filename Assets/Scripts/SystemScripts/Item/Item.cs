using UnityEngine;
using Water;

public class Item : MonoBehaviour
{
    [SerializeField] protected ItemSO _itemData;
    public ItemSO itemData { get { return _itemData; } }

    protected int droppedCount = 1;
    public int DroppedCnt { get { return droppedCount; } }

    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public virtual void SetData(int id, int droppedCount = 1)
    {
        _itemData = GameManager.Instance.GetItemData(id);
        spriteRenderer.sprite = _itemData.GetSprite();
        this.droppedCount = droppedCount;
    }
}
