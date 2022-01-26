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
    private int currentHp = 0;
    public int CurrentHp
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    private void Start()
    {
        SlimeEventManager.StartListening("PlayerDead", PlayerDead);
        SlimeEventManager.StartListening("PlayerSetActiveFalse", SetActiveFalse);

        playerState = GetComponent<PlayerState>();
    }
    private void OnEnable()
    {
        currentHp = playerStat.eternalStat.hp;
    }
    private void Update()
    {
        if (playerStat.Hp <= 0)
        {
            SlimeEventManager.TriggerEvent("PlayerDead");
        }
    }
    private void OnDisable()
    {
        SlimeEventManager.StopListening("PlayerDead", PlayerDead);
        SlimeEventManager.StopListening("PlayerSetActiveFalse", SetActiveFalse);
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

    private void PlayerDead()
    {
        SlimeEventManager.TriggerEvent("PlayerSetActiveFalse");
    }
    private void SetActiveFalse()
    {
        playerState.IsDead = false;

        gameObject.SetActive(false);
    }
}
