using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutSceneCinemachineCameraScript : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachine = null;

    void Start()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetOthographicSize(float size)
    {
        cinemachine.m_Lens.OrthographicSize = size;
    }
}
