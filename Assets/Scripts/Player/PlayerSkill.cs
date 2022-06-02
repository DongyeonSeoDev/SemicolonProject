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

        if(SlimeGameManager.Instance.SkillDelayTimerZero[skillIdx])
        {
            SlimeGameManager.Instance.SkillDelayTimerZero[skillIdx] = false;

            WhenSkillDelayTimerZero();
        }
    }
    public virtual void OnEnable()
    {
        WhenChangeSkill();

        EventManager.StartListening("OnAttackSpeedChage", OnAttackSpeedChange);
        EventManager.StartListening("StartSkill" + skillIdx, DoSkill);
        EventManager.StartListening("SkillButtonUp" + skillIdx, SkillButtonUp);
        EventManager.StartListening("SkillCancel" + skillIdx, SkillCancel);
    }
    public virtual void OnDisable()
    {
        EventManager.StopListening("OnAttackSpeedChage", OnAttackSpeedChange);
        EventManager.StopListening("StartSkill" + skillIdx, DoSkill);
        EventManager.StopListening("SkillButtonUp" + skillIdx, SkillButtonUp);
        EventManager.StopListening("SkillCancel" + skillIdx, SkillCancel);
    }

    public void OnAttackSpeedChange()
    {
        SlimeGameManager.Instance.SetSkillDelay(skillIdx, skillDelay);
    }

    public void WhenChangeSkill()
    {
        SlimeGameManager.Instance.SetSkillDelay(skillIdx, skillDelay);
    }
    public virtual void DoSkill()
    {
        
    }
    public virtual void SkillButtonUp()
    {

    }
    public virtual void SkillCancel()
    {

    }
    public virtual void WhenSkillDelayTimerZero()
    {
        
    }

}
