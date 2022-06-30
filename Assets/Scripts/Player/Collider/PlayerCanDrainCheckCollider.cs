using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanDrainCheckCollider : MonoBehaviour
{
    private PlayerState playerState = null;
    private PlayerDrain playerDrain = null;

    private BoxCollider2D playerDrainCollider = null;
    private CircleCollider2D cirCol2D = null;

    void Start()
    {
        if(SlimeGameManager.Instance.CurrentBodyId != "origin")
        {
            Debug.LogError("Current Body is not Slime!!!");

            enabled = false;

            return;
        }

        cirCol2D = GetComponent<CircleCollider2D>();

        if(cirCol2D == null)
        {
            cirCol2D = gameObject.AddComponent<CircleCollider2D>();
        }

        playerDrain = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerDrain>();
        playerState = SlimeGameManager.Instance.Player.GetComponent<PlayerState>();

        playerDrainCollider = playerDrain.PlayerDrainCol.GetComponent<BoxCollider2D>();

        if(playerDrainCollider == null)
        {
            playerDrainCollider = Instantiate(
                Resources.Load<GameObject>("Player/PlayerCollider/DrainCollider"), playerDrain.transform)
                .GetComponent<BoxCollider2D>();
        }

        cirCol2D.offset = playerDrainCollider.offset;
        cirCol2D.radius = (playerDrainCollider.size.y > playerDrainCollider.size.x ? playerDrainCollider.size.x : playerDrainCollider.size.y) / 2f;
        cirCol2D.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (SlimeGameManager.Instance.Player.PlayerState.IsDrain)
        {
            return;
        }

        ICanGetDamagableEnemy enemy = collision.GetComponent<ICanGetDamagableEnemy>();

        if(enemy != null && enemy.EnemyHpPercent() > 0 && enemy.EnemyHpPercent() <= playerDrain.PlayerDrainCol.CanDrainHpPercentage)
        {
            EventManager.TriggerEvent("PlayerStop");
            EventManager.TriggerEvent("Tuto_CanDrainObject");

            playerState.CantChangeDir = true;
            //playerDrain.drainTutorialDone = true;
        }
    }
}
