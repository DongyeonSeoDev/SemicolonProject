using System.Collections.Generic;
using System;
using UnityEngine;

public class StateManager : SingletonClass<StateManager>
{
    public Dictionary<StateAbnormality, int> stateCountDict = new Dictionary<StateAbnormality, int>();  //해당 상태이상이 앞으로 효과가 몇 번 더 발동이 되는지 저장

    public void Init()
    {
        foreach(StateAbnormality state in Global.GetEnumArr<StateAbnormality>())
        {
            stateCountDict.Add(state, 0);
        }
    }

    public void StartStateAbnormality(StateAbnormality state, int count = 10001)
    {
        StateAbnormalityEffect ase = Activator.CreateInstance(Type.GetType(state.ToString())) as StateAbnormalityEffect;
        if (count == 10001)
            ase.StartEffect();
        else
            ase.AddDuration(count);
    }

    public void RemoveStateAbnormality(StateAbnormality state, int count = 10001)
    {
        if (count == 10001)
            stateCountDict[state] = 0;
        else
            stateCountDict[state] -= Mathf.Clamp(count, 0, stateCountDict[state]);
    }

    public void RemoveAllStateAbnormality()
    {
        foreach(StateAbnormality state in stateCountDict.Keys)
        {
            stateCountDict[state] = 0;
        }
    }
}
