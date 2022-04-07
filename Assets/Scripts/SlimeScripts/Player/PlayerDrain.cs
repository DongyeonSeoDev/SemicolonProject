using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrain : PlayerSkill
{
    [SerializeField]
    private GameObject drainCollider = null; // drain 체크에 사용될 Collider
    private PlayerDrainCollider playerDrainCol = null;
    public PlayerDrainCollider PlayerDrainCol
    {
        get { return playerDrainCol; }
    }

    //private List<(GameObject, int)> drainList = new List<(GameObject, int)>();

    //private Dictionary<GameObject, float> moveTimeDic = new Dictionary<GameObject, float>();
    //private Dictionary<GameObject, float> moveTimerDic = new Dictionary<GameObject, float>();

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

    private bool canDrain = true;

    public override void Awake()
    {
        base.Awake();

        playerDrainCol = drainCollider.GetComponent<PlayerDrainCollider>();
        drainCollider.SetActive(false);
    }
    public override void OnEnable()
    {
        base.OnEnable();

        EventManager.StartListening("OnDrain", OnDrain);
    }
    public override void OnDisable()
    {
        base.OnDisable();

        EventManager.StopListening("OnDrain", OnDrain);
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
            player.PlayerState.IsDrain = true;
            player.PlayerOrderInLayerController.SetOrderInLayer("Player", 5);

            SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];

            EventManager.TriggerEvent("EnemyStop");
            EventManager.TriggerEvent("SetDrainTime", playerDrainCol.DrainTime);

            drainCollider.SetActive(true);

            canDrain = false;
        }
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
}
