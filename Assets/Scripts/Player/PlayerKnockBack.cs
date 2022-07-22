using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsCantCrossLayer;

    private Vector2 originPos = Vector2.zero;
    private Vector2 targetPos = Vector2.zero;

    private float knockBackTime = 0f;
    private float knocBackTimer = 0f;

    private void OnEnable()
    {
        EventManager.StartListening("PlayerKnockBack", OnKnockBack);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerKnockBack", OnKnockBack);
    }

    void Update()
    {
        CheckKnockBackTimer();
    }
    private void CheckKnockBackTimer()
    {
        if (knocBackTimer < knockBackTime)
        {
            knocBackTimer += Time.deltaTime;

            if(knocBackTimer >= knockBackTime)
            {
                knocBackTimer = knockBackTime;

                EventManager.TriggerEvent("KnockBackDone");

                return;
            }

            DoKnockBack();
        }
    }
    private void DoKnockBack()
    {
         SlimeGameManager.Instance.CurrentPlayerBody.transform.position = Vector2.Lerp(originPos, targetPos, knocBackTimer / knockBackTime);
    }
    private void OnKnockBack(Vector2 direction, float moveDistance, float moveTime)
    {
        if(SlimeGameManager.Instance.Player.PlayerState.IsDrain || SlimeGameManager.Instance.Player.PlayerState.IsDead || SlimeGameManager.Instance.GameClear)
        {
            return;
        }

        originPos = SlimeGameManager.Instance.CurrentPlayerBody.transform.position;

        targetPos = (direction * moveDistance) + originPos;
        targetPos = SlimeGameManager.Instance.PosCantCrossWall(whatIsCantCrossLayer, originPos, targetPos);

        knockBackTime = moveTime;
        knocBackTimer = 0f;
    }
}
