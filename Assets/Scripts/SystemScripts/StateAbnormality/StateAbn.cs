using UnityEngine;
using System;

public abstract class StateAbnormalityEffect
{

    public virtual int Duration => StateManager.Instance.GetBuffStateData(GetType().Name).duration;
    public abstract StateAbnormality StateAbn { get; }

    public virtual void StartEffect()
    {
        StateManager.Instance.stateCountDict[StateAbn.ToString()] += Duration;
        UIManager.Instance.RequestLeftBottomMsg(StateManager.Instance.GetBuffStateData(StateAbn.ToString()).stateName + " 저주에 걸렸습니다.");
    }

    public virtual void StopEffect(bool showLog = true)
    {
        bool alreadyImp = StateManager.Instance.stateCountDict[StateAbn.ToString()] > 0;

        StateManager.Instance.stateCountDict[StateAbn.ToString()] = 0;

        if(alreadyImp && showLog)
           UIManager.Instance.RequestLeftBottomMsg(StateManager.Instance.GetBuffStateData(StateAbn.ToString()).stateName + " 저주가 해제되었습니다.");
    }

    public virtual void AddDuration(int value)
    {
        StateManager.Instance.stateCountDict[StateAbn.ToString()] += value;
        if(StateManager.Instance.stateCountDict[StateAbn.ToString()] <0)
        {
            StateManager.Instance.stateCountDict[StateAbn.ToString()] = 0;
        }
    }

    public abstract void OnEffected();
}

public class Pain : StateAbnormalityEffect
{
    //public override int Duration => 5;
    public override StateAbnormality StateAbn => StateAbnormality.Pain;

    public override void StartEffect()
    {
        base.StartEffect();
        //EventManager.StopListening("StartNextStage", OnEffected);
        if (StateManager.Instance.stateCountDict[StateAbn.ToString()] == Duration)
        {
            EventManager.StartListening("StartNextStage", OnEffected);
            StateManager.Instance.stateStopDict[StateAbn.ToString()] = StopEffect;
        }
    }

    public override void StopEffect(bool showLog = true)
    {
        base.StopEffect(showLog);
        EventManager.StopListening("StartNextStage", OnEffected);
    }

    public override void OnEffected()
    {
        if(StateManager.Instance.stateCountDict[StateAbn.ToString()] <= 0)
        {
            Debug.Log("잘못된 상황 발생. 확인 필요.");
            return;
        }

        ItemUseMng.DecreaseCurrentHP(10);
        if (StateManager.Instance.stateCountDict[StateAbn.ToString()] > 1)
        {
            StateManager.Instance.stateCountDict[StateAbn.ToString()]--;
        }
        else
        {
            StopEffect();
        }

        StateManager.Instance.UpdateBuffSlotUI(StateAbn.ToString());
    }
}

public class Scar : StateAbnormalityEffect
{
    //public override int Duration => 4;
    public override StateAbnormality StateAbn => StateAbnormality.Scar;
    public override void StartEffect()
    {
        base.StartEffect();
        SlimeGameManager.Instance.Player.GetExtraDamagePercantage = 20;

        if (StateManager.Instance.stateCountDict[StateAbn.ToString()] == Duration)
        {
            EventManager.StartListening("StageClear", OnEffected);
            StateManager.Instance.stateStopDict[StateAbn.ToString()] = StopEffect;
        }
    }
    public override void StopEffect(bool showLog = true)
    {
        base.StopEffect(showLog);
        SlimeGameManager.Instance.Player.GetExtraDamagePercantage = 0;
        EventManager.StopListening("StageClear", OnEffected);
    }
    public override void OnEffected()
    {
        if (StageManager.Instance.CurrentAreaType == AreaType.MONSTER)
        {
            if (StateManager.Instance.stateCountDict[StateAbn.ToString()] <= 0)
            {
                Debug.Log("잘못된 상황 발생. 확인 필요.");
                return;
            }

            if (StateManager.Instance.stateCountDict[StateAbn.ToString()] > 1)
            {
                StateManager.Instance.stateCountDict[StateAbn.ToString()]--;
            }
            else
            {
                StopEffect();
            }
        }

        StateManager.Instance.UpdateBuffSlotUI(StateAbn.ToString());
    }
}

public class Poverty : StateAbnormalityEffect
{
    //public override int Duration => 6;
    public override StateAbnormality StateAbn => StateAbnormality.Poverty;

    public override void StartEffect()
    {
        base.StartEffect();
    }
    public override void StopEffect(bool showLog = true)
    {
        base.StopEffect(showLog);
    }
    public override void OnEffected()
    {
        //재화 시스템 아직 없으므로 보류
    }
}

public class Blind : StateAbnormalityEffect
{
    //public override int Duration => 5;
    public override StateAbnormality StateAbn => StateAbnormality.Blind;

    public override void StartEffect()
    {
        base.StartEffect();
        if (StateManager.Instance.stateCountDict[StateAbn.ToString()] == Duration)
        {
            EventManager.StartListening("StartNextStage", OnEffected);
            StateManager.Instance.stateStopDict[StateAbn.ToString()] = StopEffect;
        }
    }
    public override void StopEffect(bool showLog = true)
    {
        base.StopEffect(showLog);
        EventManager.StopListening("StartNextStage", OnEffected);
    }
    public override void OnEffected()
    {
        if (StateManager.Instance.stateCountDict[StateAbn.ToString()] <= 0)
        {
            Debug.Log("잘못된 상황 발생. 확인 필요.");
            return;
        }

        if (StateManager.Instance.stateCountDict[StateAbn.ToString()] > 1)
        {
            StateManager.Instance.stateCountDict[StateAbn.ToString()]--;
        }
        else
        {
            StopEffect();
        }

        StateManager.Instance.UpdateBuffSlotUI(StateAbn.ToString());
    }


}
