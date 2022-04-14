using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class RushAttackRange : EnemyPoolData
    {
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