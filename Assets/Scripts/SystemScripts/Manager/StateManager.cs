using System.Collections.Generic;
using System;

public class StateManager : SingletonClass<StateManager>
{
    public Dictionary<StateAbnormality, int> stateCountDict = new Dictionary<StateAbnormality, int>();  //해당 상태이상이 앞으로 효과가 몇 번 더 발동이 되는지 저장

    public bool IsPlayerFullHP => SlimeGameManager.Instance.Player.CurrentHp == SlimeGameManager.Instance.Player.PlayerStat.MaxHp;
    public bool IsPlayerNoImpr
    {
        get
        {
            foreach(int cnt in stateCountDict.Values)
            {
                if (cnt > 0) return false;
            }
            return true;
        }
    }

    public void Init()
    {
        foreach(StateAbnormality state in Global.GetEnumArr<StateAbnormality>())
        {
            stateCountDict.Add(state, 0);
        }
        stateCountDict.Remove(StateAbnormality.None);
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
        StateAbnormalityEffect ase = Util.StringToClass<StateAbnormalityEffect>(state.ToString());

        if (count == 10001)
            ase.StopEffect();
        else
            ase.AddDuration(-count);
    }

    public void RemoveAllStateAbnormality()
    {
        StateAbnormalityEffect ase;
        for(int i = 0; i<Global.EnumCount<StateAbnormality>()-1; i++)
        {
            ase = Util.StringToClass<StateAbnormalityEffect>(((StateAbnormality)i).ToString());
            ase.StopEffect();
        }
    }

    public void OnStateAbnorEffect(StateAbnormality sa)
    {
        Util.StringToClass<StateAbnormalityEffect>(sa.ToString()).OnEffected();
    }
}
