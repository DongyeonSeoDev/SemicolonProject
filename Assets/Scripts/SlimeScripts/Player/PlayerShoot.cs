using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : PlayerAction
{
    private SlimePoolManager slimePoolManager = null;

    [SerializeField]
    private GameObject projectile = null;

    [Header("최대 에너지")]
    [SerializeField]
    private float maxEnergy = 10f;
    public float MaxEnergy
    {
        get { return maxEnergy; }
    }

    [Header("에너지가 다시 차는 속도")]
    [SerializeField]
    private float energyRegenSpeed = 1f;

    [Header("총알을 발사할 때 마다 깎이는 에너지의 양")]
    [SerializeField]
    private float useEnergyAmount = 1f;

    private float currentEnergy = 0f; // 현재의 에너지
    public float CurrentEnergy
    {
        get { return currentEnergy; }
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
    private void Start()
    {
        currentEnergy = maxEnergy;
    }
    void FixedUpdate()
    {
        CheckProjectileDelayTimer();
        UpEnergy();

        if (playerInput.IsShoot && CheckEnergy() && !playerState.BodySlapping)
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

        currentEnergy -= useEnergyAmount;
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
    private void UpEnergy()
    {
        currentEnergy += Time.fixedDeltaTime * energyRegenSpeed;

        if(currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
    }
    private bool CheckEnergy()
    {
        if(currentEnergy < useEnergyAmount)
        {
            return false;
        }
        
        return true;
    }
}
