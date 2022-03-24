using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateManager 
{
    private static StateManager instance;
    public static StateManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new StateManager();
            }
            return instance;
        }
    }

    public Dictionary<StateAbnormality, int> stateCountDict = new Dictionary<StateAbnormality, int>();

    public void Init()
    {
        foreach(StateAbnormality state in Global.GetEnumArr<StateAbnormality>())
        {
            stateCountDict.Add(state, 0);
        }
    }

    public void GetStateAbnormality(StateAbnormality state)
    {
        StateAbnormalityEffect ase = Activator.CreateInstance(Type.GetType(state.ToString())) as StateAbnormalityEffect;
        ase.StartEffect();
    }
}

public abstract class StateAbnormalityEffect
{

    public virtual int Duration { get; set; }
    public virtual StateAbnormality StateAbn { get; set; }

    public virtual void StartEffect() 
    {
        StateManager.Instance.stateCountDict[StateAbn] += Duration;
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
