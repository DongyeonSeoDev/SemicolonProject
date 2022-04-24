using UnityEngine;
using Cinemachine;
using FkTweening;
using System.Collections;

public class CinemachineCameraScript : MonoSingleton<CinemachineCameraScript>
{
    private CinemachineVirtualCamera cinemachine = null;
    private CinemachineConfiner cinemachineConfiner = null;

    private CinemachineBasicMultiChannelPerlin cinemachineNoise;

    public Collider2D boundingCollider = null;

    void Start()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        cinemachineConfiner = GetComponent<CinemachineConfiner>();
        boundingCollider = GameObject.FindGameObjectWithTag("CameraLimit").GetComponent<Collider2D>();
        cinemachineNoise = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        EventManager.StartListening("PlayerDead", () =>
        {
            SetCinemachineFollow(SlimeGameManager.Instance.CurrentPlayerBody.transform);
            SetCinemachineConfiner(boundingCollider);
        });

        SetCinemachineFollow(SlimeGameManager.Instance.CurrentPlayerBody.transform);
        SetCinemachineConfiner(boundingCollider);
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

    private IEnumerator ShakeCo(float duration)
    {
        yield return new WaitForSeconds(duration);
        cinemachineNoise.m_AmplitudeGain = 0;
        cinemachineNoise.m_FrequencyGain = 0;
    }
}
