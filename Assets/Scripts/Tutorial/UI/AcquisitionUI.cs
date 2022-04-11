using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionUI : MonoBehaviour
{
    public UIType uiType;
    [HideInInspector] public bool on = false;
    private CanvasGroup cvsg;

    private void Awake()
    {
        cvsg = GetComponent<CanvasGroup>();
        OnUIVisible(false);
    }

    private void OnUIVisible(bool on)
    {
        cvsg.alpha = on ? 1 : 0;
        cvsg.interactable = on;
        cvsg.blocksRaycasts = on;
    }
}
