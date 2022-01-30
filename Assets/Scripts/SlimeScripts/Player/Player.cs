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

        UIManager.Instance.UpdatePlayerHPUI();

    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", PlayerDead);
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

            UIManager.Instance.UpdatePlayerHPUI();
        }
    }
    private void WhenGameClear()
    {

    }
    private void PlayerDead()
    {
        EventManager.TriggerEvent("PlayerSetActiveFalse");
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

        UIManager.Instance.UpdatePlayerHPUI();

        EventManager.TriggerEvent("AfterPlayerRespawn");
    }
}
