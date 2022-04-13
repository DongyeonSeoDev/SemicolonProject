using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System;

public abstract class TutorialPhase
{
    public bool IsEnded { get; protected set; }
    public Action endAction = null;
    public virtual void End()
    {
        endAction?.Invoke();
        IsEnded = true;
    }
    public abstract void DoPhaseUpdate();
}

public class StartPhase : TutorialPhase
{
    private Light2D light;

    private int needCount = 2, currentCount = 0;
    private bool complete = false;

    public StartPhase(Light2D light, int touchCount)
    {
        this.light = light;
        needCount = touchCount;
        IsEnded = false;
    }
    public override void DoPhaseUpdate()
    {
        if (!complete)
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentCount++;
                if (needCount == currentCount)
                {
                    complete = true;
                    EffectManager.Instance.OnTouchEffect();
                    light.DOInnerRadius(2, 1.5f, true);
                    light.DOOuterRadius(4, 1.8f, true, () => base.End());
                }
            }
        }
       
    }

    
}

public class SettingPhase : TutorialPhase
{
    private ParticleSystem dustParticle;
    private int pressCount = 10, currentCount = 0;

    public SettingPhase(ParticleSystem effect, int pressCount, Action end)
    {
        dustParticle = effect;
        this.pressCount = pressCount;
        endAction = end;
        IsEnded = false;
    }
    public override void DoPhaseUpdate()
    {
        if(Input.GetKeyDown(KeySetting.fixedKeyDict[KeyAction.ESCAPE]))
        {
            if(++currentCount < pressCount)
            {
                
            }
            else
            {
                End();
            }
        }
    }

    public override void End()
    {
        base.End();
    }
}
