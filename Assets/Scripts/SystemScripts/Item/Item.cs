using UnityEngine;

namespace Water
{
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
            spriteRenderer.sprite = _itemData.GetSprite();
        }
    }
}
