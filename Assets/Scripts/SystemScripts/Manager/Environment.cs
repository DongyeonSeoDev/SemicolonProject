using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class Environment : MonoSingleton<Environment>
{
    public Light2D mainLight;
    public Volume mainVolume;

    public VolumeProfile defaultVolProfile, mainVolProfile;  //처음 상태의 기본 볼륨 프로필, 게임속에서 바뀌는 불륨 프로필

    #region Post Processing Options
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;
    private LiftGammaGain LGG;
    #endregion

    public Color damagedColor;
    public Color changeBodyColor;

    private void Awake()
    {
        if (!mainLight) mainLight = FindObjectOfType<Light2D>();
        SetVolume();

        DefineEvent();
    }

    private void SetVolume()
    {
        mainVolume.profile.TryGet<Bloom>(out bloom);
        mainVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        mainVolume.profile.TryGet<Vignette>(out vignette);
        mainVolume.profile.TryGet<LiftGammaGain>(out LGG);
    }

    private void DefineEvent()
    {
        TimeManager.timePauseAction += ()=> mainLight.DOIntensity(0.7f, 0.5f, true);
        TimeManager.timeResumeAction += () => mainLight.DOIntensity(1f, 0.5f, true);

        EventManager.StartListening("ChangeBody", (str, b) =>
        {
            if (string.IsNullOrEmpty(str) == b) { } //매개변수 확인용
            vignette.color.Override(changeBodyColor);
            vignette.DOVignetteIntensity(0.4f, 0.3f, false, () => vignette.DOVignetteIntensity(0f, 0.3f, false));
        });
    }

    public void OnDamaged()
    {
        vignette.color.Override(damagedColor);
        vignette.DOVignetteIntensity(0.4f, 0.3f, false, () =>vignette.DOVignetteIntensity(0f, 0.3f, false));
    }

    public void OnEnteredOrExitRecoveryArea(bool enter)
    {
        mainLight.intensity = enter ? 1.5f : 1f;
        bloom.intensity.value = enter ? 1.5f : 1f;
        bloom.threshold.value = enter ? 0.8f : 1f;

        if (enter)
        {
            Action action = null;
            action += ()=>
            {
                OnEnteredOrExitRecoveryArea(false);
                EventManager.StopListening(Global.EnterNextMap, action);
            };

            EventManager.StartListening(Global.EnterNextMap, action);
        }
    }

    public void OnEnteredOrExitImprecationArea(bool enter)
    {
        LGG.active = enter;

        if (enter)
        {
            IEnumerator chrCo = ChromAberRepeatCO();
            StartCoroutine(chrCo);

            Action action = null;
            action += () =>
            {
                OnEnteredOrExitImprecationArea(false);
                chromaticAberration.active = false;
                StopCoroutine(chrCo);
                EventManager.StopListening(Global.EnterNextMap, action);
            };

            EventManager.StartListening(Global.EnterNextMap, action);
        }
    }

    private IEnumerator ChromAberRepeatCO()
    {
        bool up = true;
        chromaticAberration.active = true;
        while(true)
        {
            chromaticAberration.DOChromIntensity(up ? 0.2f : 0f, 0.5f, true);
            yield return new WaitForSecondsRealtime(0.6f);
            up = !up;
        }
    }
}
