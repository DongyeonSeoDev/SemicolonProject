using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private SlimePoolManager slimePoolManager = null;

    private PlayerInput playerInput = null;
    private PlayerState playerState = null;


    [SerializeField]
    private GameObject projectile = null;

    [SerializeField]
    private float projectileSpeed = 1f;

    private void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;

        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
    }

    void FixedUpdate()
    {
        if (playerInput.IsShoot && !playerState.BodySlapping)
        {
            Shoot();

            playerInput.IsShoot = false;
        }
        else if(playerInput.IsShoot)
        {
            playerInput.IsShoot = false;
        }
    }
    private void Shoot()
    {
        GameObject temp = null;

        Vector2 direction = (playerInput.MousePosition - (Vector2)transform.position).normalized;

        bool findInDic = false;

        (temp, findInDic) = slimePoolManager.Find(projectile);

        if (findInDic)
        {
            temp.SetActive(true);
        }
        else
        {
            temp = Instantiate(projectile, transform);
        }

        temp.transform.position = transform.position;
        temp.GetComponent<PlayerProjectile>().OnSpawn(direction, projectileSpeed);

        EventManager.TriggerEvent("PlayerShoot");
    }
}
