using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : PlayerAction
{
    private SlimePoolManager slimePoolManager = null;

    [SerializeField]
    private GameObject projectile = null;

    [Header("총알 발사시에 사용하는 에너지")]
    [SerializeField]
    private float maxEnergy = 10f;

    [Header("총알을 발사할 때 마다 사용하는 에너지")]
    [SerializeField]
    private float useAmount = 1f;

    private float currentEnergy = 0f; // 현재의 에너지
    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set { currentEnergy = value; }
    }

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
    private void OnEnable() 
    {
        currentEnergy = maxEnergy;
    }
    void FixedUpdate()
    {
        CheckProjectileDelayTimer();
        
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
            projectileDelayTimer -= Time.fixedDeltaTime;

            if (projectileDelayTimer <= 0f)
            {
                canShoot = true;
            }
        }
    }

}
