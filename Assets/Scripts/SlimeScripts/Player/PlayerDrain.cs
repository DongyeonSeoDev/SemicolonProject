using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrain : MonoBehaviour
{
    private PlayerInput playerInput = null;

    [SerializeField]
    private GameObject drainCollider = null; // drain 체크에 사용될 Collider
    private List<(GameObject, int)> drainList = new List<(GameObject, int)>();
    private Dictionary<GameObject, float> moveTimerDic = new Dictionary<GameObject, float>();

    [Header("Drain되는 오브젝트가 빨려들어오는 속도")]
    [SerializeField]
    private float drainSpeed = 2f;

    [Header("Drain 되는 오브젝트와 플레이어간의 거리가 이정도보다 작아져야 드레인 판정")]
    [SerializeField]
    private float drainDoneDistance = 0.1f;

    [SerializeField]
    private float reDrainTime = 10f;
    private float reDrainTimer = 0f;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        EventManager.StartListening("OnDrain", OnDrain);

        drainCollider.SetActive(false);
    }
    void Update()
    {
        if (reDrainTimer > 0f)
        {
            reDrainTimer -= Time.deltaTime;

            if (reDrainTimer <= 0f)
            {
                reDrainTimer = 0f;
            }
        }

        if (playerInput.IsDrain)
        {
            playerInput.IsDrain = false;

            if (reDrainTimer <= 0f)
            {
                reDrainTimer = reDrainTime;

                drainCollider.SetActive(true);
            }
        }

        DrainObjMove();
    }
    private void DrainObjMove()
    {
        if (drainList.Count > 0)
        {
            List<(GameObject, int)> removeList = new List<(GameObject, int)>();

            foreach (var item in drainList)
            {
                Vector2 direction = transform.position - item.Item1.transform.position;
                direction = direction.normalized;

                // x.Item1.transform.Translate(direction * Time.deltaTime);
                float distance = Vector2.Distance(transform.position, item.Item1.transform.position);
                float moveTime = distance / drainSpeed;

                item.Item1.transform.position = Vector2.Lerp(transform.position, direction, moveTimerDic[item.Item1] / moveTime);

                if (distance <= drainDoneDistance) // 흡수 판정 체크
                {
                    string objName = item.Item1.name;

                    PlayerEnemyUnderstandingRateManager.Instance.SetUnderstandingRate(objName,
                     PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(objName) + item.Item2);

                    Enemy.Enemy enemy = item.Item1.GetComponent<Enemy.Enemy>();

                    removeList.Add(item);

                    if (enemy != null)
                    {
                        enemy.EnemyDestroy();

                        continue;
                    }
                }

                if (moveTimerDic.ContainsKey(item.Item1))
                {
                    moveTimerDic[item.Item1] += Time.deltaTime;
                }
            }

            removeList.ForEach(x => {
                moveTimerDic.Remove(x.Item1);
                drainList.Remove(x);
            });
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnDrain", OnDrain);
    }
    private void OnDrain(GameObject obj, int upValue) // upValue는 이해도(동화율)이 얼마나 오를 것인가.
    {
        moveTimerDic.Add(obj, 0f);
        drainList.Add((obj, upValue));
    }
}
