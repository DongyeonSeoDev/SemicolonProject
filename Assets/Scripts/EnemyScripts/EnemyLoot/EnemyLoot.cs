using UnityEngine;

namespace Enemy
{
    public class EnemyLoot : EnemyPoolData
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