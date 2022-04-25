using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyHPBar : MonoBehaviour
{
    private Canvas canvas;
    protected virtual void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Util.MainCam;
    }
}
