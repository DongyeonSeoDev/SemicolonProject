using UnityEngine;
using System;

public abstract class StateAbnormalityEffect
{

    public virtual int Duration { get; set; }
    public virtual StateAbnormality StateAbn { get; set; }

    public virtual void StartEffect()
    {
        StateManager.Instance.stateCountDict[StateAbn] += Duration;
        UIManager.Instance.RequestLeftBottomMsg(Global.StateAbnorToString(StateAbn) + " ���ֿ� �ɷȽ��ϴ�.");
    }

    public virtual void StopEffect()
    {
        StateManager.Instance.stateCountDict[StateAbn] = 0;
        UIManager.Instance.RequestLeftBottomMsg(Global.StateAbnorToString(StateAbn) + " ���ְ� �����Ǿ����ϴ�.");
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
        EventManager.StopListening("StartNextStage", OnEffected);
        EventManager.StartListening("StartNextStage", OnEffected);
    }

    public override void StopEffect()
    {
        base.StopEffect();
        EventManager.StopListening("StartNextStage", OnEffected);
    }

    public override void OnEffected()
    {
        if (StateManager.Instance.stateCountDict[StateAbn] > 0)
        {
            ItemUseMng.DecreaseCurrentHP(10);
            StateManager.Instance.stateCountDict[StateAbn]--;
        }
        else
        {
            StateManager.Instance.stateCountDict[StateAbn] = 0;
            EventManager.StopListening("StartNextStage", OnEffected);
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
        //�������ʿ��� �޴� ������ 20�� �����ϴ°� ó���ؾ��ҵ�
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
        //��ȭ �ý��� ���� �����Ƿ� ����
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
        //�̰� ��ȹ ��� �� �����ҵ�
    }


}
