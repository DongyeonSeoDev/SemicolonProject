using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class CentipedeShootAroundBullet : EnemyBullet
    {
        private Vector2 originPos = Vector2.zero;

        [SerializeField]
        private float timerSpeed = 1f;
        [SerializeField]
        private float maxTimerSpeed = 10f;

        [SerializeField]
        private float gradient = 1f;
        [SerializeField]
        private float minTime = 0f;
        [SerializeField]
        private float maxTime = 5f; // max�� ��ǻ� Gizmos�θ� ���δ�

        [SerializeField]
        private float despawnDelay = 0f;
        [SerializeField]
        private float startTime = 0f;
        [SerializeField]
        private float endTime = 0f;

        [SerializeField]
        private float timer = 0f;
        [SerializeField]
        private float despawnTimer = 0f;

        private bool startDespawn = false;

        protected override void Update()
        {
            if(startDespawn)
            {
                despawnTimer -= Time.deltaTime;

                if(despawnTimer <= 0f)
                {
                    startDespawn = false;
                    timer = 0f;
                    gameObject.SetActive(false);
                }
            }
            else
            {
                if (timer >= endTime)
                {
                    // ��

                    despawnTimer = despawnDelay;
                    startDespawn = true;
                }

                timer += Time.deltaTime * timerSpeed;

                transform.position = originPos + GetPositionPerTime(timer);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            for (float i = 0f; i < maxTime; i += 0.1f)
            {
                Gizmos.DrawSphere(GetPositionPerTime(i), 0.1f);
            }

            Gizmos.color = Color.blue;

            for (float i = startTime; i < endTime; i += 0.1f)
            {
                Gizmos.DrawSphere(GetPositionPerTime(i), 0.1f);
            }
        }

        public void SetData(float timerSpeed, float gradient, float startTime)
        {
            if (timerSpeed > maxTimerSpeed)
            {
                timerSpeed = maxTimerSpeed;
            }

            this.timerSpeed = timerSpeed;
            this.gradient = gradient;
            this.startTime = startTime;
            endTime = maxTime + Random.Range(-maxTime / 4f, maxTime / 6f);
            originPos = transform.position;
        }

        public void ResetTimer(float startTime, float maxTime)
        {
            this.startTime = startTime;
            this.maxTime = maxTime;

            timer = startTime;
        }

        private Vector2 GetPositionPerTime(float time)
        {
            Vector2 result = Vector2.zero;

            float resultX = time;
            float resultY = gradient * (time - minTime) * (time - maxTime);

            result = new Vector2(resultX, resultY);
            result *= targetDirection.normalized;

            return result;
        }
    }
}
