using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrainCollider : MonoBehaviour
{
    [SerializeField]
    private LayerMask canDrainObjLayers;

    [SerializeField]
    private float drainTime = 0.5f;
    private float drainTimer = 0f;

    [Header("흡수하려는 적의 HP가 해당 변수 이하의 퍼센트가 되어야 흡수 가능")]
    [SerializeField]
    private float canDrainHpPercentage = 10;

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
                gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canDrainObjLayers.CompareGameObjectLayer(other.gameObject))
        {
            // Debug.Log(other.gameObject.layer);
            // Drain되는 오브젝트는 삭제처리
            Enemy.Enemy enemy = other.GetComponent<Enemy.Enemy>();

            float hpPercentage = enemy.EnemyHpPercent();// 닿은 적의 현재 체력의 퍼센트를 구함

            if (hpPercentage <= 0f)
            {
                return;
            }

            // Debug.Log(hpPercentage);

            if (enemy != null && hpPercentage <= canDrainHpPercentage)
            {
                // enemy.EnemyDestroy();

                EventManager.TriggerEvent("TryDrain", other.transform.position, true);
                EventManager.TriggerEvent("OnDrain", other.gameObject, other.transform.position, 1); // 여기의 param은 임시 값
              
                Debug.Log("Do Drain");
            }
            else if(enemy != null)
            {
                EventManager.TriggerEvent("TryDrain", other.transform.position, false);
            }
        }
    }
}
