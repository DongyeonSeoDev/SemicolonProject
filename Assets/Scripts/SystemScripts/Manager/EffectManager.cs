using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;

public class EffectManager : MonoBehaviour
{
    private void Start()
    {
        Global.AddMonoAction(Global.AcquisitionItem, item => ((Item)item).FollowEffect());
    }
}
