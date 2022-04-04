using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrain : PlayerSkill
{
    [SerializeField]
    private GameObject drainCollider = null; // drain 체크에 사용될 Collider
    private PlayerDrainCollider playerDrainCol = null;

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

    //[Header("Drain되는 오브젝트가 빨려들어오는 속도")]
    //[SerializeField]
    //private float drainSpeed = 2f;

    //[Header("Drain 되는 오브젝트와 플레이어간의 거리가 이정도보다 작아져야 드레인 판정")]
    //[SerializeField]
    //private float drainDoneDistance = 0.1f;

    private bool canDrain = true;

    //[SerializeField]
    //private float reDrainTime = 10f;
    //private float reDrainTimer = 0f;

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

        //DoDrain();
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

            SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];
            EventManager.TriggerEvent("SetDrainTime", playerDrainCol.DrainTime);

            drainCollider.SetActive(true);

            canDrain = false;
        }
    }
    // DoDrain은 흡수할 적 오브젝트를 끌고와, 일정 거리 이하로 가까워지면 흡수판정을 하는 함수다.
    // 이 함수의 역할을 이제 PlayerDrainCollider스크립트의 멤버함수가 하도록 한다.
    //private void DoDrain()
    //{
    //    if (player.DrainList.Count > 0)
    //    {
    //        List<(GameObject, int)> removeList = new List<(GameObject, int)>();

    //        foreach (var item in player.DrainList)
    //        {
    //            float distance = Vector2.Distance(transform.position, item.Item1.transform.position);

    //            if (!moveTimeDic.ContainsKey(item.Item1))
    //            {
    //                moveTimeDic.Add(item.Item1, distance / drainSpeed);
    //            }

    //            item.Item1.transform.position = Vector2.Lerp(item.Item1.transform.position, transform.position, moveTimerDic[item.Item1] / moveTimeDic[item.Item1]);

    //            if (distance <= drainDoneDistance) // 흡수 판정 체크
    //            {
    ////                Enemy.Enemy enemy = item.Item1.GetComponent<Enemy.Enemy>();
    ////                string objId = enemy.GetEnemyId();

    ////                if (PlayerEnemyUnderstandingRateManager.Instance.CheckMountObjIdContain(objId))
    ////                {
    ////                    PlayerEnemyUnderstandingRateManager.Instance.UpUnderstandingRate(objId, item.Item2);
    //                }
    //                else
    //                {
    //                    PlayerEnemyUnderstandingRateManager.Instance.UpDrainProbabilityDict(objId, upMountingPercentageValue);
    //                    PlayerEnemyUnderstandingRateManager.Instance.CheckMountingEnemy(objId, item.Item2);
    //                }

    //                removeList.Add(item);

    //                if (enemy != null)
    //                {
    //                    enemy.EnemyDestroy();

    //                    continue;
    //                }
    //            }

    //            if (moveTimerDic.ContainsKey(item.Item1))
    //            if (moveTimerDic.ContainsKey(item.Item1))
    //            {
    //                moveTimerDic[item.Item1] += Time.deltaTime;
    //            }
    //        }

    //        removeList.ForEach(x =>
    //        {
    //            moveTimeDic.Remove(x.Item1);
    //            moveTimerDic.Remove(x.Item1);
    //            player.DrainList.Remove(x);
    //        });
    //    }
    //}
    private void OnDrain(GameObject obj, Vector2 position, int upValue) // upValue는 이해도(동화율)이 얼마나 오를 것인가.
    {
        //if (moveTimerDic.ContainsKey(obj)) // 이 타이머들은 DoDrain을 위한 것이다. DoDrain을 사용하지 않으므로 마찬가지로 사용하지 않는다.
        //{
        //    moveTimerDic[obj] = 0f;
        //}
        //else
        //{
        //    moveTimerDic.Add(obj, 0f);
        //}
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
