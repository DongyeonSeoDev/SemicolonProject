using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerState playerState = null;

    [SerializeField]
    private Stat playerStat = new Stat();
    public Stat PlayerStat
    {
        get { return playerStat; }
        set { playerStat = value; }
    }

    [SerializeField]
    private PlayerInput playerInput = null;
    public PlayerInput PlayerInput
    {
        get { return playerInput; }
    }

    private List<(GameObject, int)> drainList = new List<(GameObject, int)>();
    public List<(GameObject, int)> DrainList
    {
        get { return drainList; }
    }

    #region 에너지 관련 변수들
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
    public float EnergyRegenSpeed
    {
        get { return energyRegenSpeed; }
    }
    [Header("공격안할 때 에너지가 다시 차는 속도")]
    [SerializeField]
    private float energyRegenSpeedWhenNotAttack = 1.3f;
    public float EnergyRegenSpeedWhenNotAttack
    {
        get { return energyRegenSpeedWhenNotAttack; }
    }

    private float currentEnergy = 0f; // 현재의 에너지
    public float CurrentEnergy
    {
        get { return currentEnergy; }
    }
    #endregion

    #region 장착확률, 동화율이 오르는 값 변수들
    [Header("적을 죽일 때 오르는 장착확률의 값")]
    [SerializeField]
    private float upMountingPercentageValueWhenEnemyDead = 2f;
    public float UpMountingPercentageValueWhenEnemyDead
    {
        get { return upMountingPercentageValueWhenEnemyDead; }
    }

    [Header("적을 죽일 때 오르는 동화율의 값")]
    [SerializeField]
    private int upUnderstandingRateValueWhenEnemyDead = 1;
    public int UpUnderstadingRateValueWhenEnemyDead
    {
        get { return upUnderstandingRateValueWhenEnemyDead; }
    }

    [Header("특정 몹으로 변신하여 그 상태로 적을 죽였을 때 해당 몹에 대한 동화율이 오르는 값")]
    [SerializeField]
    private int upUnderstandingRateValueWhenEnemyDeadAfterBodyChanged = 3;
    public int UpUnderstandingRateValueWhenBodyChanged
    {
        get { return upUnderstandingRateValueWhenEnemyDeadAfterBodyChanged; }
    }
    #endregion

    private float currentHp = 0;
    public float CurrentHp
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        playerStat.additionalEternalStat = new EternalStat();

        playerState.IsDead = false;

        currentHp = playerStat.MaxHp;
        currentEnergy = maxEnergy;

        UIManager.Instance.UpdatePlayerHPUI();
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", PlayerDead);
        EventManager.StartListening("EnemyDead", EnemyDead);
        EventManager.StartListening("PlayerSetActiveFalse", SetActiveFalse);
        EventManager.StartListening("GameClear", WhenGameClear);

        playerState.IsDead = false;
    }
    private void Update()
    {
        if (currentHp <= 0)
        {
            EventManager.TriggerEvent("PlayerDead");
        }
        else
        {
            UpEnergy();
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", PlayerDead);
        EventManager.StopListening("EnemyDead", EnemyDead);
        EventManager.StopListening("PlayerSetActiveFalse", SetActiveFalse);
        EventManager.StopListening("GameClear", WhenGameClear);
    }
    private void UpEnergy()
    {
        currentEnergy += Time.deltaTime * (playerInput.IsDoSkill0 ? energyRegenSpeed : energyRegenSpeedWhenNotAttack);

        if (currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
    }
    public void UseEnergy(float useEventAmount)
    {
        currentEnergy -= useEventAmount;
    }
    public bool CheckEnergy(float useEngeryAmount)
    {
        if (SlimeGameManager.Instance.Player.CurrentEnergy < useEngeryAmount)
        {
            return false;
        }

        return true;
    }
    public void GetDamage(int damage, bool critical = false, bool stateAbnormality = false)
    {
        if (!playerState.IsDead)
        {
            int dm = damage;

            if (!stateAbnormality)
            {
                dm = damage - playerStat.Defense;
            }

            if (dm <= 0)
            {
                dm = 0;
            }

            currentHp -= dm;

            if (currentHp <= 0)
            {
                if (stateAbnormality)
                {
                    currentHp = 1;
                }
                else
                {
                    playerState.IsDead = true;
                }
            }

            EffectManager.Instance.OnDamaged(dm, critical, false, SlimeGameManager.Instance.CurrentPlayerBody.transform.position);
            UIManager.Instance.UpdatePlayerHPUI(true);
        }
    }
    public void GetDamage(GameObject attacker, int damage, bool critical = false, bool stateAbnormality = false)
    {
        foreach(var item in drainList)
        {
            if(item.Item1 == attacker)
            {
                return;
            }
        }
        
        if (!playerState.IsDead)
        {
            int dm = damage;

            if(!stateAbnormality)
            {
                dm = damage - playerStat.Defense;
            }

            if (dm <= 0)
            {
                dm = 0;
            }

            currentHp -= dm;

            if (currentHp <= 0)
            {
                if (stateAbnormality)
                {
                    currentHp = 1;
                }
                else
                {
                    playerState.IsDead = true;
                }
            }

            EffectManager.Instance.OnDamaged(dm, critical, false, SlimeGameManager.Instance.CurrentPlayerBody.transform.position); 
            UIManager.Instance.UpdatePlayerHPUI(true);
        }
    }
    public void GiveDamage(Enemy.Enemy targetEnemy, int minDamage, int maxDamage, float sturnTime = 1, float knockBackPower = 20, bool isKnockBack = true)
    {
        (int, bool) damage;
        damage.Item1 = Random.Range(minDamage, maxDamage + 1);
        damage.Item2 = false;

        damage = CriticalCheck(damage.Item1);

        targetEnemy.GetDamage(damage.Item1, damage.Item2, isKnockBack, knockBackPower, sturnTime);
    }
    public void Mag_GiveDamage(Enemy.Enemy targetEnemy, int minDamage, int maxDamage, float magnification, float strunTime = 1, float knockBackPower = 20, bool isKnockBack = true)
    {
        (int, bool) damage;
        damage.Item1 = Random.Range(minDamage, maxDamage + 1);
        damage.Item2 = false;

        damage = CriticalCheck(damage.Item1);

        damage.Item1 = (int)(damage.Item1 * magnification);

        targetEnemy.GetDamage(damage.Item1, damage.Item2, isKnockBack, knockBackPower, strunTime);
    }
    public (int, bool) CriticalCheck(int damage)
    {
        int n_damage = damage;
        bool isCritical = false;

        float checkRate = 0f;

        checkRate = Random.Range(0f, 100f);

        if(checkRate <= playerStat.CriticalRate)
        {
            n_damage += playerStat.CriticalDamage;
            isCritical = true;
        }

        return (n_damage, isCritical);
    }

    public void GetHeal(int healAmount)
    {
        if (!playerState.IsDead)
        {
            if (healAmount > 0)
            {
                currentHp += healAmount;

                if (currentHp > playerStat.MaxHp)
                {
                    currentHp = playerStat.MaxHp;
                }

                UIManager.Instance.UpdatePlayerHPUI();
            }
        }
    }
    private void WhenGameClear()
    {

    }
    private void PlayerDead()
    {
        EventManager.TriggerEvent("PlayerSetActiveFalse");
    }
    private void EnemyDead(string objId)
    {
        if (SlimeGameManager.Instance.CurrentBodyId == "origin")
        {
            if (PlayerEnemyUnderstandingRateManager.Instance.CheckMountObjIdContain(objId))
            {
                PlayerEnemyUnderstandingRateManager.Instance.UpUnderstandingRate(objId, upUnderstandingRateValueWhenEnemyDead);
            }
            else
            {
                PlayerEnemyUnderstandingRateManager.Instance.SetMountingPercentageDict(objId, PlayerEnemyUnderstandingRateManager.Instance.GetDrainProbabilityDict(objId) + upMountingPercentageValueWhenEnemyDead);
            }
        }
        else
        {
            Debug.Log("우오옷 동화율이 오른다앗");
            PlayerEnemyUnderstandingRateManager.Instance.UpUnderstandingRate(SlimeGameManager.Instance.CurrentBodyId, upUnderstandingRateValueWhenEnemyDeadAfterBodyChanged);
        }
    }
    private void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
    public void WhenRespawn()
    {
        playerStat.additionalEternalStat = new EternalStat();

        playerState.IsDead = false;
        currentHp = playerStat.MaxHp;

        UIManager.Instance.UpdatePlayerHPUI();

        SlimeGameManager.Instance.PlayerBodyChange("origin", true);

        EventManager.TriggerEvent("AfterPlayerRespawn");
    }

}
