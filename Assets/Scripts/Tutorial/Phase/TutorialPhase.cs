using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System;
using FkTweening;
using Water;

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
                    light.DOOuterRadius(4, 1.6f, true, () => End());
                }
            }
        }
       
    }

    public override void End()
    {
        SlimeGameManager.Instance.Player.PlayerInput.IsPauseByTuto = false;

        StoredData.GetGameObjectData("PlayerHeadTxtObj").GetComponent<CanvasGroup>().alpha = 1;
        StoredData.DeleteGameObjectKey("PlayerHeadTxtObj");

        base.End();
    }
}

public class SettingPhase : TutorialPhase
{
    private int pressCount = 10, currentCount = 0;

    private float camShakeStr = 0.3f, camFreq = 0.7f;

    private int maxCount = 3, offset = 3;

    private Vector2 effSpawnPosOffset = new Vector2(0, 13f);

    public SettingPhase(int pressCount, Action end)
    {
        if (!PoolManager.IsContainKey("DustEffect1"))
        {
            GameObject o = Resources.Load<GameObject>("System/Effects/FallDustEffect"); 
            PoolManager.CreatePool(o, EffectManager.Instance.transform, 3, "DustEffect1");
        }

        this.pressCount = pressCount;
        endAction = end;
        IsEnded = false;
    }

    private bool RecognizeESC()
    {
        if (Input.GetKeyDown(KeySetting.fixedKeyDict[KeyAction.SETTING]) && !TimeManager.IsTimePaused && UIManager.Instance.CanInteractUI)
        {
            if(!Util.IsActiveGameUI(UIType.SKILLDETAIL) && !Util.IsActiveGameUI(UIType.STATEINFO))
            {
                return true;
            }
        }
        return false;
    }

    public override void DoPhaseUpdate()
    {
        if(RecognizeESC())
        {
            if(++currentCount < pressCount)
            {
                //Camera Shake
                CinemachineCameraScript.Instance.Shake(camShakeStr, camFreq, 0.3f);
                camShakeStr += 0.2f;
                camFreq += 0.1f;

                SoundManager.Instance.PlaySoundBox("ESC Effect SFX");

                //Dust Particle Effect
                ParticleSystem ps = PoolManager.GetItem<ParticleSystem>("DustEffect1");
                ParticleSystem.MainModule main = ps.main;
                main.maxParticles = maxCount;
                maxCount += offset;

                SlimeFollowObj sfo = PoolManager.GetItem<SlimeFollowObj>("PlayerFollowEmptyObj");
                sfo.offset = effSpawnPosOffset;
                ps.gameObject.transform.SetParent(sfo.transform);
                ps.gameObject.transform.localPosition = Vector2.zero;
                ps.Play();
                Util.DelayFunc(() =>
                {
                    ps.gameObject.transform.parent = null;
                    sfo.gameObject.SetActive(false);
                    ps.gameObject.SetActive(false);
                },2f);          
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

public class AbsorptionPhase : TutorialPhase
{
    public AbsorptionPhase(Action end)
    {
        IsEnded = false;
        endAction = end;
    }

    public override void DoPhaseUpdate()
    {
        if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK2]))
        {
            End();
        }
    }

    public override void End()
    {
        base.End();
    }
}

public class RushTutorialPhase : TutorialPhase
{

    private GameObject emphasisEff;

    public RushTutorialPhase(GameObject emphasisEff)
    {
        this.emphasisEff = emphasisEff;
    }

    public override void DoPhaseUpdate()
    {
        if(Input.GetKeyUp(KeySetting.keyDict[KeyAction.SPECIALATTACK1]))
        {
            End();
        }
    }

    public override void End()
    {
        emphasisEff.SetActive(false);
        base.End();
    }
}
