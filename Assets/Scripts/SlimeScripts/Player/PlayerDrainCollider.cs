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

    private void OnEnable()
    {
        drainTimer = drainTime;
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

            if (enemy != null)
            {
                enemy.EnemyDestroy();

                SlimeEventManager.TriggerEvent("OnDrain", other.gameObject.name, 1); // 여기의 param은 임시 값
                Debug.Log("Do Drain");
            }
        }
    }
}
