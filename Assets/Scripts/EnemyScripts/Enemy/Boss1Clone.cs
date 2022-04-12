using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Boss1Clone : EnemyPoolData
    {
        public Rigidbody2D rigid;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            EventManager.StartListening("PlayerSetActiveFalse", PlayerDeadEvent);
        }

        public void PlayerDeadEvent()
        {
            gameObject.SetActive(false);
        }
    }
}