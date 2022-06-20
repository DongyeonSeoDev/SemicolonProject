using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTitlePopUpObject : TitleObject
{
    [SerializeField]
    private bool canSetMenuIdxFalseOnEnable = true;

    public TitlePopUpObject enableObj = null;

    public SoundBox onEnableSoundBox = null;
    public SoundBox onDisableSoundBox = null;

    public override void DoWork()
    {
        base.DoWork();

        if (!enableObj.gameObject.activeSelf)
        {
            Enable();
        }
    }

    public void Enable()
    {
        if (enableObj != null)
        {
            if(onEnableSoundBox != null)
            {
                SoundManager.Instance.PlaySoundBox(onEnableSoundBox);
            }

            if(canSetMenuIdxFalseOnEnable)
            {
                menu.canSetMenuIdx = false;
            }

            enableObj.lastEnabler = this;

            enableObj.gameObject.SetActive(true);
        }
    }
    public void Disable()
    {
        if (enableObj != null)
        {
            if (onDisableSoundBox != null)
            {
                SoundManager.Instance.PlaySoundBox(onDisableSoundBox);
            }

            if (canSetMenuIdxFalseOnEnable)
            {
                menu.canSetMenuIdx = true;
            }

            enableObj.gameObject.SetActive(false);
        }
    }
}
