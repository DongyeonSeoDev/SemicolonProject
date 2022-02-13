using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : PlayerAction
{
    private SlimePoolManager slimePoolManager = null;

    [SerializeField]
    private GameObject projectile = null;

    [SerializeField]
    private float projectileSpeed = 1f;

    public override void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;

        base.Awake();
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

        if (findInDic && temp != null)
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
