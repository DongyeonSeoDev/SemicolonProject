using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSkill : PlayerAction
{
    [SerializeField]
    protected float skillDelay = 5f;

    [Header("�� ��ų�� ������ ��Ÿ���� �� 0: �⺻����, 1: ��ų1, 2: ��ų2")]
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
