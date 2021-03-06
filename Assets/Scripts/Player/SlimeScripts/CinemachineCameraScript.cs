using UnityEngine;
using Cinemachine;
using FkTweening;
using System.Collections;
using System;

public class CinemachineCameraScript : MonoSingleton<CinemachineCameraScript>
{
    private CinemachineVirtualCamera cinemachine = null;
    private CinemachineConfiner cinemachineConfiner = null;

    private CinemachineBasicMultiChannelPerlin cinemachineNoise;

    //public Collider2D boundingCollider = null;

    private readonly float defaultOrthographicSize = 8.5f;

    private void Start()
    { 
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        cinemachineConfiner = GetComponent<CinemachineConfiner>();
        //boundingCollider = GameObject.FindGameObjectWithTag("CameraLimit").GetComponent<Collider2D>();
        cinemachineNoise = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachine.m_Lens.OrthographicSize = defaultOrthographicSize;

        EventManager.StartListening("PlayerDead", () =>
        {
            SetCinemachineFollow(SlimeGameManager.Instance.CurrentPlayerBody.transform);
            //SetCinemachineConfiner(boundingCollider);
        });

        SetCinemachineFollow(SlimeGameManager.Instance.CurrentPlayerBody.transform);
        //SetCinemachineConfiner(boundingCollider);

        cinemachineNoise.m_AmplitudeGain = 0;
        cinemachineNoise.m_FrequencyGain = 0;
    }

    public void SetCinemachineFollow(Transform target)
    {
        cinemachine.Follow = target;
    }

    public void SetCinemachineConfiner(Collider2D collider)
    {
        cinemachineConfiner.m_BoundingShape2D = collider;
    }

    public void Shake(float strength, float frequency, float duration)
    {
        cinemachineNoise.m_AmplitudeGain = strength;
        cinemachineNoise.m_FrequencyGain = frequency;
        DOUtil.ExecuteTweening("CVCam_Shake_" + name, ShakeCo(duration), this);
    }

    public void Shake(CamShakeData shakeData)
    {
        cinemachineNoise.m_AmplitudeGain = shakeData.strength;
        cinemachineNoise.m_FrequencyGain = shakeData.frequency;
        DOUtil.ExecuteTweening("CVCam_Shake_" + name, ShakeCo(shakeData.duration), this);
    }

    public void DoOrthographicSize(float target, float duration, Action OnComplete = null)
    {
        DOUtil.StartCo("DoOrthographicSize " + name, CamOrthographicSizeCo(target, duration, OnComplete), this);
    }

    private IEnumerator ShakeCo(float duration)
    {
        yield return new WaitForSeconds(duration);
        cinemachineNoise.m_AmplitudeGain = 0;
        cinemachineNoise.m_FrequencyGain = 0;
    }

    private IEnumerator CamOrthographicSizeCo(float target, float duration, Action OnComplete)
    {
        float t = 0;
        while(t < duration)
        {
            yield return null;
            t += Time.deltaTime;
            cinemachine.m_Lens.OrthographicSize = Mathf.Lerp(cinemachine.m_Lens.OrthographicSize, target, t / duration);
        }
        cinemachine.m_Lens.OrthographicSize = target;
        OnComplete?.Invoke();
    }
}
