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

        float hpPercentage = player.CurrentHp / (float)player.PlayerStat.MaxHp;


        if (bodyId == "origin")
        {
            Destroy(currentPlayerBody);

            newBody = Instantiate(originPlayerBody, player.transform);

            if(pasteBodyAdditionalStat != null)
            {
                player.PlayerStat.additionalEternalStat -= pasteBodyAdditionalStat;

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

            EventManager.TriggerEvent("ChangeBody");

            return;
        }

        if (playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyId) >= playerEnemyUnderstandingRateManager.MinBodyChangeUnderstandingRate)
        {
            Destroy(currentPlayerBody);
            
            (GameObject, EternalStat) newBodyData = playerEnemyUnderstandingRateManager.ChangalbeBodyDict[bodyId];

            float upNewBodyStat = ((playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyId) 
            - playerEnemyUnderstandingRateManager.MinBodyChangeUnderstandingRate) / 10f);

            if(upNewBodyStat >= 1f)
            {
                newBodyData.Item2 *= upNewBodyStat; // 이해도가 변신가능 이해도보다 20 높으면 상승하는 능력치가 2배가 됌
            }

            newBody = Instantiate(newBodyData.Item1, player.transform);

            if(pasteBodyAdditionalStat != null)
            {
                player.PlayerStat.additionalEternalStat -= pasteBodyAdditionalStat;

                pasteBodyAdditionalStat = new EternalStat();
            }

            pasteBodyAdditionalStat = newBodyData.Item2;

            player.PlayerStat.additionalEternalStat += newBodyData.Item2;
            Debug.Log(hpPercentage);
            player.CurrentHp = (int)(player.PlayerStat.MaxHp * hpPercentage); 

            newBody.AddComponent<PlayerBodyScript>();

            enemy = newBody.GetComponent<Enemy.Enemy>();

            if (enemy)
            {
                enemy.EnemyControllerChange(Enemy.EnemyController.PLAYER);
            }

            newBody.transform.position = spawnPos;

            UIManager.Instance.UpdatePlayerHPUI();
            cinemachineCameraScript.SetCinemachineFollow(newBody.transform);
            playerEnemyUnderstandingRateManager.SetUnderstandingRate(bodyId, 0);

            EventManager.TriggerEvent("ChangeBody");

            // TODO: PlayerBody로서의 처리
        }
        else
        {
            Debug.Log("Body Id: '" + bodyId + "' 로의 Body Change에 실패했습니다.");
        }
    }
}
