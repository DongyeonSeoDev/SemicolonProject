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
    void Start()
    {
        playerState = SlimeGameManager.Instance.Player.GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSturnTimer();
    }
    
    public void KnockBack(Vector2 direction, float speed, float knockBackTime)
    {
        rigid.AddForce(direction.normalized * speed, ForceMode2D.Impulse);

        childRigids.ForEach(x => x.AddForce(direction.normalized * speed, ForceMode2D.Impulse));

        playerState.IsKnockBack = true;

        EventManager.TriggerEvent("PlayerKnockBack");
    }
    public void KnockBack(Vector2 direction, float speed, float knockBackTime, float sturnTime)
    {
        rigid.AddForce(direction.normalized * speed, ForceMode2D.Impulse);

        childRigids.ForEach(x => x.AddForce(direction.normalized * speed, ForceMode2D.Impulse));

        playerState.IsKnockBack = true;

        EventManager.TriggerEvent("PlayerKnockBack");

        Sturn(sturnTime);
    }
    public void Sturn(float sturnTime)
    {
        sturnTimer = sturnTime;

        playerState.IsSturn = true;

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
                playerState.IsSturn = false;
            }
        }
    }
}
