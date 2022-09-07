using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : PlayerSkill
{
    private SlimePoolManager slimePoolManager = null;

    private PlayerShootDirectionControl playerShootDirectionControl = null;

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
        playerShootDirectionControl = GetComponent<PlayerShootDirectionControl>();

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

    public override void DoSkill()
    {
        base.DoSkill();

        if (canShoot && SlimeGameManager.Instance.Player.CheckEnergy(useEnergyAmount) && !playerState.BodySlapping)
        {
            canShoot = false;

            GameObject temp = null;
            bool findInDic = false;

            Vector2 mouseDirection = (playerInput.MousePosition - (Vector2)transform.position).normalized;
            Vector2 shootDirection = Vector2.zero;
            List<float> directionList = new List<float>();

            if (player.PlayerStat.choiceStat.multipleShots.statLv > 0)
            {
                if (player.PlayerStat.choiceStat.multipleShots.statLv <= playerShootDirectionControl.DirectionList.Count)
                {
                    directionList = playerShootDirectionControl.DirectionList[player.PlayerStat.choiceStat.multipleShots.statLv - 1].dataList;
                }
                else
                {
                    directionList = playerShootDirectionControl.DirectionList[playerShootDirectionControl.DirectionList.Count - 1].dataList;
                }
            }
            else
            {
                directionList.Add(0);
            }

            PlayerProjectile pTemp = null;
            int shootId = 0;
            for (int i = 0; i < directionList.Count; i++)
            {
                shootDirection = Quaternion.Euler(mouseDirection.x, mouseDirection.y, directionList[i]) * mouseDirection;
                (temp, findInDic) = slimePoolManager.Find(projectile);

                if (findInDic && temp != null)
                {
                    temp.SetActive(true);
                }
                else
                {
                    temp = Instantiate(projectile, SlimePoolManager.Instance.transform);
                }

                if(shootId == 0)
                {
                    shootId = temp.GetInstanceID();
                }

                temp.transform.position = (Vector2)transform.position + ((shootDirection).normalized * shootPosOffset);

                pTemp = temp.GetComponent<PlayerProjectile>();
                pTemp.OnSpawn((shootDirection).normalized, projectileSpeed);
                pTemp.shootId = shootId;

                if (directionList.Count <= 1)
                {
                    pTemp.isShootAlone = true;
                }
                else
                {
                    pTemp.isShootAlone = false;
                    PlayerProjectileControl.Instance.AddListDict(shootId, pTemp);
                }
            }

            CinemachineCameraScript.Instance.ShakeOrthoSize();

            SlimeGameManager.Instance.Player.UseEnergy(useEnergyAmount);
            SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];

            SoundManager.Instance.PlaySoundBox("SlimeSkill0");

            EventManager.TriggerEvent("PlayerShoot");
        }
    }

    public override void WhenSkillDelayTimerZero()
    {
        base.WhenSkillDelayTimerZero();

        canShoot = true;
    }
}
