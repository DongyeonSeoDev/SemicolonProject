using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water
{
    public class Item : MonoBehaviour
    {
        [SerializeField] protected ItemSO _itemData;
        public ItemSO itemData { get { return _itemData; } }

        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _itemData.GetSprite();
        }
    }
}
