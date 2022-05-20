using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerState playerState = null;
    public PlayerState PlayerState
    {
        get { return playerState; }
    }

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

    [SerializeField]
    private PlayerChoiceStatControl playerChoiceStatControl = null;
    public PlayerChoiceStatControl PlayerChoiceStatControl
    {
        get { return playerChoiceStatControl; }
    }

    [SerializeField]
    private OrderInLayerConroller playerOrderInLayerController = null;
    public OrderInLayerConroller PlayerOrderInLayerController
    {
        get {
            if(playerOrderInLayerController == null)
            {
                playerOrderInLayerController = GetComponentInChildren<OrderInLayerConroller>();
            }

            return playerOrderInLayerController;
        }
    }

    private List<GameObject> coveredObjectList = new List<GameObject>();
    public List<GameObject> CoveredObjectList
    {
        get { return coveredObjectList; }
    }

    private float getExtraDamagePercantage = 0f;
    /// <summary>
    /// 추가 데미지 퍼센테이지가 20%이면 이 변수에 20을 넣으면 된다.
    /// </summary>
    public float GetExtraDamagePercantage
    {
        get { return getExtraDamagePercantage; }
        set { 
            getExtraDamagePercantage = value;

            if(getExtraDamagePercantage < 0f)
            {
                getExtraDamagePercantage = 0f;
            }
        }
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

    private float originEternalSpeed = 0f;
    private float originAdditionalSpeed = 0f;
    private bool speedSlowStart = false;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        playerInput = GetComponent<PlayerInput>();
        playerChoiceStatControl = GetComponent<PlayerChoiceStatControl>();

        playerOrderInLayerController = GetComponentInChildren<OrderInLayerConroller>();
    }
    private void Start()
    {
        playerStat.additionalEternalStat = new EternalStat();

        playerState.IsDead = false;

        playerStat.currentHp = playerStat.MaxHp;
        currentEnergy = maxEnergy;

        UIManager.Instance.UpdatePlayerHPUI();
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", PlayerDead);
        EventManager.StartListening("EnemyDead", EnemyDead);
        EventManager.StartListening("PlayerSetActiveFalse", SetActiveFalse);
        EventManager.StartListening("GameClear", WhenGameClear);
        EventManager.StartListening("ChangeBody", OnChangeBody);
        EventManager.StartListening("StartPlayerSlow", StartPlayerSlow);
        EventManager.StartListening("StopPlayerSlow", StopPlayerSlow);

        playerState.IsDead = false;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.RightControl))
        {
            Debug.Log("RightControl");
        }

        if (playerStat.currentHp <= 0)
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
        EventManager.StopListening("ChangeBody", OnChangeBody);
        EventManager.StopListening("StartPlayerSlow", StartPlayerSlow);
        EventManager.StopListening("StopPlayerSlow", StopPlayerSlow);
    }
    private void OnChangeBody()
    {
        playerOrderInLayerController = GetComponentInChildren<OrderInLayerConroller>();
    }
    private void UpEnergy()
    {
        if(playerState.IsDrain)
        {
            return;
        }

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
    public void GetDamage(float damage, Vector2 position, Vector2 direction, Vector3 size, bool isUseParticle = true, bool critical = false, bool stateAbnormality = false)
    {
        if ((playerState.BodySlapping && !stateAbnormality) ||
            (playerState.IsDrain && !stateAbnormality) ||
            SlimeGameManager.Instance.GameClear)
        {
            return;
        }

        if (!playerState.IsDead)
        {
            float dm = damage;

            if (!stateAbnormality) // 효과데미지 아닐 때
            {
                dm = damage - playerStat.Defense;
                dm += (int)(dm * getExtraDamagePercantage / 100f);
            }

            if (dm <= 0)
            {
                dm = 0;
            }
            else
            {
                playerChoiceStatControl.TotalDamage += dm;
            }

            playerStat.currentHp -= dm;

            SlimeGameManager.Instance.Player.PlayerChoiceStatControl.CheckEndurance();

            if (playerStat.currentHp <= 0)
            {
                if (stateAbnormality)
                {
                    playerStat.currentHp = 1;
                }
                else
                {
                    playerState.IsDead = true;
                }
            }

            EffectManager.Instance.OnDamaged(dm, critical, false, SlimeGameManager.Instance.CurrentPlayerBody.transform.position, position, direction, size, isUseParticle);
            UIManager.Instance.UpdatePlayerHPUI(true);
        }
    }
    /// <summary>
    /// EventManager.TriggerEvent("PlayerOnDamage", GameObject); 를 항상 같이 호출해줄것!
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="damage"></param>
    /// <param name="critical"></param>
    /// <param name="stateAbnormality"></param>
    public void GetDamage(GameObject attacker, float damage, Vector2 position, Vector2 direction, Vector3 size, bool critical = false, bool stateAbnormality = false)
    {
        if ((playerState.BodySlapping && !stateAbnormality) ||
            (playerState.IsDrain && !stateAbnormality) ||
            SlimeGameManager.Instance.GameClear)
        {
            return;
        }

        //SlimeGameManager.Instance.playerHitCheckDict.Add(attacker, false);

        if (SlimeGameManager.Instance.playerHitCheckDict.ContainsKey(attacker))
        {
            SlimeGameManager.Instance.playerHitCheckDict[attacker] = true;
        }

        if (!playerState.IsDead)
        {
            float dm = damage;

            if (!stateAbnormality) // 효과데미지 아닐 때
            {
                dm = damage - playerStat.Defense;
                dm += (int)(dm * (getExtraDamagePercantage / 100f));
            }

            if (dm <= 0)
            {
                dm = 0;
            }
            else
            {
                playerChoiceStatControl.TotalDamage += dm;
            }

            playerStat.currentHp -= dm;

            SlimeGameManager.Instance.Player.PlayerChoiceStatControl.CheckEndurance();

            if (playerStat.currentHp <= 0)
            {
                if (stateAbnormality)
                {
                    playerStat.currentHp = 1;
                }
                else
                {
                    playerState.IsDead = true;
                }
            }

            EffectManager.Instance.OnDamaged(dm, critical, false, SlimeGameManager.Instance.CurrentPlayerBody.transform.position, position, direction, size);
            UIManager.Instance.UpdatePlayerHPUI(true);
        }
    }
    public void GiveDamage(ICanGetDamagableEnemy targetEnemy, Vector2 direction, Vector2 position, float minDamage, float maxDamage, float stunTime = 1, float knockBackPower = 20, bool isKnockBack = true, Vector3? size = null)
    {
        (float, bool) damage;
        damage.Item1 = Random.Range(minDamage, maxDamage + 1);
        damage.Item2 = false;

        damage = CriticalCheck(damage.Item1);

        targetEnemy.GetDamage(damage.Item1, damage.Item2, isKnockBack, stunTime > 0, direction, position, true, knockBackPower, stunTime, size);
    }
    public void Mag_GiveDamage(ICanGetDamagableEnemy targetEnemy, Vector2 direction, Vector2 position, float minDamage, float maxDamage, float magnification, float stunTime = 1, float knockBackPower = 20, bool isKnockBack = true, Vector3? size = null)
    {
        (float, bool) damage;
        damage.Item1 = Random.Range(minDamage, maxDamage + 1);
        damage.Item2 = false;

        damage = CriticalCheck(damage.Item1);

        damage.Item1 = (int)(damage.Item1 * magnification);

        targetEnemy.GetDamage(damage.Item1, damage.Item2, isKnockBack, stunTime > 0, direction, position, true, knockBackPower, stunTime, size);
    }
    public (float, bool) CriticalCheck(float damage)
    {
        float n_damage = damage;
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
                playerStat.currentHp += healAmount;

                if (playerStat.currentHp > playerStat.MaxHp)
                {
                    playerStat.currentHp = playerStat.MaxHp;
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
        playerStat.additionalEternalStat = new EternalStat();

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
                PlayerEnemyUnderstandingRateManager.Instance.UpDrainProbabilityDict(objId, upMountingPercentageValueWhenEnemyDead);
            }
        }
        else
        {
            //Debug.Log("우오옷 동화율이 오른다앗");
            PlayerEnemyUnderstandingRateManager.Instance.UpUnderstandingRate(SlimeGameManager.Instance.CurrentBodyId, upUnderstandingRateValueWhenEnemyDeadAfterBodyChanged);
        }
    }
    private void StartPlayerSlow()
    {
        if(!speedSlowStart)
        {
            speedSlowStart = true;

            originEternalSpeed = playerStat.eternalStat.speed.statValue;
            originAdditionalSpeed = playerStat.additionalEternalStat.speed.statValue;

            playerStat.eternalStat.speed.statValue = 0.1f;
            playerStat.additionalEternalStat.speed.statValue = 0f;
        }
    }
    private void StopPlayerSlow()
    {
        if (speedSlowStart)
        {
            speedSlowStart = false;

            playerStat.eternalStat.speed.statValue = originEternalSpeed;
            playerStat.additionalEternalStat.speed.statValue = originAdditionalSpeed;
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
        playerStat.currentHp = playerStat.MaxHp;

        UIManager.Instance.UpdatePlayerHPUI();

        SlimeGameManager.Instance.PlayerBodyChange("origin", true);

        EventManager.TriggerEvent("AfterPlayerRespawn");
    }
}
