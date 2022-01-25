using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    private void Start()
    {
        SlimeEventManager.StartListening("PlayerDead", PlayerDead);
    }
    private void OnEnable()
    {
        currentHp = playerStat.eternalStat.hp;
    }
    private void Update()
    {
        if (playerStat.eternalStat.hp <= 0)
        {
            SlimeEventManager.TriggerEvent("PlayerDead");
        }
    }
    private void OnDisable()
    {
        SlimeEventManager.StopListening("PlayerDead", PlayerDead);
    }
    public void GetDamage(int damage)
    {
        if (!isDead)
        {
            int dm = damage - playerStat.Defense;

            if (dm <= 0)
            {
                dm = 0;
            }

            currentHp -= dm;

            if (currentHp <= 0)
            {
                isDead = true;
            }

            Water.UIManager.Instance.UpdatePlayerHPUI();
        }
    }

    private void PlayerDead()
    {

    }
}
