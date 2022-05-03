using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrain : PlayerSkill
{
    private readonly string canDrainCheckColliderPath = "Player/PlayerCollider/CanDrainCheckCollider";

    [SerializeField]
    private GameObject drainCollider = null; // drain 체크에 사용될 Collider
    private PlayerDrainCollider playerDrainCol = null;
    public PlayerDrainCollider PlayerDrainCol
    {
        get { return playerDrainCol; }
    }

    [Header("흡수를 했을 때의 장착 확률을 올려주는 수치")]
    [SerializeField]
    private float upMountingPercentageValue = 5f;
    public float UpMountingPercentageValue
    {
        get { return upMountingPercentageValue; }
    }
    [Header("흡수를 했을 때 장착한 적의 동화율을 올려주는 수치")]
    [SerializeField]
    private int upUnderstandingRateValue = 1;
    public int UpUnderstandingRateValue
    {
        get { return upUnderstandingRateValue; }
    }

    public bool drainTutorial = false;

    private bool canDrain = true;
    public bool drainTutorialDone = false;
    public bool cantDrainObject = false;

    public override void Awake()
    {
        base.Awake();

        playerDrainCol = drainCollider.GetComponent<PlayerDrainCollider>();
        drainCollider.SetActive(false);
    }
    private void Start()
    {
        if(TutorialManager.Instance.IsTutorialStage)
        {
            Instantiate(Resources.Load<GameObject>(canDrainCheckColliderPath), transform);
        }
        else
        {
            drainTutorialDone = true;
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();

        EventManager.StartListening("OnDrain", OnDrain);
        EventManager.StartListening("EnemySpawnAfter", EnemyStop);
    }
    public override void OnDisable()
    {
        base.OnDisable();

        drainCollider.SetActive(false);

        EventManager.StopListening("OnDrain", OnDrain);
        EventManager.StopListening("EnemySpawnAfter", EnemyStop);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void WhenSkillDelayTimerZero()
    {
        base.WhenSkillDelayTimerZero();

        canDrain = true;
    }
    public override void DoSkill()
    {
        base.DoSkill();

        if (canDrain)
        {
            drainTutorial = false;

            DoDrain();
        }
    }
    public void DoDrainByTuto()
    {
        drainTutorial = true;

        DoDrain();
    }
    private void DoDrain()
    {
        if(!drainTutorialDone && !drainTutorial)
        {
            return;
        }

        player.PlayerState.IsDrain = true;
        player.PlayerOrderInLayerController.SetOrderInLayer("Object", 0);

        SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];

        EventManager.TriggerEvent("SetDrainTime", playerDrainCol.DrainTime);
        EventManager.TriggerEvent("EnemyStop");
        EventManager.TriggerEvent("PlayerStop");

        drainCollider.SetActive(true);

        canDrain = false;
    }
    private void OnDrain(GameObject obj, Vector2 position, int upValue) // upValue는 이해도(동화율)이 얼마나 오를 것인가.
    {
        Enemy.Enemy enemy = obj.GetComponent<Enemy.Enemy>();
        string objId = enemy.GetEnemyId();

        if (PlayerEnemyUnderstandingRateManager.Instance.CheckMountObjIdContain(objId))
        {
            PlayerEnemyUnderstandingRateManager.Instance.UpUnderstandingRate(objId, upUnderstandingRateValue);
        }
        else
        {
            PlayerEnemyUnderstandingRateManager.Instance.UpDrainProbabilityDict(objId, upMountingPercentageValue);
            PlayerEnemyUnderstandingRateManager.Instance.CheckMountingEnemy(objId, upUnderstandingRateValue);
        }

        if (enemy != null)
        {
            enemy.EnemyDestroy();
        }
    }
    private void EnemyStop()
    {
        if (player.PlayerState.IsDrain)
        {
            EventManager.TriggerEvent("EnemyStop");
        }
    }
}
