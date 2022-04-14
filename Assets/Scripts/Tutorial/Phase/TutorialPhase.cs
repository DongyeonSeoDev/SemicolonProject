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
                    light.DOOuterRadius(4, 1.8f, true, () => End());
                }
            }
        }
       
    }

    public override void End()
    {
        SlimeGameManager.Instance.Player.PlayerInput.IsPauseByTuto = false;
        base.End();
    }
}

public class SettingPhase : TutorialPhase
{
    private int pressCount = 10, currentCount = 0;

    private float camShakeStr = 0.3f, camFreq = 0.7f;

    private int maxCount = 3, offset = 3;

    private Vector2 effSpawnPosOffset = new Vector2(0, 11f);

    public SettingPhase(int pressCount, Action end)
    {
        if (!PoolManager.IsContainKey("DustEffect1"))
        {
            GameObject o = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SystemPrefabs/Effect/FallDustEffect.prefab"));
            PoolManager.CreatePool(o, EffectManager.Instance.transform, 3, "DustEffect1");
        }

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
                //Camera Shake
                CinemachineCameraScript.Instance.Shake(camShakeStr, camFreq, 0.3f);
                camShakeStr += 0.2f;
                camFreq += 0.1f;

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
