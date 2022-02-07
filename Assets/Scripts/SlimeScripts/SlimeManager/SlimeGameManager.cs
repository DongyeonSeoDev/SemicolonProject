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
    private GameObject currentPlayerBody = null;
    public GameObject CurrentPlayerBody
    {
        get { return currentPlayerBody; }
        set { currentPlayerBody = value; }
    }

    private void Start()
    {
        EventManager.StartListening("PlayerRespawn", PlayerSpawn);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerRespawn", PlayerSpawn);
    }

    private void PlayerSpawn(Vector2 spawnPosition)
    {
        player.transform.position = spawnPosition;

        player.gameObject.SetActive(true);

        player.WhenRespawn();
    }
}
