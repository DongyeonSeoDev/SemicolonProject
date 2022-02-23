using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrain : PlayerAction
{

    [SerializeField]
    private GameObject drainCollider = null; // drain 체크에 사용될 Collider

    private List<(GameObject, int)> drainList = new List<(GameObject, int)>();

    private Dictionary<GameObject, float> moveTimeDic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> moveTimerDic = new Dictionary<GameObject, float>();

    [Header("흡수를 했을 때의 장착 확률을 올려주는 수치")]
    [SerializeField]
    private float upMountingPercentageValue = 5f;
    [Header("흡수를 했을 때 장착한 적의 동화율을 올려주는 수치")]
    [SerializeField]
    private int upUnderstandingRateValue = 1;

    [Header("Drain되는 오브젝트가 빨려들어오는 속도")]
    [SerializeField]
    private float drainSpeed = 2f;

    [Header("Drain 되는 오브젝트와 플레이어간의 거리가 이정도보다 작아져야 드레인 판정")]
    [SerializeField]
    private float drainDoneDistance = 0.1f;

    [SerializeField]
    private float reDrainTime = 10f;
    private float reDrainTimer = 0f;

    public override void Awake()
    {
        base.Awake();

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
                // Vector2 direction = transform.position - item.Item1.transform.position;
                // direction = direction.normalized;

                // x.Item1.transform.Translate(direction * Time.deltaTime);
                float distance = Vector2.Distance(transform.position, item.Item1.transform.position);

                if (!moveTimeDic.ContainsKey(item.Item1))
                {
                    moveTimeDic.Add(item.Item1, distance / drainSpeed);
                }

                item.Item1.transform.position = Vector2.Lerp(item.Item1.transform.position, transform.position, moveTimerDic[item.Item1] / moveTimeDic[item.Item1]);
                Debug.Log(distance);

                if (distance <= drainDoneDistance) // 흡수 판정 체크
                {
                    Enemy.Enemy enemy = item.Item1.GetComponent<Enemy.Enemy>();
                    string objId = enemy.GetEnemyId();

                    PlayerEnemyUnderstandingRateManager.Instance.SetMountingPercentageDict(objId, PlayerEnemyUnderstandingRateManager.Instance.GetMountingPercentageDict(objId) + upMountingPercentageValue);

                    if (PlayerEnemyUnderstandingRateManager.Instance.CheckMountObjIdContain(objId))
                    {
                        if (CheckMountingEnemy(objId))
                        {
                            UpUnderStandingRate(objId, item.Item2);
                        }
                    }
                    else
                    {
                        UpUnderStandingRate(objId, item.Item2);
                    }

                    removeList.Add(item);

                    if (enemy != null)
                    {
                        Debug.Log(distance);
                        enemy.EnemyDestroy();

                        continue;
                    }
                }

                if (moveTimerDic.ContainsKey(item.Item1))
                {
                    moveTimerDic[item.Item1] += Time.deltaTime;
                }
            }

            removeList.ForEach(x =>
            {
                moveTimeDic.Remove(x.Item1);
                moveTimerDic.Remove(x.Item1);
                drainList.Remove(x);
            });
        }
    }
    private bool CheckMountingEnemy(string objId)
    {
        if (!PlayerEnemyUnderstandingRateManager.Instance.MountingPercentageDict.ContainsKey(objId))
        {
            return false;
        }

        float value = Random.Range(0f, 100f);

        if (value <= PlayerEnemyUnderstandingRateManager.Instance.GetMountingPercentageDict(objId)) // 확률 체크
        {
            if (!PlayerEnemyUnderstandingRateManager.Instance.CheckCanMountObj())
            {
                return false;
            }
            // 장착을 물어봄
            // 장착을 하면 true를, 장착을 하지 않으면 flase를 return함, 지금은 그냥 true를 return

            PlayerEnemyUnderstandingRateManager.Instance.SetMountObj(objId);

            return true;
        }
        else
        {
            return false;
        }
    }
    private void UpUnderStandingRate(string objId, int upValue) // 이해도(동화율)을 올려줌
    {
        PlayerEnemyUnderstandingRateManager.Instance.SetUnderstandingRate(objId,
            PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(objId) + upValue);

        if (PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(objId) > PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate) // 최대치 처리
        {
            PlayerEnemyUnderstandingRateManager.Instance.SetUnderstandingRate(objId, PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate);
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
