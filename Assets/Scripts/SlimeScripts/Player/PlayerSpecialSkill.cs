using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSpecialSkill : PlayerAction
{
    [SerializeField]
    protected float skillDelay = 5f;
    protected float skillDelayTimer = 0f;

    public virtual void Update()
    {
        CheckSkillDelay();
    }
    public virtual void OnEnable()
    {
        WhenChangeSkill();
    }

    public void WhenChangeSkill()
    {
        SlimeGameManager.Instance.CurrentSkillDelay = skillDelay;
        SlimeGameManager.Instance.CurrentSkillDelay = skillDelayTimer;
    }
    public virtual void DoSkill()
    {

    }
    protected void CheckSkillDelay()
    {
        if (skillDelayTimer > 0f)
        {
            skillDelayTimer -= Time.deltaTime;
            SlimeGameManager.Instance.CurrentSkillDelay = skillDelayTimer;

            if (skillDelayTimer <= 0f)
            {
                WhenSkillDelayTimerZero();
            }
        }
    }
    public virtual void WhenSkillDelayTimerZero()
    {

    }

}
