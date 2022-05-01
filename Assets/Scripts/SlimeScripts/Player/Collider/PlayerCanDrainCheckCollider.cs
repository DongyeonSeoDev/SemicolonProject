using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanDrainCheckCollider : MonoBehaviour
{
    private PlayerDrain playerDrain = null;

    private BoxCollider2D playerDrainCollider = null;
    private BoxCollider2D boxCol2D = null;

    void Start()
    {
        if(SlimeGameManager.Instance.CurrentBodyId != "origin")
        {
            Debug.LogError("Current Body is not Slime!!!");

            enabled = false;

            return;
        }

        boxCol2D = GetComponent<BoxCollider2D>();

        if(boxCol2D == null)
        {
            boxCol2D = gameObject.AddComponent<BoxCollider2D>();
        }

        playerDrain = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerDrain>();
        playerDrainCollider = playerDrain.PlayerDrainCol.GetComponent<BoxCollider2D>();

        if(playerDrainCollider == null)
        {
            playerDrainCollider = Instantiate(
                Resources.Load<GameObject>("Player/PlayerCollider/DrainCollider"), playerDrain.transform)
                .GetComponent<BoxCollider2D>();
        }

        boxCol2D.offset = playerDrainCollider.offset;
        boxCol2D.size = playerDrainCollider.size / 1.2f;
        boxCol2D.isTrigger = true;
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
            Debug.Log(enemy.EnemyHpPercent());
            EventManager.TriggerEvent("Tuto_CanDrainObject");
        }
    }
}
