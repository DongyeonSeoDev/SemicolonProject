using UnityEngine;

namespace Enemy
{
    public class EnemyEffect : EnemyPoolData
    {
        private SpriteRenderer spriteRenderer = null;
        private Animator animator = null;
        private readonly int hashAnimationStart = Animator.StringToHash("AnimationStart");

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public void Play(Color color)
        {
            spriteRenderer.color = color;
            animator.SetTrigger(hashAnimationStart);

            Util.DelayFunc(() =>
            {
                gameObject.SetActive(false);
            }, 0.7f, this);
        }
    }
}