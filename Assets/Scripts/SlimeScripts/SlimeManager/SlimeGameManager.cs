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
    private PlayerEnemyUnderstandingRateManager playerEnemyUnderstandingRateManager = null;
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
                    Debug.LogError("There is no player!");
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
    private void Awake() 
    {
        playerEnemyUnderstandingRateManager = PlayerEnemyUnderstandingRateManager.Instance;
    }
    private void Start()
    {
        EventManager.StartListening("PlayerRespawn", PlayerBodySpawn);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerRespawn", PlayerBodySpawn);
    }

    private void PlayerBodySpawn(Vector2 spawnPosition)
    {
        player.gameObject.SetActive(true);
        currentPlayerBody.SetActive(true);

        currentPlayerBody.transform.position = spawnPosition;

        player.WhenRespawn();
    }
    public void PlayerBodyDespawn()
    {
        currentPlayerBody.SetActive(false);
    }
    public void PlayerBodyChange(string bodyId)
    {
        if(playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyId) >= 100f)
        {
            Destroy(currentPlayerBody);

            GameObject newBody = Instantiate(playerEnemyUnderstandingRateManager.ChangalbeBodyDict[bodyId], player.transform);

            newBody.transform.position = currentPlayerBody.transform.position;

            newBody.AddComponent<PlayerBodyScript>();

            // TODO: PlayerBody로서의 처리
        }
        else
        {
            Debug.Log("Body Id: '" + bodyId + "' 로의 Body Change에 실패했습니다.");
        }
    }
}
