using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;

public static partial class ScriptHelper
{
    
}

public class SlimeGameManager : MonoSingleton<SlimeGameManager>
{
    private Player player = null;
    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindObjectOfType<Player>();

                if (player == null)
                {
                    Debug.LogError("There is no playerStat!");
                }
            }

            return player;
        }
    }

    private void Start()
    {
        EventManager.StartListening("PlayerRespawn", PlayerRespawn);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerRespawn", PlayerRespawn);
    }

    private void PlayerRespawn(Vector2 respawnPosition)
    {
        player.transform.position = respawnPosition;

        player.gameObject.SetActive(true);

        player.WhenRespawn();
    }

}
