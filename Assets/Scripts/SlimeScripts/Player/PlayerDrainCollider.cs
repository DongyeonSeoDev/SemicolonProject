using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrainCollider : MonoBehaviour
{
    [SerializeField]
    private LayerMask canDrainObjLayers;

    private PlayerDrain playerDrain = null;

    [SerializeField]
    private GameObject grabSoftBody = null;

    private List<ICanGetDamagableEnemy> tryDrainList = new List<ICanGetDamagableEnemy>();
    private List<ICanGetDamagableEnemy> doDrainList = new List<ICanGetDamagableEnemy>();

    private float drainTime = 3f;
    public float DrainTime
    {
        get { return drainTime; }
    }

    [Header("흡수되지 않을 적이 끌려올 때의 흡수되는 몹의 몇 %만큼 이동하는지 정하는 값")]
    [Header("이 값이 20이면 흡수되는 오브젝트가 이동하는 값의 1/5(20%)만큼 이동한다.")]
    [SerializeField]
    private float drainDisLessPercentageWhenFailed = 20f;
    public float DrainDisLessPercentageWhenFailed
    {
        get { return drainDisLessPercentageWhenFailed; }
    }

    [SerializeField]
    private float drainMoveSpeed = 1f;
    public float DrainSpeed
    {
        get { return drainMoveSpeed; }
    }

    private float drainTimer = 0f;

    private Dictionary<GameObject, Vector2> drainMoveOriginPosDict = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, Vector2> drainMoveTargetPosDict = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, float> drainMoveTimeDict = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> drainMoveTimerDict = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, int> paste_int_DrainMoveTimerDict = new Dictionary<GameObject, int>();

    [Header("Drain되는 오브젝트들이 몇 초에 한 번씩 위치가 갱신되는가")]
    [SerializeField]
    private float drainMoveUpdateTime = 2f;
    public float DrainMoveUpdateTIme
    {
        get { return drainMoveUpdateTime; }
    }

    [Header("흡수하려는 적의 HP가 해당 변수 이하의 퍼센트가 되어야 흡수 가능")]
    [SerializeField]
    private float canDrainHpPercentage = 10;
    public float CanDrainHpPercentage
    {
        get { return canDrainHpPercentage; }
    }

    private int int_timer = 0;

    private void Awake()
    {
        playerDrain = transform.parent.GetComponent<PlayerDrain>();
    }
    private void OnEnable()
    {
        drainTimer = drainTime;
    }
    void Update()
    {
        if (drainTimer > 0f)
        {
            List<ICanGetDamagableEnemy> removeList = new List<ICanGetDamagableEnemy>();
            drainTimer -= Time.deltaTime;

            if (drainTimer <= 0f)
            {
                try
                {
                    foreach (var item in tryDrainList)
                    {
                        removeList.Add(item);
                        RemoveList(item.GetGameObject());

                        if (doDrainList.Contains(item))
                        {
                            EventManager.TriggerEvent("OnDrain", item.GetGameObject(), item.GetTransform().position, 1); // 여기의 param은 임시 값
                        }
                    }
                }
                catch
                {
                    
                }

                foreach(var item in removeList)
                {
                    doDrainList.Remove(item);
                    tryDrainList.Remove(item);
                }

                SlimeGameManager.Instance.Player.PlayerOrderInLayerController.StartSetOrderInLayerAuto();
                SlimeGameManager.Instance.Player.PlayerState.IsDrain = false;

                EventManager.TriggerEvent("EnemyStart");

                gameObject.SetActive(false);
            }
        }

        CheckDrainMoveTime();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 튜토리얼 전용 스크립트를 만든다
        
        if (!playerDrain.cantDrainObject && canDrainObjLayers.CompareGameObjectLayer(other.gameObject))
        {
            // Debug.Log(other.gameObject.layer);
            //Drain되는 오브젝트는 삭제처리
            SlimeGameManager.Instance.Player.DrainList.Add(other.gameObject);

           ICanGetDamagableEnemy enemy = other.GetComponent<ICanGetDamagableEnemy>();

            Vector2 dir = (transform.position - other.transform.position).normalized;
            float hpPercentage = enemy.EnemyHpPercent();// 닿은 적의 현재 체력의 퍼센트를 구함

            if (hpPercentage <= 0f)
            {
                return;
            }

            float distance = Vector2.Distance(transform.position, enemy.GetTransform().position);
            float drainMoveTime = 0f;

            tryDrainList.Add(enemy);

            if (playerDrain.drainTutorial || (enemy != null && hpPercentage <= canDrainHpPercentage)) // 흡수 성공////////////////////
            {
                doDrainList.Add(enemy);
                SpawnGrabObj(enemy.GetGameObject());

                EventManager.TriggerEvent("TryDrain", other.transform.position, true);

                Debug.Log("Do Drain");
            }
            else if(enemy != null) // 흡수 실패
            {
                distance *= drainDisLessPercentageWhenFailed / 100f;

                Debug.Log(distance);

                EventManager.TriggerEvent("TryDrain", other.transform.position, false);
            }

            drainMoveTime = distance / drainMoveSpeed;

            if (drainMoveTime > drainTimer)
            {
                drainTimer = drainMoveTime;
                drainTime = drainTimer;
            }

            // 여기부턴 흡수 성공 혹은 실패한 오브젝트의 이동 관련 처리를 위한
            // 사전 준비작업

            EventManager.TriggerEvent("SetDrainTime", drainTime);

            if (!drainMoveOriginPosDict.ContainsKey(other.gameObject))
            {
                drainMoveOriginPosDict.Add(other.gameObject, other.transform.position);
                drainMoveTargetPosDict.Add(other.gameObject, other.transform.position + (Vector3)(dir * distance));
                drainMoveTimeDict.Add(other.gameObject, drainMoveTime);
                drainMoveTimerDict.Add(other.gameObject, 0f);
            }
        }
    }

    private void SpawnGrabObj(GameObject obj)
    {
        GameObject grabObj = null;
        bool spawned = false;

        (grabObj, spawned) = SlimePoolManager.Instance.Find(grabSoftBody);

        if(spawned)
        {
            grabObj.SetActive(true);
        }
        else
        {
            grabObj = Instantiate(grabSoftBody, SlimePoolManager.Instance.transform);
        }

        grabObj.GetComponent<GrabSoftBody>().OnSpawn(obj, Vector2.zero);
    }

    // Player.DoDrain의 역할을 대신하는 함수가 필요하다.
    // DoDrain과는 다르게, 흡수대상의 오브젝트들을 n초 단위로 끊어서 이동시킨다.
    // (시작 지점에서 n초 뒤에 있어야 하는 곳으로 이동할 때 그 곳으로 서서히 이동하는 것이 아닌 그 곳으로 바로 이동, 잠시 쉬었다 똑같이 이동 을 반복)
    // 일정 거리 이하로 가까워지면 흡수로 판단하고 PlayerDrain의 OnDrain함수를 호출한다.

    private void CheckDrainMoveTime()
    {
        List<ICanGetDamagableEnemy> removeList = new List<ICanGetDamagableEnemy> ();

        try
        {
            foreach (var item in tryDrainList)
            {
                GameObject key = item.GetGameObject();

                drainMoveTimerDict[key] += Time.deltaTime;

                if (drainMoveTimerDict[key] >= drainMoveTimeDict[key])
                {
                    RemoveList(key);
                    removeList.Add(item);

                    if (key.GetComponent<Enemy.DrainTutorialEnemy>() != null)
                    {
                        EventManager.TriggerEvent("DrainTutorialEnemyDrain", key.transform.position);

                        Destroy(key.gameObject);

                        return;
                    }

                    if (doDrainList.Contains(item))
                    {
                        EventManager.TriggerEvent("OnDrain", key, key.transform.position, 1); // 여기의 param은 임시 값
                    }

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
                    item.GetTransform().position = Vector2.Lerp(drainMoveOriginPosDict[key], drainMoveTargetPosDict[key], drainMoveTimerDict[key] / drainMoveTimeDict[key]);
                }
                else
                {
                    item.GetTransform().position = Vector2.Lerp(drainMoveOriginPosDict[key], drainMoveTargetPosDict[key], paste_int_DrainMoveTimerDict[key] / drainMoveTimeDict[key]);
                }
            }
        }
        catch
        {

        }

        foreach(var item in removeList)
        {
            doDrainList.Remove(item);
            tryDrainList.Remove(item);
        }
    }

    private void RemoveList(GameObject obj)
    {
        drainMoveOriginPosDict.Remove(obj);
        drainMoveTargetPosDict.Remove(obj);
        drainMoveTimeDict.Remove(obj);
        drainMoveTimerDict.Remove(obj);
        paste_int_DrainMoveTimerDict.Remove(obj);
    }
}
