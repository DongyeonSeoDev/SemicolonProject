using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffect : PlayerAction
{
    private float sturnTimer = 0f;

   public override void Awake()
   {
       base.Awake();
   }
    private void OnEnable()
    {
        EventManager.StartListening("KnockBackDone", OnKnockBackDone);
    }
    private void OnDisable()
    {
        EventManager.StopListening("KnockBackDone", OnKnockBackDone);
    }
    void Start()
    {
        playerState = SlimeGameManager.Instance.Player.GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSturnTimer();
    }
    
    public void KnockBack(Vector2 direction, float speed, float knockBackTime) // knockBackTime은 knockBack되는 시간이다.
    {
        if(playerState.IsDrain)
        {
            return;
        }

        float moveDistance = speed * knockBackTime;

        playerState.IsKnockBack = true;

        EventManager.TriggerEvent("PlayerKnockBack", direction, moveDistance, knockBackTime);
    }
    public void KnockBack(Vector2 direction, float speed, float knockBackTime, float sturnTime) // knockBackTime은 knockBack되는 시간이다.
    {
        if (playerState.IsDrain)
        {
            return;
        }

        float moveDistance = speed * knockBackTime;

        playerState.IsKnockBack = true;

        EventManager.TriggerEvent("PlayerKnockBack", direction, moveDistance, knockBackTime);

        Sturn(sturnTime);
    }
    private void OnKnockBackDone()
    {
        playerState.IsKnockBack = false;
    }
    public void Sturn(float sturnTime)
    {
        if(playerState.IsDrain)
        {
            return;
        }

        sturnTimer = sturnTime;

        playerState.IsStun = true;

        EventManager.TriggerEvent("PlayerSturn");
    }
    private void CheckSturnTimer()
    {
        if (sturnTimer > 0f)
        {
            sturnTimer -= Time.deltaTime;

            if (sturnTimer <= 0f)
            {
                sturnTimer = 0f;
                playerState.IsStun = false;
            }
        }
    }
}
