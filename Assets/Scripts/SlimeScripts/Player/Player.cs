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
        currentHp = playerStat.Hp;

        Water.UIManager.Instance.UpdatePlayerHPUI();

    }
    private void OnEnable()
    {
        SlimeEventManager.StartListening("PlayerDead", PlayerDead);
        SlimeEventManager.StartListening("PlayerSetActiveFalse", SetActiveFalse);
        SlimeEventManager.StartListening("GameClear", WhenGameClear);

        // playerStat = originStat;
        playerState.IsDead = false;
    }
    private void Update()
    {
        if (currentHp <= 0)
        {
            SlimeEventManager.TriggerEvent("PlayerDead");
        }
    }
    private void OnDisable()
    {
        SlimeEventManager.StopListening("PlayerDead", PlayerDead);
        SlimeEventManager.StopListening("PlayerSetActiveFalse", SetActiveFalse);
        SlimeEventManager.StopListening("GameClear", WhenGameClear);
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

            Water.UIManager.Instance.UpdatePlayerHPUI();
        }
    }
    private void WhenGameClear()
    {

    }
    private void PlayerDead()
    {
        SlimeEventManager.TriggerEvent("PlayerSetActiveFalse");
    }
    private void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
    public void WhenRespawn()
    {
        playerStat.additionalEternalStat = new EternalStat();

        playerState.IsDead = false;
        currentHp = playerStat.Hp;

        Water.UIManager.Instance.UpdatePlayerHPUI();

        SlimeEventManager.TriggerEvent("AfterPlayerRespawn");
    }
}
