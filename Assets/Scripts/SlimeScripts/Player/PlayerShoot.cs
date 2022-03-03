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

    [SerializeField]
    private float projectileDelayTime = 0.2f;
    private float projectileDelayTimer = 0f;

    private bool canShoot = true;

    public override void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;

        base.Awake();
    }
    private void Update()
    {
        CheckProjectileDelayTimer();
    }
    void FixedUpdate()
    {
        if (playerInput.IsShoot && !playerState.BodySlapping)
        {
            if (canShoot)
            {
                Shoot();
            }

            playerInput.IsShoot = false;
        }
        else if (playerInput.IsShoot)
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

        projectileDelayTimer = projectileDelayTime;
        canShoot = false;

        EventManager.TriggerEvent("PlayerShoot");
    }
    private void CheckProjectileDelayTimer()
    {
        if (projectileDelayTimer > 0f)
        {
            projectileDelayTimer -= Time.deltaTime;

            if (projectileDelayTimer <= 0f)
            {
                canShoot = true;
            }
        }
    }

}
