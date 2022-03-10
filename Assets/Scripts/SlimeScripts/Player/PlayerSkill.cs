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
        //CheckSkillDelay();
    }
    public virtual void OnEnable()
    {
        WhenChangeSkill();

        EventManager.StartListening("StartSkill" + skillIdx, DoSkill);
        EventManager.StartListening("Skill" + skillIdx + "DelayTimerZero", WhenSkillDelayTimerZero);
    }
    public virtual void OnDisable()
    {
        EventManager.StopListening("StartSkill" + skillIdx, DoSkill);
        EventManager.StopListening("Skill" + skillIdx + "DelayTimerZero", WhenSkillDelayTimerZero);
    }

    public void WhenChangeSkill()
    {
        SlimeGameManager.Instance.SetSkillDelay(skillIdx, skillDelay);
    }
    public virtual void DoSkill()
    {
        SlimeGameManager.Instance.CurrentSkillDelayTimer[skillIdx] = SlimeGameManager.Instance.SkillDelays[skillIdx];
    }
    public virtual void WhenSkillDelayTimerZero()
    {

    }

}
