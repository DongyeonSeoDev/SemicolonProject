using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class TestEnemy : MonoBehaviour // �� �׽�Ʈ �ڵ�
    {
        public Enemy testEnemy;

        private void Start()
        {
            Invoke("StartEnemyFollow", 3f);
        }

        private void StartEnemyFollow()
        {
            Debug.Log("StartFollow");

            testEnemy.StartFollow();
            Invoke("EndEnemyFollow", 10f);
        }

        private void EndEnemyFollow()
        {
            Debug.Log("EndFollow");

            testEnemy.EndFollow();
        }
    }
}