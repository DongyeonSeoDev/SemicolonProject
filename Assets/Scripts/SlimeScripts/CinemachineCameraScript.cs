using UnityEngine;
using Cinemachine;

public class CinemachineCameraScript : MonoSingleton<CinemachineCameraScript>
{
    private CinemachineVirtualCamera cinemachine = null;
    private CinemachineConfiner cinemachineConfiner = null;

    private CinemachineBasicMultiChannelPerlin noise;

    public Collider2D boundingCollider = null;

    void Start()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        cinemachineConfiner = GetComponent<CinemachineConfiner>();
        boundingCollider = GameObject.FindGameObjectWithTag("CameraLimit").GetComponent<Collider2D>();
        noise = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

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

    public void Shake()
    {
        
    }
}
