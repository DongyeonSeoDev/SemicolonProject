using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BulletEffect : EnemyPoolData
    {
        private Animator animator;
        private readonly int hashAnimationPlay = Animator.StringToHash("AnimationPlay");

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Play(float angle)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
            animator.SetTrigger(hashAnimationPlay);

            Util.DelayFunc(() =>
            {
                gameObject.SetActive(false);
            }, 0.7f, this);
        }
    }
}
