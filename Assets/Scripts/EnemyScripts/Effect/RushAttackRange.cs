using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class RushAttackRange : EnemyPoolData
    {
        private Animator animator;

        private readonly int hashReset = Animator.StringToHash("Reset");

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            EventManager.StartListening("PlayerSetActiveFalse", EndEvent);
            EventManager.StartListening("BossDead", EndEvent);
        }

        public void AnimationReset()
        {
            animator.SetTrigger(hashReset);
        }

        public void EndEvent()
        {
            gameObject.SetActive(false);
        }
    }
}