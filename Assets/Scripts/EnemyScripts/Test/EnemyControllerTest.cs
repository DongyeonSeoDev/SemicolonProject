using UnityEngine;

namespace Enemy
{
    public class EnemyControllerTest : MonoBehaviour
    {
        public KeyCode testKeyCode;

        private void Update()
        {
            if (Input.GetKeyDown(testKeyCode))
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.EnemyControllerChange(EnemyController.PLAYER);
                }
            }
        }
    }
}