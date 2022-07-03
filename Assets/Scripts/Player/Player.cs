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

    private float prevAttackSpeed = 0f;

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
    private PlayerReflectionScript playerReflectionScript = null;
    public PlayerReflectionScript PlayerReflectionScript
    {
        get { return playerReflectionScript; }
    }

    [SerializeField]
    private OrderInLayerController playerOrderInLayerController = null;
    public OrderInLayerController PlayerOrderInLayerController
    {
        get {
            if(playerOrderInLayerController == null)
            {
                playerOrderInLayerController = GetComponentInChildren<OrderInLayerController>();
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

    [Header("전체적인 에너지가 다시 차는 속도")]
    [SerializeField]
    private float totalEnergyReganSpeed = 1f;
    public float TotalEnergyReganSpeed
    {
        get { return totalEnergyReganSpeed; }
    }

    [Header("공격할 때 에너지가 다시 차는 속도")]
    [SerializeField]
    private float energyRegenSpeedAttack = 1f;
    public float EnergyRegenSpeedAttack
    {
        get { return energyRegenSpeedAttack; }
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

    private float fakePercentage = 0f;
    public float FakePercentage
    {
        get { return fakePercentage; }
        set { fakePercentage = value; }
    }

    private bool speedSlowStart = false;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        playerInput = GetComponent<PlayerInput>();
        playerChoiceStatControl = GetComponent<PlayerChoiceStatControl>();
        playerReflectionScript = GetComponent<PlayerReflectionScript>();

        playerOrderInLayerController = GetComponentInChildren<OrderInLayerController>();
    }
    private void Start()
    {
        playerStat.additionalEternalStat.ResetAdditional();

        playerState.IsDead = false;

        playerStat.currentHp = playerStat.MaxHp;
        currentEnergy = maxEnergy;

        ////UIManager.Instance.UpdatePlayerHPUI();
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
        if(prevAttackSpeed != playerStat.AttackSpeed)
        {
            prevAttackSpeed = playerStat.AttackSpeed;

            EventManager.TriggerEvent("OnAttackSpeedChage");
        }

        CheckTotalEnergySpeed();

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
        playerOrderInLayerController = GetComponentInChildren<OrderInLayerController>();
    }
    private void UpEnergy()
    {
        if(playerState.IsDrain || SlimeGameManager.Instance.CurrentBodyId != "origin")
        {
            playerChoiceStatControl.MucusChargeEnergyMax = false;

            return;
        }

        currentEnergy += Time.deltaTime * (playerInput.IsDoSkill0 ? energyRegenSpeedAttack : energyRegenSpeedWhenNotAttack) * totalEnergyReganSpeed;

        if (currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;

            playerChoiceStatControl.MucusChargeEnergyMax = true;
        }
        else if(playerChoiceStatControl.MucusChargeEnergyMax)
        {
            playerChoiceStatControl.MucusChargeEnergyMax = false;
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
    public void GetDamage(float damage, Vector2 effectPosition, Vector2 direction, Vector3? effectSize = null, bool critical = false, bool stateAbnormality = false)
    {
        if ((playerState.IsDrain && !stateAbnormality) ||
            SlimeGameManager.Instance.GameClear)
        {
            return;
        }


        if((playerState.BodySlapping && !stateAbnormality))
        {
            return;
        }

        if (playerStat.choiceStat.fake.isUnlock && !stateAbnormality && CheckFake())
        {
            damage = 0f;
        }

        float dm = (int)damage;

        if (TutorialManager.Instance.hpUI.gameObject.activeSelf)
        {
            if (!playerState.IsDead)
            {
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
                    playerChoiceStatControl.UpTotalDamage(dm);
                }

                playerStat.currentHp -= dm;

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
                UIManager.Instance.UpdatePlayerHPUI(true);
            }
        }

        EffectManager.Instance.OnDamaged(dm, critical, false, SlimeGameManager.Instance.CurrentPlayerBody.transform.position, effectPosition, direction, effectSize);
        EventManager.TriggerEvent("PlayerGetDamaged");
    }
    /// <summary>
    /// EventManager.TriggerEvent("PlayerOnDamage", GameObject); 를 항상 같이 호출해줄것!
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="damage"></param>
    /// <param name="critical"></param>
    /// <param name="stateAbnormality"></param>
    /// 
    // 투사체가 플레이어에게 데미지를 줄때는 무조건 attacker를 매개변수로 보내줘야한다.
    public void GetDamage(GameObject attacker, float damage, Vector2 effectPosition, Vector2 direction, Vector3? effectSize = null, bool critical = false, bool stateAbnormality = false)
    {
        if (SlimeGameManager.Instance.GameClear)
        {
            return;
        }

        if ((playerState.BodySlapping && !stateAbnormality))
        {
            if (playerStat.choiceStat.reflection.isUnlock)
            {
                var bullet = attacker.GetComponent<Enemy.EnemyBullet>();

                if (bullet != null && !bullet.isReflection)
                {
                    Vector2 dir1 = -(direction.normalized);
                    Vector2 dir2 = playerInput.LastBodySlapVector;

                    playerReflectionScript.DoReflection(bullet.poolType, (dir1 + dir2).normalized, Global.CurrentPlayer.PlayerStat.MinDamage, Global.CurrentPlayer.PlayerStat.MinDamage, 0, 0);
                }
            }

            return;
        }

        if (playerState.IsDrain && !stateAbnormality)
        {
            return;
        }

        if (playerStat.choiceStat.fake.isUnlock && !stateAbnormality && CheckFake())
        {
            damage = 0f;
        }

        //SlimeGameManager.Instance.playerHitCheckDict.Add(attacker, false);

        float dm = (int)damage;

        if (TutorialManager.Instance.hpUI.gameObject.activeSelf)
        {
            if (SlimeGameManager.Instance.playerHitCheckDict.ContainsKey(attacker))
            {
                SlimeGameManager.Instance.playerHitCheckDict[attacker] = true;
            }

            if (!playerState.IsDead)
            {
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
                    playerChoiceStatControl.UpTotalDamage(dm);
                }

                playerStat.currentHp -= dm;

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

                UIManager.Instance.UpdatePlayerHPUI(true);
            }
        }

        EffectManager.Instance.OnDamaged(dm, critical, false, SlimeGameManager.Instance.CurrentPlayerBody.transform.position, effectPosition, direction, effectSize);
        EventManager.TriggerEvent("PlayerGetDamaged");
    }
    public bool CheckFake()
    {
        if(!playerStat.choiceStat.fake.isUnlock)
        {
            return false;
        }

        float value = 0f;

        value = Random.Range(0f, 100f);

        if(value <= fakePercentage)
        {
            playerChoiceStatControl.UpFakeNum();

            return true;
        }

        return false;
    }
    public void CheckTotalEnergySpeed()
    {
        if(!playerStat.choiceStat.mucusRecharge.isUnlock)
        {
            return;
        }

        totalEnergyReganSpeed = playerStat.choiceStat.mucusRecharge.statValue 
            * playerChoiceStatControl.ChoiceDataDict[NGlobal.MucusRechargeID].upTargetStatPerChoiceStat + 1f;
    }
    public (float, bool) GiveDamage(ICanGetDamagableEnemy targetEnemy, float minDamage, float maxDamage, Vector2 effectPosition, Vector2 direction, bool isKnockBack = true, float knockBackPower = 20, float stunTime = 1, Vector3? effectSize = null)
    {
        (float, bool) damage;
        damage.Item1 = Random.Range(minDamage, maxDamage + 1);
        damage.Item2 = false;

        damage = CriticalCheck(damage.Item1);

        targetEnemy.GetDamage(damage.Item1, damage.Item2, isKnockBack, stunTime > 0, effectPosition, direction, knockBackPower, stunTime, effectSize);

        return damage;
    }

    public void Mag_GiveDamage(ICanGetDamagableEnemy targetEnemy, float minDamage, float maxDamage, Vector2 effectPosition, Vector2 direction, float magnification, bool isKnockBack = true, float knockBackPower = 20, float stunTime = 1, Vector3? effectSize = null)
    {
        (float, bool) damage;
        damage.Item1 = Random.Range(minDamage, maxDamage + 1);
        damage.Item2 = false;

        damage = CriticalCheck(damage.Item1);

        damage.Item1 = (int)(damage.Item1 * magnification);

        targetEnemy.GetDamage(damage.Item1, damage.Item2, isKnockBack, stunTime > 0, effectPosition, direction, knockBackPower, stunTime, effectSize);
    }
    public (float, bool) CriticalCheck(float damage)
    {
        float n_damage = damage;
        bool isCritical = false;

        float checkRate = 0f;

        checkRate = Random.Range(0f, 100f);

        if(checkRate <= playerStat.CriticalRate)
        {
            if (playerStat.CriticalDamage <= 1f)
            {
                n_damage += damage;
            }
            else
            {
                n_damage += damage + ((playerStat.CriticalDamage - 1) * (damage / 10f));
            }

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
        playerStat.additionalEternalStat.ResetAdditional();

        EventManager.TriggerEvent("PlayerSetActiveFalse");
    }
    private void EnemyDead(GameObject enemyObj, string objId, bool isDrained)
    {
        if(TutorialManager.Instance.IsTutorialStage)
        {
            return;
        }

        Enemy.Enemy enemy = enemyObj.GetComponent<Enemy.Enemy>();

        if (SlimeGameManager.Instance.CurrentBodyId == "origin")
        {
            if (!isDrained)
            {
                if (PlayerEnemyUnderstandingRateManager.Instance.CheckMountObjIdContain(objId))
                {
                    PlayerEnemyUnderstandingRateManager.Instance.UpUnderstandingRate(objId, upUnderstandingRateValueWhenEnemyDead);

                    BattleUIManager.Instance.InsertAbsorptionInfo(objId, PlayerEnemyUnderstandingRateManager.Instance.GetDrainProbabilityDict(objId),
                        PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(objId), KillNoticeType.UNDERSTANDING);
                }
                else
                {
                    PlayerEnemyUnderstandingRateManager.Instance.UpDrainProbabilityDict(objId, upMountingPercentageValueWhenEnemyDead);
                }
            }
        }
        else
        {
            //Debug.Log("우오옷 동화율이 오른다앗");
            PlayerEnemyUnderstandingRateManager.Instance.UpUnderstandingRate(SlimeGameManager.Instance.CurrentBodyId, upUnderstandingRateValueWhenEnemyDeadAfterBodyChanged);

            BattleUIManager.Instance.InsertAbsorptionInfo(SlimeGameManager.Instance.CurrentBodyId, PlayerEnemyUnderstandingRateManager.Instance.GetDrainProbabilityDict(SlimeGameManager.Instance.CurrentBodyId),
                PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(SlimeGameManager.Instance.CurrentBodyId), KillNoticeType.UNDERSTANDING);
        }

        UIManager.Instance.playerStatUI.AddPlayerStatPointExp(enemy.AddExperience);
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
        playerStat.additionalEternalStat.ResetAdditional();

        playerState.IsDead = false;
        playerStat.currentHp = playerStat.MaxHp;

        UIManager.Instance.UpdatePlayerHPUI();

        SlimeGameManager.Instance.PlayerBodyChange("origin", true);

        EventManager.TriggerEvent("AfterPlayerRespawn");
    }
}
