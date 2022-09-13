using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    public Toggle atkCamShakeToggle;
    public Toggle hitCamShakeToggle;
    public Toggle timeFreezeToggle;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;

        Option op = gm.savedData.option;
        atkCamShakeToggle.isOn = op.IsAtkShakeCamera;
        hitCamShakeToggle.isOn = op.IsHitShakeCam;
        timeFreezeToggle.isOn = op.IsHitTimeFreeze;
    }

    public void SetAtkCamShake()
    {
        gm.savedData.option.IsAtkShakeCamera = atkCamShakeToggle.isOn;
    }
    public void SetHitCamShake()
    {
        gm.savedData.option.IsHitShakeCam = hitCamShakeToggle.isOn;
    }
    public void SetTimeFreeze()
    {
        gm.savedData.option.IsHitTimeFreeze = timeFreezeToggle.isOn;
    }
}
