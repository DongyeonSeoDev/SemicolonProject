using System.Collections.Generic;
using System;
using UnityEngine;
using Water;

public class StateManager : SingletonClass<StateManager>
{
    private Dictionary<string, BuffStateDataSO> idToStateDataDic = new Dictionary<string, BuffStateDataSO>();

    public Dictionary<string, int> stateCountDict = new Dictionary<string, int>();  //해당 상태이상이 앞으로 효과가 몇 번 더 발동이 되는지 저장

    public Dictionary<string, Action<bool>> stateStopDict = new Dictionary<string, Action<bool>>(); //버프를 해제시키는 함수를 담아놓음

    private Dictionary<string, BuffSlot> buffSlotDic = new Dictionary<string, BuffSlot>(); //왼 위 적용중인 버프 아이콘들


    public bool IsPlayerFullHP => SlimeGameManager.Instance.Player.PlayerStat.currentHp == SlimeGameManager.Instance.Player.PlayerStat.MaxHp;
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
            stateStopDict.Add(state.ToString(), null);
        }
        stateCountDict.Remove(StateAbnormality.None.ToString());

        Transform slotPar = GameObject.Find("StateInfoImagesPanel").transform;
        GameObject slotUI =  Resources.Load<GameObject>("System/UI/UIStateInfoSlot");
        PoolManager.CreatePool(slotUI, slotPar, 3, "StateSlot");

        foreach(BuffStateDataSO data in Resources.LoadAll<BuffStateDataSO>("System/State/"))
        {
            idToStateDataDic.Add(data.Id, data);
        }
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
            stateStopDict[state.ToString()]?.Invoke(true);
        else
            ase.AddDuration(-count);

        RemoveBuffSlotUI(state.ToString());
    }

    public void RemoveAllStateAbnormality(bool showLog = true)
    {
        //StateAbnormalityEffect ase;
        string key;

        for (int i = 0; i<Global.EnumCount<StateAbnormality>()-1; i++)
        {
            key = ((StateAbnormality)i).ToString();
            stateStopDict[key]?.Invoke(showLog);
            RemoveBuffSlotUI(key);
            //ase = WDUtil.StringToClass<StateAbnormalityEffect>(((StateAbnormality)i).ToString());
            //ase.StopEffect(showLog);  //새로 만든 것을 스탑해서 그런지 이벤트에서 스탑 함수가 제거가 안됨
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

    public void Reset()
    {
        idToStateDataDic.Clear();
        stateCountDict.Clear();
        buffSlotDic.Clear();
        stateStopDict.Clear();  
    }
}
