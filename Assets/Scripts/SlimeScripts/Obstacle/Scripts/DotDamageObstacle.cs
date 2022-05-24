using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotDamageObstacle : MonoBehaviour
{
    [SerializeField]
    private int dotDamage = 0;
    [SerializeField]
    private float damageDelay = 0;
    private float damageTimer = 0;

    private bool isPlayerIn = false;
    private void Update() 
    {
        GetGetDamage();
    }
    private void GetGetDamage()
    {
        damageTimer += Time.deltaTime;

        if(isPlayerIn && damageTimer >= damageDelay)
        {
            SlimeGameManager.Instance.Player.GetDamage(gameObject, dotDamage, transform.position, Enemy.EnemyManager.Instance.transform.position - transform.position, effectSize: Vector3.zero);

            damageTimer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            isPlayerIn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            isPlayerIn = false;
        }
    }
}
