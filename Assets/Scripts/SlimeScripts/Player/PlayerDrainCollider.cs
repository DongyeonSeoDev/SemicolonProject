using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrainCollider : MonoBehaviour
{
    [SerializeField]
    private LayerMask canDrainObjLayers;

    private List<Enemy.Enemy> doDrainList = new List<Enemy.Enemy>();

    private float drainTime = 0.5f;
    public float DrainTime
    {
        get { return drainTime; }
    }

    [SerializeField]
    private float drainMoveSpeed = 1f;
    public float DrainSpeed
    {
        get { return drainMoveSpeed; }
    }

    private float drainTimer = 0f;

    private Dictionary<GameObject, Vector2> drainMoveOriginPosDict = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, float> drainMoveTimeDict = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> drainMoveTimerDict = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, int> paste_int_DrainMoveTimerDict = new Dictionary<GameObject, int>();

    [Header("Drain되는 오브젝트들이 몇 초에 한 번씩 위치가 갱신되는가")]
    [SerializeField]
    private float drainMoveUpdateTime = 2f;

    [Header("흡수하려는 적의 HP가 해당 변수 이하의 퍼센트가 되어야 흡수 가능")]
    [SerializeField]
    private float canDrainHpPercentage = 10;

    private int int_timer = 0;

    private void OnEnable()
    {
        drainTimer = drainTime;
    }
    private void Start()
    {
        //EventManager.StartListening("ssss", );
    }
    void Update()
    {
        if (drainTimer > 0f)
        {
            drainTimer -= Time.deltaTime;

            if (drainTimer <= 0f)
            {
                try
                {
                    foreach (var item in doDrainList)
                    {
                        EventManager.TriggerEvent("OnDrain", item.gameObject, item.transform.position, 1); // 여기의 param은 임시 값
                        RemoveList(item.gameObject);
                    }
                }
                catch
                {

                }

                SlimeGameManager.Instance.Player.PlayerState.IsDrain = false;

                gameObject.SetActive(false);
            }
        }

        CheckDrainMoveTime();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canDrainObjLayers.CompareGameObjectLayer(other.gameObject))
        {
            // Debug.Log(other.gameObject.layer);
            // Drain되는 오브젝트는 삭제처리

            SlimeGameManager.Instance.Player.DrainList.Add(other.gameObject);
            Enemy.Enemy enemy = other.GetComponent<Enemy.Enemy>();

            enemy.GetDamage(1, false, false, 0, drainMoveUpdateTime);

            float hpPercentage = enemy.EnemyHpPercent();// 닿은 적의 현재 체력의 퍼센트를 구함

            if (hpPercentage <= 0f)
            {
                return;
            }


            // Debug.Log(hpPercentage);

            if (enemy != null && hpPercentage <= canDrainHpPercentage)
            {
                // enemy.EnemyDestroy();

                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                float drainMoveTime = distance / drainMoveSpeed;

                drainTimer += drainMoveTime - drainTimer;
                drainTime = drainTimer;

                EventManager.TriggerEvent("SetDrainTime", drainTime);

                doDrainList.Add(enemy);
                drainMoveOriginPosDict.Add(other.gameObject, other.transform.position);
                drainMoveTimeDict.Add(other.gameObject, drainMoveTime);
                drainMoveTimerDict.Add(other.gameObject, 0f);

                EventManager.TriggerEvent("TryDrain", other.transform.position, true);
              
                Debug.Log("Do Drain");
            }
            else if(enemy != null)
            {
                EventManager.TriggerEvent("TryDrain", other.transform.position, false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // DoDrainList에서 빼주는 처리
        if(canDrainObjLayers.CompareGameObjectLayer(other.gameObject))
        {
            RemoveList(other.gameObject);
        }
    }

    // PlayerDoDrain의 역할을 대신하는 함수가 필요하다.
    // DoDrain과는 다르게, 흡수대상의 오브젝트들을 n초 단위로 끊어서 이동시킨다.
    // (시작 지점에서 n초 뒤에 있어야 하는 곳으로 이동할 때 그 곳으로 서서히 이동하는 것이 아닌 그 곳으로 바로 이동, 잠시 쉬었다 똑같이 이동 을 반복)
    // 일정 거리 이하로 가까워지면 흡수로 판단하고 PlayerDrain의 OnDrain함수를 호출한다.

    private void CheckDrainMoveTime()
    {
        try
        {
            foreach (var item in doDrainList)
            {
                GameObject key = item.gameObject;

                drainMoveTimerDict[key] += Time.deltaTime;

                if (drainMoveTimerDict[key] >= drainMoveTimeDict[key])
                {
                    EventManager.TriggerEvent("OnDrain", key, key.transform.position, 1); // 여기의 param은 임시 값
                    RemoveList(key);

                    continue;
                }

                bool updateObj = false;
                int_timer = (int)drainMoveTimerDict[key];

                if (!paste_int_DrainMoveTimerDict.ContainsKey(key))
                {
                    paste_int_DrainMoveTimerDict.Add(key, -1);
                }

                if (int_timer != paste_int_DrainMoveTimerDict[key])
                {
                    updateObj = true;
                }

                paste_int_DrainMoveTimerDict[key] = int_timer;

                if (drainMoveTimerDict[key] % drainMoveUpdateTime <= 0.1f && updateObj)
                {
                    item.GetDamage(1, false, false, 0, drainMoveUpdateTime);
                    item.transform.position = Vector2.Lerp(drainMoveOriginPosDict[key], transform.position, drainMoveTimerDict[key] / drainMoveTimeDict[key]);
                }
                else
                {
                    item.transform.position = Vector2.Lerp(drainMoveOriginPosDict[key], transform.position, paste_int_DrainMoveTimerDict[key] / drainMoveTimeDict[key]);
                }
            }
        }
        catch
        {

        }
    }

    private void RemoveList(GameObject obj)
    {
        doDrainList.Remove(obj.GetComponent<Enemy.Enemy>());
        drainMoveOriginPosDict.Remove(obj);
        drainMoveTimeDict.Remove(obj);
        drainMoveTimerDict.Remove(obj);
        paste_int_DrainMoveTimerDict.Remove(obj);
    }
}
