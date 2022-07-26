using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class PlayerStatusEffect : PlayerAction
{
    private StunEffect currentStunEffect = null;

    [SerializeField] private Vector2 stunPosition;

    private float sturnTimer = 0f;

   public override void Awake()
   {
       base.Awake();
   }
    private void OnEnable()
    {
        EventManager.StartListening("KnockBackDone", OnKnockBackDone);
        EventManager.StartListening("PlayerDead", DeleteEffect);
    }
    private void OnDisable()
    {
        EventManager.StopListening("KnockBackDone", OnKnockBackDone);
        EventManager.StopListening("PlayerDead", DeleteEffect);
    }
    void Start()
    {
        playerState = SlimeGameManager.Instance.Player.GetComponent<PlayerState>();
    }
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

        if (currentStunEffect == null)
        {
            currentStunEffect = EnemyPoolManager.Instance.GetPoolObject(Type.StunEffect, stunPosition).GetComponent<StunEffect>();
            currentStunEffect.transform.SetParent(SlimeGameManager.Instance.CurrentPlayerBody.transform, false);
        }
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

                DeleteEffect();
            }
        }
    }

    private void DeleteEffect()
    {
        if (currentStunEffect != null)
        {
            currentStunEffect.gameObject.SetActive(false);
            currentStunEffect.transform.SetParent(EnemyPoolManager.Instance.transform, false);
            currentStunEffect = null;
        }
    }
}
