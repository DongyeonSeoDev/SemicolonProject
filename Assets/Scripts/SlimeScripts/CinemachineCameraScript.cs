using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Water;

public class CinemachineCameraScript : MonoSingleton<CinemachineCameraScript>
{
    private CinemachineVirtualCamera cinemachine = null;

    void Start()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();

        SetCinemachineFollow(SlimeGameManager.Instance.Player.transform);
    }

    public void SetCinemachineFollow(Transform target)
    {
        cinemachine.Follow = target;
    }
}
