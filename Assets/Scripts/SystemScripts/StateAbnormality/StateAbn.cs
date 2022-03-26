using UnityEngine;
using System;

public abstract class StateAbnormalityEffect
{

    public virtual int Duration { get; set; }
    public virtual StateAbnormality StateAbn { get; set; }

    public virtual void StartEffect()
    {
        StateManager.Instance.stateCountDict[StateAbn] += Duration;
        UIManager.Instance.RequestLeftBottomMsg(Global.StateAbnorToString(StateAbn) + " 저주에 걸렸습니다.");
    }

    public virtual void AddDuration(int value)
    {
        StateManager.Instance.stateCountDict[StateAbn] += value;
        if(StateManager.Instance.stateCountDict[StateAbn]<0)
        {
            StateManager.Instance.stateCountDict[StateAbn] = 0;
        }
    }

    public abstract void OnEffected();
}

public class Pain : StateAbnormalityEffect
{
    public override int Duration => 5;
    public override StateAbnormality StateAbn => StateAbnormality.Pain;

    public override void StartEffect()
    {
        base.StartEffect();
    }

    public override void OnEffected()
    {

    }
}

public class Scar : StateAbnormalityEffect
{
    public override int Duration => 4;
    public override StateAbnormality StateAbn => StateAbnormality.Scar;
    public override void StartEffect()
    {
        base.StartEffect();
    }
    public override void OnEffected()
    {

    }
}

public class Poverty : StateAbnormalityEffect
{
    public override int Duration => 6;
    public override StateAbnormality StateAbn => StateAbnormality.Poverty;

    public override void StartEffect()
    {
        base.StartEffect();
    }
    public override void OnEffected()
    {

    }
}

public class Blind : StateAbnormalityEffect
{
    public override int Duration => 5;
    public override StateAbnormality StateAbn => StateAbnormality.Blind;

    public override void StartEffect()
    {
        base.StartEffect();
    }

    public override void OnEffected()
    {

    }


}
