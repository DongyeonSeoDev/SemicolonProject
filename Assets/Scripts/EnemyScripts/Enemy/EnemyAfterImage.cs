using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAfterImage : EnemyPoolData
    {
        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void Init(Sprite sprite, Color color, float time)
        {
            sr.sprite = sprite;
            sr.color = color;

            Util.DelayFunc(() =>
            {
                gameObject.SetActive(false);
            }, time);
        }
    }
}