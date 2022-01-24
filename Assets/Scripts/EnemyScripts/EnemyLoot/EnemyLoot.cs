using UnityEngine;

namespace Enemy
{
    public class EnemyLoot : PoolManager
    {
        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void Init(Sprite sprite)
        {
            sr.sprite = sprite;
        }
    }
}