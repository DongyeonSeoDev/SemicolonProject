using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerState playerState = null;

    [SerializeField]
    private Stat playerStat = new Stat();
    private Stat originStat = new Stat();
    public Stat PlayerStat
    {
        get { return playerStat; }
        set { playerStat = value; }
    }

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

    private int currentHp = 0;
    public int CurrentHp
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();

        // originStat = PlayerStat;
    }
    private void Start()
    {
        playerStat.additionalEternalStat = new EternalStat();

        playerState.IsDead = false;
        currentHp = playerStat.MaxHp;

        UIManager.Instance.UpdatePlayerHPUI();

    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", PlayerDead);
        EventManager.StartListening("EnemyDead", EnemyDead);
        EventManager.StartListening("PlayerSetActiveFalse", SetActiveFalse);
        EventManager.StartListening("GameClear", WhenGameClear);

        // playerStat = originStat;
        playerState.IsDead = false;
    }
    private void Update()
    {
        if (currentHp <= 0)
        {
            EventManager.TriggerEvent("PlayerDead");
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", PlayerDead);
        EventManager.StopListening("EnemyDead", EnemyDead);
        EventManager.StopListening("PlayerSetActiveFalse", SetActiveFalse);
        EventManager.StopListening("GameClear", WhenGameClear);
    }
    public void GetDamage(int damage)
    {
        if (!playerState.IsDead)
        {
            int dm = damage - playerStat.Defense;

            if (dm <= 0)
            {
                dm = 0;
            }

            currentHp -= dm;

            if (currentHp <= 0)
            {
                playerState.IsDead = true;
            }

            EffectManager.Instance.OnDamaged(dm, false,false, SlimeGameManager.Instance.CurrentPlayerBody.transform.position); 
            UIManager.Instance.UpdatePlayerHPUI(true);
        }
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
        PlayerEnemyUnderstandingRateManager.Instance.SetMountingPercentageDict(objId, PlayerEnemyUnderstandingRateManager.Instance.GetMountingPercentageDict(objId) + upMountingPercentageValueWhenEnemyDead);

        if (PlayerEnemyUnderstandingRateManager.Instance.CheckMountObjIdContain(objId))
        {
            PlayerEnemyUnderstandingRateManager.Instance.UpUnderStandingRate(objId, upUnderstandingRateValueWhenEnemyDead);
        }
        else
        {
            if (PlayerEnemyUnderstandingRateManager.Instance.CheckMountingEnemy(objId))
            {
                PlayerEnemyUnderstandingRateManager.Instance.UpUnderStandingRate(objId, upUnderstandingRateValueWhenEnemyDead);
            }
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

        SlimeGameManager.Instance.PlayerBodyChange("origin");

        EventManager.TriggerEvent("AfterPlayerRespawn");
    }
}
