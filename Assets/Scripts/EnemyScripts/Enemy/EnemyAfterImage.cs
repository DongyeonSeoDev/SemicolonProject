using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Enemy
{
    public class EnemyAfterImage : EnemyPoolData
    {
        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void Init(Sprite sprite, Color color, float time, bool isFlipX)
        {
            sr.sprite = sprite;
            sr.color = color;
            sr.flipX = isFlipX;

            sr.DOFade(0f, time);
        }
    }
}