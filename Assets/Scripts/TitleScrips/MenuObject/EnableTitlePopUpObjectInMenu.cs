using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTitlePopUpObjectInMenu : TitleMenuObject
{
    [SerializeField]
    private bool canSetMenuIdxFalseOnEnable = true;

    public TitlePopUpObjectInMenu enableObj = null;

    public SoundBox onEnableSoundBox = null;
    public SoundBox onDisableSoundBox = null;

    public override void DoWork()
    {
        base.DoWork();

        if (!enableObj.gameObject.activeSelf)
        {
            PlayOnEnableSoundBox();
            Enable();
        }
    }

    public void Enable()
    {
        if (enableObj != null)
        {
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
            if (canSetMenuIdxFalseOnEnable)
            {
                menu.canSetMenuIdx = true;
            }

            enableObj.gameObject.SetActive(false);
        }
    }
    public void PlayOnEnableSoundBox()
    {
        if (onEnableSoundBox != null)
        {
            SoundManager.Instance.PlaySoundBox(onEnableSoundBox);
        }
    }
    public void PlayOnDisableSoundBox()
    {
        if (onDisableSoundBox != null)
        {
            SoundManager.Instance.PlaySoundBox(onDisableSoundBox);
        }
    }
}
