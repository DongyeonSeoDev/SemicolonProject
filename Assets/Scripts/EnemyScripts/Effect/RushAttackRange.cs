using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class RushAttackRange : EnemyPoolData
    {
        private void Start()
        {
            EventManager.StartListening("PlayerSetActiveFalse", EndEvent);
            EventManager.StartListening("BossDead", EndEvent);
        }

        public void EndEvent()
        {
            gameObject.SetActive(false);
        }
    }
}