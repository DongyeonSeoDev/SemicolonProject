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

    public virtual void StopEffect()
    {
        bool alreadyImp = StateManager.Instance.stateCountDict[StateAbn] > 0;

        StateManager.Instance.stateCountDict[StateAbn] = 0;

        if(alreadyImp)
           UIManager.Instance.RequestLeftBottomMsg(Global.StateAbnorToString(StateAbn) + " 저주가 해제되었습니다.");
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
        //EventManager.StopListening("StartNextStage", OnEffected);
        if(StateManager.Instance.stateCountDict[StateAbn] == Duration)
           EventManager.StartListening("StartNextStage", OnEffected);
    }

    public override void StopEffect()
    {
        base.StopEffect();
        EventManager.StopListening("StartNextStage", OnEffected);
    }

    public override void OnEffected()
    {
        if(StateManager.Instance.stateCountDict[StateAbn] <= 0)
        {
            Debug.Log("잘못된 상황 발생. 확인 필요.");
            return;
        }

        ItemUseMng.DecreaseCurrentHP(10);
        if (StateManager.Instance.stateCountDict[StateAbn] > 1)
        {
            StateManager.Instance.stateCountDict[StateAbn]--;
        }
        else
        {
            StopEffect();
        }
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
    public override void StopEffect()
    {
        base.StopEffect();
    }
    public override void OnEffected()
    {
        if (StateManager.Instance.stateCountDict[StateAbn] > 1)
        {
            StateManager.Instance.stateCountDict[StateAbn]--;
        }
        else
        {
            StopEffect();
        }
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
    public override void StopEffect()
    {
        base.StopEffect();
    }
    public override void OnEffected()
    {
        //재화 시스템 아직 없으므로 보류
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
    public override void StopEffect()
    {
        base.StopEffect();
    }
    public override void OnEffected()
    {
        //이건 기획 듣고 좀 봐야할듯
    }


}
