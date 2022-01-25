using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Stat playerStat = new Stat();
    public Stat PlayerStat
    {
        get { return playerStat; }
        set { playerStat = value; }
    }
    private int currentHp = 0;
    
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
        currentHp -= (damage - playerStat.eternalStat.defense);
    }
    private void PlayerDead()
    {

    }
}
