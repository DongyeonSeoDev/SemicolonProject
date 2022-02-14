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
    private CinemachineCameraScript cinemachineCameraScript = null;
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
    private GameObject originPlayerBody = null;
    private GameObject currentPlayerBody = null;
    public GameObject CurrentPlayerBody
    {
        get { return currentPlayerBody; }
        set { currentPlayerBody = value; }
    }
    private EternalStat pasteBodyAdditionalStat = new EternalStat();
    private void Awake()
    {
        playerEnemyUnderstandingRateManager = PlayerEnemyUnderstandingRateManager.Instance;

        originPlayerBody = Resources.Load<GameObject>("Player/DefaultPlayer/Player").GetComponentInChildren<PlayerBodyScript>().gameObject;
    }
    private void Start()
    {
        EventManager.StartListening("PlayerRespawn", PlayerBodySpawn);

        cinemachineCameraScript = FindObjectOfType<CinemachineCameraScript>();
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
        Player player = SlimeGameManager.Instance.player;

        Enemy.Enemy enemy = null;

        GameObject newBody = null;

        Vector2 spawnPos = currentPlayerBody.transform.position;

        Destroy(currentPlayerBody);

        if (bodyId == "origin")
        {
            newBody = Instantiate(originPlayerBody, player.transform);

            if(pasteBodyAdditionalStat != null)
            {
                player.PlayerStat.additionalEternalStat -= pasteBodyAdditionalStat;
                player.CurrentHp = player.PlayerStat.MaxHp; 

                pasteBodyAdditionalStat = new EternalStat();
            }

            enemy = newBody.GetComponent<Enemy.Enemy>();

            if (enemy)
            {
                enemy.EnemyControllerChange(Enemy.EnemyController.PLAYER);
            }

            newBody.transform.position = spawnPos;

            UIManager.Instance.UpdatePlayerHPUI();

            cinemachineCameraScript.SetCinemachineFollow(newBody.transform);

            return;
        }

        if (playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyId) >= 100f)
        {
            (GameObject, EternalStat) newBodyData = playerEnemyUnderstandingRateManager.ChangalbeBodyDict[bodyId];

            newBody = Instantiate(newBodyData.Item1, player.transform);

            pasteBodyAdditionalStat = newBodyData.Item2;

            player.PlayerStat.additionalEternalStat += newBodyData.Item2;
            player.CurrentHp = player.PlayerStat.MaxHp;

            newBody.AddComponent<PlayerBodyScript>();

            enemy = newBody.GetComponent<Enemy.Enemy>();

            if (enemy)
            {
                enemy.EnemyControllerChange(Enemy.EnemyController.PLAYER);
            }

            newBody.transform.position = spawnPos;

            UIManager.Instance.UpdatePlayerHPUI();

            cinemachineCameraScript.SetCinemachineFollow(newBody.transform);

            // TODO: PlayerBody로서의 처리
        }
        else
        {
            Debug.Log("Body Id: '" + bodyId + "' 로의 Body Change에 실패했습니다.");
        }
    }
}
