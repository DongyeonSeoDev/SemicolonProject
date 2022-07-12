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

    public Dictionary<GameObject, bool> playerHitCheckDict = new Dictionary<GameObject, bool>();

    /// <summary>
    /// 이 코드는 사용하면 위험할 수 있다. Property인 Player를 사용하자
    /// </summary>
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

    private bool gameClear = false;
    public bool GameClear
    {
        get { return gameClear; }
        set { gameClear = value; }
    }

    private string currentBodyId = "origin";
    public string CurrentBodyId
    {
        get { return currentBodyId; }
        set { currentBodyId = value; }
    }

    private EternalStat prevBodyAdditionalStat = new EternalStat();
    // pasteExtraStat

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

    private bool[] skillDelayTimerZero = new bool[3] { false, false, false};
    public bool[] SkillDelayTimerZero
    {
        get { return skillDelayTimerZero; }
    }

    private void Awake()
    {
        playerEnemyUnderstandingRateManager = PlayerEnemyUnderstandingRateManager.Instance;

        originPlayerBody = Resources.Load<GameObject>("Player/DefaultPlayer/Player").GetComponentInChildren<PlayerBodyScript>().gameObject;
    }
    private void Start()
    {
        EventManager.StartListening("PlayerRespawn", PlayerBodySpawn);
        EventManager.StartListening("GameClear", WhenGameClear);

        cinemachineCameraScript = FindObjectOfType<CinemachineCameraScript>();
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerRespawn", PlayerBodySpawn);
        EventManager.StopListening("GameClear", WhenGameClear);
    }
    private void Update()
    {
        CheckBodyTimer();
        CheckSkillTimer();
    }
    private void WhenGameClear()
    {
        gameClear = true;
    }
    private void PlayerBodySpawn()
    {
        bodyChangeTimer = 0f;
        canBodyChange = true;

        for(int i = 0; i < 3; i++)
        {
            currentSkillDelayTimer[i] = 0f;

            skillDelayTimerZero[i] = true;
        }

        Player.gameObject.SetActive(true);

        if (currentPlayerBody != null)
        {
            currentPlayerBody.SetActive(true);
        }

        //currentPlayerBody.transform.position = spawnPosition;
        prevBodyAdditionalStat = new EternalStat();

        Player.WhenRespawn();
    }
    public void PlayerBodyDespawn()
    {
        currentPlayerBody.SetActive(false);
    }
    public void PlayerBodyChange(string bodyId, bool isDead = false)
    {
        #region 예외처리
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

        if (player.PlayerState.IsDrain || player.PlayerState.IsStun)
        {
            return;
        }
        #endregion

        Enemy.Enemy enemy = null;

        GameObject newBody = null;

        Vector2 spawnPos = currentPlayerBody.transform.position;

        float hpPercentage = Player.PlayerStat.currentHp / Player.PlayerStat.MaxHp;

        //Destroy(currentPlayerBody);

        Debug.Log(currentPlayerBody.name);
        SlimePoolManager.Instance.AddObject(currentPlayerBody);
        currentPlayerBody.SetActive(false);
        currentPlayerBody.tag = "Untagged";

        EventManager.TriggerEvent("EnemyStart");
        EventManager.TriggerEvent("PlayerStart");

        bool found = false;

        if(prevBodyAdditionalStat != null)
        {
            Debug.Log(prevBodyAdditionalStat.minDamage.statValue);
        }

        #region 원래의 모습으로 변신
        if (bodyId == "origin")
        {
            if (prevBodyAdditionalStat != null && !isDead)
            {
                Player.PlayerStat.additionalEternalStat.Sub(prevBodyAdditionalStat);

                //player.CurrentHp = (player.PlayerStat.MaxHp * hpPercentage).Round();
                Player.PlayerStat.currentHp = Player.PlayerStat.MaxHp * hpPercentage;

                prevBodyAdditionalStat = new EternalStat();

                SetCanBodyChangeFalse();
            }

            (newBody, found) = SlimePoolManager.Instance.Find(originPlayerBody, false);

            if (!found)
            {
                newBody = Instantiate(originPlayerBody, Player.transform);
            }
            else
            {
                newBody.SetActive(true);
                newBody.tag = "Player";
            }

            Enemy.EnemyManager.Player = newBody;
            currentBodyId = bodyId;

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

            if (prevBodyAdditionalStat != null && !isDead)
            {
                Player.PlayerStat.additionalEternalStat.Sub(prevBodyAdditionalStat);

                prevBodyAdditionalStat = new EternalStat();
            }

            prevBodyAdditionalStat = newBodyData.Item2;

            Player.PlayerStat.additionalEternalStat.Sum(prevBodyAdditionalStat);

            //player.CurrentHp = (player.PlayerStat.MaxHp * hpPercentage).Round();
            Player.PlayerStat.currentHp = Player.PlayerStat.MaxHp * hpPercentage;

            (newBody, found) = SlimePoolManager.Instance.Find(newBodyData.Item1);

            if (!found)
            {
                newBody = Instantiate(newBodyData.Item1, Player.transform);
            }
            else
            {
                newBody.SetActive(true);
            }

            Enemy.EnemyManager.Player = newBody;

            //newBody = Instantiate(newBodyData.Item1, player.transform);

            newBody.AddComponent<PlayerBodyScript>();

            enemy = newBody.GetComponent<Enemy.Enemy>();

            if (enemy)
            {
                enemy.ChangeToPlayerController();
            }

            newBody.transform.position = spawnPos;

            UIManager.Instance.UpdatePlayerHPUI();
            cinemachineCameraScript.SetCinemachineFollow(newBody.transform);

            EventManager.TriggerEvent("ChangeBody");
            EventManager.TriggerEvent("ChangeBody", bodyId, isDead);

            //for(int i = 0; i < currentSkillDelayTimer.Length; i++) // 스킬 갱신
            //{
            //    if(currentSkillDelayTimer[i] <= 0f)
            //    {
            //        //EventManager.TriggerEvent("Skill" + i + "DelayTimerZero");
            //        skillDelayTimerZero[i]
            //    }
            //}

            SetCanBodyChangeFalse();
        }
        else
        {
            Debug.Log("Body Id: '" + bodyId + "' 로의 Body Change에 실패했습니다.");
        }
        #endregion

        EventManager.TriggerEvent("OnAttackSpeedChage");
    }

    private void SetCanBodyChangeFalse()
    {
        canBodyChange = false;
        bodyChangeTimer = bodyChangeTime;
    }

    private void CheckBodyTimer()
    {
        if (Player.PlayerState.IsDrain)
        {
            return;
        }

        if (bodyChangeTimer > 0f)
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
        if(Player.PlayerState.IsDrain)
        {
            return;
        }

        for(int i = 0; i < currentSkillDelayTimer.Length; i++)
        {
            if(currentSkillDelayTimer[i] > 0f)
            {
                currentSkillDelayTimer[i] -= Time.deltaTime;

                if(currentSkillDelayTimer[i] <= 0f)
                {
                    currentSkillDelayTimer[i] = 0f;

                    skillDelayTimerZero[i] = true;
                }
            }
        }
    }
    public void SetSkillDelay(int skillIdx, float delayTime)
    {
        try
        {
            currentSkillDelay[skillIdx] = delayTime / Player.PlayerStat.AttackSpeed;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void AddSkillDelay(int skillIdx, float delayTime)
    {
        try
        {
            currentSkillDelay[skillIdx] += delayTime;
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
            return hit.point - hit.point / 20f;
        }
        else
        {
            return targetPos;
        }
    }
}