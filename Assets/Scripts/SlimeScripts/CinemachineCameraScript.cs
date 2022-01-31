using UnityEngine;
using Cinemachine;

public class CinemachineCameraScript : MonoSingleton<CinemachineCameraScript>
{
    private CinemachineVirtualCamera cinemachine = null;
    private CinemachineConfiner cinemachineConfiner = null;

    private Collider2D boundingCollider = null;

    void Start()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        cinemachineConfiner = GetComponent<CinemachineConfiner>();
        boundingCollider = GameObject.FindGameObjectWithTag("CameraLimit").GetComponent<Collider2D>();

        EventManager.StartListening("PlayerDead", () =>
        {
            SetCinemachineFollow(SlimeGameManager.Instance.Player.transform);
            SetCinemachineConfiner(boundingCollider);
        });

        SetCinemachineFollow(SlimeGameManager.Instance.Player.transform);
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
}
