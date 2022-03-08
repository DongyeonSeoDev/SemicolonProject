using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSkill : PlayerAction
{
    [SerializeField]
    protected float skillDelay = 5f;

    [Header("이 스킬의 종류를 나타내는 값 0: 기본공격, 1: 스킬1, 2: 스킬2")]
    [SerializeField]
    protected int skillIdx = 0;

    public virtual void Update()
    {
        CheckSkillDelay();
    }
    public virtual void OnEnable()
    {
        WhenChangeSkill();

        EventManager.StartListening("StartSkill" + skillIdx, DoSkill);
    }
    public virtual void OnDisable()
    {
        EventManager.StopListening("StartSkill" + skillIdx, DoSkill);
    }

    public void WhenChangeSkill()
    {
        SlimeGameManager.Instance.SkillDelays[skillIdx] = skillDelay;
        SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = 0f;
    }
    public virtual void DoSkill()
    {
     
    }
    protected void CheckSkillDelay()
    {
        if (SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] > 0f)
        {
            SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] -= Time.deltaTime;

            if (SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] <= 0f)
            {
                WhenSkillDelayTimerZero();
            }
        }
    }
    public virtual void WhenSkillDelayTimerZero()
    {

    }

}
