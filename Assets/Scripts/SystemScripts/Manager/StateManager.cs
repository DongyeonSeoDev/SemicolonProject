using System.Collections.Generic;
using System;
using UnityEngine;
using Water;

public class StateManager : SingletonClass<StateManager>
{
    private Dictionary<string, BuffStateDataSO> idToStateDataDic = new Dictionary<string, BuffStateDataSO>();

    public Dictionary<string, int> stateCountDict = new Dictionary<string, int>();  //해당 상태이상이 앞으로 효과가 몇 번 더 발동이 되는지 저장

    private Dictionary<string, BuffSlot> buffSlotDic = new Dictionary<string, BuffSlot>();


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

    public BuffStateDataSO GetBuffStateData(string id)
    {
        if (idToStateDataDic.ContainsKey(id)) return idToStateDataDic[id];

        BuffStateDataSO data = Resources.Load<BuffStateDataSO>("System/State/" + id);
        idToStateDataDic.Add(id, data);
        return data;
    }

    public void Init()
    {
        foreach(StateAbnormality state in Global.GetEnumArr<StateAbnormality>())
        {
            stateCountDict.Add(state.ToString(), 0);
        }
        stateCountDict.Remove(StateAbnormality.None.ToString());

        Transform slotPar = GameObject.Find("StateInfoImagesPanel").transform;
        GameObject slotUI = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SystemPrefabs/UI/Slot/StateInfoSlot.prefab");
        PoolManager.CreatePool(slotUI, slotPar, 3, "StateSlot");
    }

    public void StartStateAbnormality(StateAbnormality state, int count = 10001)
    {
        StateAbnormalityEffect ase = Activator.CreateInstance(Type.GetType(state.ToString())) as StateAbnormalityEffect;
        
        if (count == 10001)
            ase.StartEffect();
        else
            ase.AddDuration(count);

        CreateOrUpdateBuffSlotUI(state.ToString());
    }

    public void RemoveStateAbnormality(StateAbnormality state, int count = 10001)
    {
        StateAbnormalityEffect ase = WDUtil.StringToClass<StateAbnormalityEffect>(state.ToString());

        if (count == 10001)
            ase.StopEffect();
        else
            ase.AddDuration(-count);

        RemoveBuffSlotUI(state.ToString());
    }

    public void RemoveAllStateAbnormality(bool showLog = true)
    {
        StateAbnormalityEffect ase;
        for(int i = 0; i<Global.EnumCount<StateAbnormality>()-1; i++)
        {
            ase = WDUtil.StringToClass<StateAbnormalityEffect>(((StateAbnormality)i).ToString());
            ase.StopEffect(showLog);
            RemoveBuffSlotUI(((StateAbnormality)i).ToString());
        }
    }

    public void OnStateAbnorEffect(StateAbnormality sa)
    {
        WDUtil.StringToClass<StateAbnormalityEffect>(sa.ToString()).OnEffected();
    }

    public void CreateOrUpdateBuffSlotUI(string id)
    {
        if (buffSlotDic.ContainsKey(id)) buffSlotDic[id].UpdateInfo();
        else
        {
            BuffSlot slot = PoolManager.GetItem<BuffSlot>("StateSlot");
            buffSlotDic.Add(id, slot);
            slot.SetData(GetBuffStateData(id));
        }
    }

    public void RemoveBuffSlotUI(string id)
    {
        if(buffSlotDic.ContainsKey(id) && stateCountDict[id] <= 0)
        {
            buffSlotDic[id].gameObject.SetActive(false);
            buffSlotDic.Remove(id);
        }
    }

    public void UpdateBuffSlotUI(string id)
    {
        if (buffSlotDic.ContainsKey(id))
        {
            if (stateCountDict[id] <= 0)
                RemoveBuffSlotUI(id);
            else
                CreateOrUpdateBuffSlotUI(id);
        }
    }
}
