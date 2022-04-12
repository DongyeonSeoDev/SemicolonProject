using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public abstract class TutorialPhase
{
    public bool IsEnded { get; protected set; }
    public abstract void DoPhaseUpdate();
}

public class StartPhase : TutorialPhase
{
    private Light2D light;

    private int needCount = 2, currentCount = 0;
    private bool complete = false;

    public StartPhase(Light2D light)
    {
        this.light = light;
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
                }
            }
        }
        else
        {
            if (light.pointLightInnerRadius < 2f)
            {
                light.pointLightInnerRadius += Time.deltaTime * 5f;
            }
            if (light.pointLightOuterRadius < 4f)
            {
                light.pointLightOuterRadius += Time.deltaTime * 5f;
            }
        }
    }
}
