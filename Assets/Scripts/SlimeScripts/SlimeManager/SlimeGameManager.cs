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
        get { return currentSkillDelay; } // 이 배열의 타이머를 Set해줄때는 반드시 SetSkillDelay함수를 활용할 것. 여기에 직접 넣게되면 SkillSpeed가 적용되지 않음.
    }

    private float[] currentSkillDelayTimer = new float[3]; //  0 => 기본공격, 1 => 스킬 1, 2 => 스킬 2
    public float[] CurrentSkillDelayTimer
    {
        get { return currentSkillDelayTimer; } 
    }

    [Header("동화율 몇퍼당 변신시 능력치가 오를지를 정하는 변수")]
    [SerializeField]
    private int understadingRatePercentageWhenUpStat = 10;
    [Header("동화율 'understadingRatePercentageWhenUpStat'퍼당 변신시 오르게되는 능력치가 오르게 되는 수치(배율)")]
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
        CheckSkillTimer();
    }

    private void PlayerBodySpawn()
    {
        player.gameObject.SetActive(true);
        currentPlayerBody.SetActive(true);

        //currentPlayerBody.transform.position = spawnPosition;
        pasteBodyAdditionalStat = new EternalStat();

        player.WhenRespawn();
    }
    public void PlayerBodyDespawn()
    {
        currentPlayerBody.SetActive(false);
    }
    public void PlayerBodyChange(string bodyId, bool isDead = false)
    {
        if (!isDead)
        {
            if (bodyId == "" || !canBodyChange)
            {
                return;
            }

            if (bodyId == currentBodyId)
            {
                Debug.Log("이미 해당 Body로 변신중입니다.");

                return;
            }
        }

        Enemy.Enemy enemy = null;

        GameObject newBody = null;

        Vector2 spawnPos = currentPlayerBody.transform.position;

        float hpPercentage = player.CurrentHp / player.PlayerStat.MaxHp;

        //Destroy(currentPlayerBody);

        Debug.Log(currentPlayerBody.name);
        SlimePoolManager.Instance.AddObject(currentPlayerBody);
        currentPlayerBody.SetActive(false);

        bool found = false;

        #region 원래의 모습으로 변신
        if (bodyId == "origin")
        {
            (newBody, found) = SlimePoolManager.Instance.Find(originPlayerBody, false);

            if (!found)
            {
                newBody = Instantiate(originPlayerBody, player.transform);
            }
            else
            {
                newBody.SetActive(true);
            }

            currentBodyId = bodyId;

            if (pasteBodyAdditionalStat != null && !isDead)
            {
                player.PlayerStat.additionalEternalStat -= pasteBodyAdditionalStat;

                //player.CurrentHp = (player.PlayerStat.MaxHp * hpPercentage).Round();
                player.CurrentHp = player.PlayerStat.MaxHp * hpPercentage;

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

            EventManager.TriggerEvent("ChangeBody");
            EventManager.TriggerEvent("ChangeBody", bodyId, isDead);

            return;
        }
        #endregion
        #region 원래와는 다른 모습으로 변신
        if (playerEnemyUnderstandingRateManager.MountedObjList.Contains(bodyId))
        {
            (GameObject, EternalStat) newBodyData = playerEnemyUnderstandingRateManager.ChangalbeBodyDict[bodyId];
            currentBodyId = bodyId;

            (newBody, found) = SlimePoolManager.Instance.Find(newBodyData.Item1);

            if (!found)
            {
                newBody = Instantiate(newBodyData.Item1, player.transform);
            }
            else
            {
                newBody.SetActive(true);
            }

            //newBody = Instantiate(newBodyData.Item1, player.transform);

            if (pasteBodyAdditionalStat != null && !isDead)
            {
                player.PlayerStat.additionalEternalStat -= pasteBodyAdditionalStat;

                pasteBodyAdditionalStat = new EternalStat();
            }

            pasteBodyAdditionalStat = newBodyData.Item2 + GetExtraUpStat(bodyId);

            player.PlayerStat.additionalEternalStat += pasteBodyAdditionalStat;

            //player.CurrentHp = (player.PlayerStat.MaxHp * hpPercentage).Round();
            player.CurrentHp = player.PlayerStat.MaxHp * hpPercentage;

            newBody.AddComponent<PlayerBodyScript>();

            enemy = newBody.GetComponent<Enemy.Enemy>();

            if (enemy)
            {
                enemy.EnemyControllerChange(Enemy.EnemyController.PLAYER);
            }

            newBody.transform.position = spawnPos;

            UIManager.Instance.UpdatePlayerHPUI();
            cinemachineCameraScript.SetCinemachineFollow(newBody.transform);

            EventManager.TriggerEvent("ChangeBody");
            EventManager.TriggerEvent("ChangeBody", bodyId, isDead);

            SetCanBodyChangeFalse();

            // TODO: PlayerBody로서의 처리
        }
        else
        {
            Debug.Log("Body Id: '" + bodyId + "' 로의 Body Change에 실패했습니다.");
        }
        #endregion
    }
    public EternalStat GetExtraUpStat(string objId)
    {
        EternalStat result = new EternalStat();
        EternalStat upStat = playerEnemyUnderstandingRateManager.ChangalbeBodyDict[objId].Item2;

        if (PlayerEnemyUnderstandingRateManager.Instance.ChangalbeBodyDict.ContainsKey(objId))
        {
            int upNewBodyStat = (playerEnemyUnderstandingRateManager.GetUnderstandingRate(objId) / understadingRatePercentageWhenUpStat);

            if (upNewBodyStat >= 1) // this code is "imsi" code that inserted "imsi" values.
            {
                result = (upStat * upNewBodyStat * upStatPercentage).Abs();// 10% 마다 upStatPercentage배씩 상승
            }
        }
        else
        {
            Debug.LogWarning(objId + " is not on the changableBodyList!");
        }

        return result;
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
    private void CheckSkillTimer()
    {
        for(int i = 0; i < currentSkillDelayTimer.Length; i++)
        {
            if(currentSkillDelayTimer[i] > 0f)
            {
                currentSkillDelayTimer[i] -= Time.deltaTime;

                if(currentSkillDelayTimer[i] <= 0f)
                {
                    currentSkillDelayTimer[i] = 0f;

                    EventManager.TriggerEvent("Skill" + i + "DelayTimerZero");
                }
            }
        }
    }
    public void SetSkillDelay(int skillIdx, float delayTime)
    {
        try
        {
            if (skillIdx == 0) // skillIdx가 0이면 그거슨 기본공격
            {
                currentSkillDelay[skillIdx] = delayTime / player.PlayerStat.AttackSpeed;
            }
            else
            {
                currentSkillDelay[skillIdx] = delayTime;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    public Vector2 PosCantCrossWall(LayerMask crashableLayer, Vector2 startPos, Vector2 targetPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, (targetPos - startPos).normalized, Vector2.Distance(startPos, targetPos), crashableLayer);

        if (hit)
        {
            return hit.point;
        }
        else
        {
            return targetPos;
        }
    }
}
