using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : PlayerSkill
{
    private SlimePoolManager slimePoolManager = null;

    [SerializeField]
    private GameObject projectile = null;

    [SerializeField]
    private float shootPosOffset = 0f;

    [SerializeField]
    private float projectileSpeed = 1f;

    [Header("총알을 발사할 때 마다 깎이는 에너지의 양")]
    [SerializeField]
    private float useEnergyAmount = 1f;

    private bool canShoot = true;

    public override void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;

        base.Awake();
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    public override void Update()
    {
        base.Update();
    }
    void FixedUpdate()
    {
        //CheckSkillDelay();
    }
    public override void DoSkill()
    {
        base.DoSkill();

        if (canShoot && SlimeGameManager.Instance.Player.CheckEnergy(useEnergyAmount) && !playerState.BodySlapping)
        {
            canShoot = false;

            GameObject temp = null;
            bool findInDic = false;

            Vector2 direction = (playerInput.MousePosition - (Vector2)transform.position).normalized;

            (temp, findInDic) = slimePoolManager.Find(projectile);

            if (findInDic && temp != null)
            {
                temp.SetActive(true);
            }
            else
            {
                temp = Instantiate(projectile, SlimePoolManager.Instance.transform);
            }

            temp.transform.position = (Vector2)transform.position + (direction * shootPosOffset);
            temp.GetComponent<PlayerProjectile>().OnSpawn(direction, projectileSpeed);

            SlimeGameManager.Instance.Player.UseEnergy(useEnergyAmount);
            SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];

            SoundManager.Instance.PlaySoundBox("SlimeSkill0");

            EventManager.TriggerEvent("PlayerShoot");
        }

        //playerInput.IsDoSkill0 = false;
    }
    public override void WhenSkillDelayTimerZero()
    {
        base.WhenSkillDelayTimerZero();

        canShoot = true;
    }
    //private void CheckProjectileDelayTimer()
    //{
    //    if (projectileDelayTimer > 0f)
    //    {
    //        projectileDelayTimer -= Time.fixedDeltaTime;

    //        if (projectileDelayTimer <= 0f)
    //        {
    //            canShoot = true;
    //        }
    //    }
    //}
}
