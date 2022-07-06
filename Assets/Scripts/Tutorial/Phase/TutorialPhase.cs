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
            if (Input.GetMouseButtonDown(0))  // 아무곳이나 터치하면
            {
                AnyKeyEvent();
                EffectManager.Instance.SpawnEffect(PoolManager.GetItem("TouchEffect1"), Util.MousePositionForScreenSpace, 1f, true);  //터치 이펙트
            }
            else if(Input.anyKeyDown)  //아무 키나 누르면
            {
                AnyKeyEvent();
                EffectManager.Instance.SpawnEffect(PoolManager.GetItem("TouchEffect1"), Util.ScreenToWorldPosForScrSpace(ScriptHelper.RandomVector(new Vector2(150f, 150f), new Vector2(1000f, 600f))), 1f, true);
            }
        }
       
    }

    private void AnyKeyEvent()
    {
        if(currentCount++ == 0)
        {
            TalkManager.Instance.SetSubtitle(new string[3] { "!", "누구세요?", "뭔가 힘이 느껴진 것 같은데..." }, 
                new float[3] {0.2f, 0.15f, 0.2f},
                new float[3] {1f, 1.5f, 1.5f});
        }
        if (needCount == currentCount)
        {
            complete = true;

            //EffectManager.Instance.OnTouchEffect();
            light.DOInnerRadius(2, 1.5f, true);
            light.DOOuterRadius(4, 1.6f, true, () => End());
        }
    }

    public override void End()
    {
        SlimeGameManager.Instance.Player.PlayerInput.IsPauseByTuto = false;

        StoredData.GetGameObjectData("PlayerHeadTxtObj", true).GetComponent<CanvasGroup>().alpha = 1;
        StoredData.GetGameObjectData<CanvasGroup>("PlayerHeadImgParent", true).alpha = 1;

        base.End();
    }
}

public class SettingPhase : TutorialPhase
{
    private int pressCount = 10, currentCount = 0;

    private float camShakeStr = 0.3f, camFreq = 0.7f;

    private int maxCount = 9, offset = 4;

    private Vector2 effSpawnPosOffset = new Vector2(0, 13f);

    public SettingPhase(int pressCount, Action end)
    {
        if (!PoolManager.IsContainKey("DustEffect1"))
        {
            GameObject o = Resources.Load<GameObject>("System/Effects/FallDustEffectUI"); 
            PoolManager.CreatePool(o, UIManager.Instance.worldUICvsg.transform, 3, "DustEffect1");
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
                camShakeStr += 0.25f;
                camFreq += 0.14f;

                SoundManager.Instance.PlaySoundBox("ESC Effect SFX");

                //Dust Particle Effect
                ParticleSystem ps = PoolManager.GetItem<ParticleSystem>("DustEffect1");
                ParticleSystem.MainModule main = ps.main;
                main.maxParticles = maxCount;
                maxCount += offset;

                ps.Play();
                Util.DelayFunc(() => ps.gameObject.SetActive(false), 2f);

                /*SlimeFollowObj sfo = PoolManager.GetItem<SlimeFollowObj>("PlayerFollowEmptyObj");
                sfo.offset = effSpawnPosOffset;
                ps.gameObject.transform.SetParent(sfo.transform);
                ps.gameObject.transform.localPosition = Vector2.zero;
                ps.Play();
                Util.DelayFunc(() =>
                {
                    ps.gameObject.transform.parent = null;
                    sfo.gameObject.SetActive(false);
                    ps.gameObject.SetActive(false);
                },2f);*/   
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
        Util.DelayFunc(() => PoolManager.ClearPool("DustEffect1", true), 5f);
    }
}

public class AbsorptionPhase : TutorialPhase
{

    private float startTime;

    public AbsorptionPhase(Action end)
    {
        IsEnded = false;
        endAction = end;
        startTime = Time.unscaledTime + 1f;
    }

    public override void DoPhaseUpdate()
    {
        if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.SPECIALATTACK2]) && startTime < Time.unscaledTime)
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


public class QuikSlotTutorialPhase : TutorialPhase
{
    RectTransform emphasisEffect;

    public QuikSlotTutorialPhase(RectTransform emphaRt)
    {
        emphasisEffect = emphaRt;
    }

    public override void DoPhaseUpdate()
    {
        if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.ITEM_QUIKSLOT]))
        {
            End();
        }
    }

    public override void End()
    {
        emphasisEffect.gameObject.SetActive(false);

        Util.DelayFunc(() =>
        {
            KeyActionManager.Instance.GetElement(InitGainType.HP_STAT);
        }, 2.5f);

        base.End();
    }
}