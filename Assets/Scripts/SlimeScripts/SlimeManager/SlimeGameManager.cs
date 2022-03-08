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

    private string currentBodyId = "origin";
    public string CurrentBodyId
    {
        get { return currentBodyId; }
        set { currentBodyId = value; }
    }

    private EternalStat pasteBodyAdditionalStat = new EternalStat();

    private bool canBodyChange = true;

    [SerializeField]
    private float bodyChangeTime = 10f;
    public float BodyChangeTime
    {
        get { return bodyChangeTime; }
    }

    private float bodyChangeTimer = 0f;
    public float BodyChangeTimer
    {
        get { return bodyChangeTimer; }
    }

    private float[] currentSkillDelay = new float[3]; // 0 => 기본공격, 1 => 스킬 1, 2 => 스킬 2
    public float[] SkillDelays
    {
        get { return currentSkillDelay; }

    }

    private float[] currentSkillDelayTimer = new float[3]; //  0 => 기본공격, 1 => 스킬 1, 2 => 스킬 2
    public float[] CurrentSkillDelayTimer
    {
        get { return currentSkillDelayTimer; }
    }

    [Header("동화율 10퍼당 변신시 오르게되는 능력치가 오르게 되는 수치(배율)")]
    [SerializeField]
    private float upStatPercentage = 0.2f;
    public float UpStatPercentage
    {
        get { return upStatPercentage; }
    }

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
    private void Update()
    {
        CheckBodyTimer();
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
    public void PlayerBodyChange(string bodyId, bool isDead = false)
    {
        if(bodyId == "" || !canBodyChange)
        {
            return;
        }

        if(bodyId == currentBodyId)
        {
            Debug.Log("이미 해당 Body로 변신중입니다.");

            return;
        }

        Player player = this.player;

        Enemy.Enemy enemy = null;

        GameObject newBody = null;

        Vector2 spawnPos = currentPlayerBody.transform.position;

        float hpPercentage = player.CurrentHp / (float)player.PlayerStat.MaxHp;

        if (bodyId == "origin")
        {
            Destroy(currentPlayerBody);

            newBody = Instantiate(originPlayerBody, player.transform);
            currentBodyId = bodyId;

            if (pasteBodyAdditionalStat != null && !isDead)
            {
                player.PlayerStat.additionalEternalStat -= pasteBodyAdditionalStat;

                player.CurrentHp = (int)(player.PlayerStat.MaxHp * hpPercentage);

                pasteBodyAdditionalStat = new EternalStat();

                SetCanBodyChangeFalse();
            }

            enemy = newBody.GetComponent<Enemy.Enemy>();

            if (enemy)
            {
                enemy.EnemyControllerChange(Enemy.EnemyController.PLAYER);
            }

            newBody.transform.position = spawnPos;

            UIManager.Instance.UpdatePlayerHPUI();
            cinemachineCameraScript.SetCinemachineFollow(newBody.transform);

            EventManager.TriggerEvent("ChangeBody", bodyId, isDead);

            return;
        }

        if (playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyId) >= playerEnemyUnderstandingRateManager.MinBodyChangeUnderstandingRate)
        {
            Destroy(currentPlayerBody);

            (GameObject, EternalStat) newBodyData = playerEnemyUnderstandingRateManager.ChangalbeBodyDict[bodyId];
            currentBodyId = bodyId;

            int upNewBodyStat = ((playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyId)
            - playerEnemyUnderstandingRateManager.MinBodyChangeUnderstandingRate) / 10);

            if (upNewBodyStat >= 1) // this code is "imsi" code that inserted "imsi" values.
            {
                upNewBodyStat = (int)(upNewBodyStat * upStatPercentage); // 10% 마다 0.2배씩 상승
                newBodyData.Item2 += newBodyData.Item2 * upNewBodyStat;
            }

            newBody = Instantiate(newBodyData.Item1, player.transform);

            if (pasteBodyAdditionalStat != null && !isDead)
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

            EventManager.TriggerEvent("ChangeBody", bodyId, isDead);

            SetCanBodyChangeFalse();

            // TODO: PlayerBody로서의 처리
        }
        else
        {
            Debug.Log("Body Id: '" + bodyId + "' 로의 Body Change에 실패했습니다.");
        }
    }

    private void SetCanBodyChangeFalse()
    {
        canBodyChange = false;
        bodyChangeTimer = bodyChangeTime;
    }

    private void CheckBodyTimer()
    {
        if(bodyChangeTimer > 0f)
        {
            bodyChangeTimer -= Time.deltaTime;

            if(bodyChangeTimer <= 0f)
            {
                canBodyChange = true;
            }
        }
    }
    public Vector2 PosCantCrossWall(LayerMask wallLayer, Vector2 startPos, Vector2 targetPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, (targetPos - startPos).normalized, Vector2.Distance(startPos, targetPos), wallLayer);

        if (hit)
        {
            if (wallLayer.CompareGameObjectLayer(hit.collider.gameObject))
            {
                return hit.point - (targetPos - startPos).normalized * (Vector2.Distance(startPos, targetPos) / 10f);
            }
            else
            {
                return hit.point;
            }
        }
        else
        {
            return targetPos;
        }
    }
}
