using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTitlePopUpObject : MonoBehaviour
{
    public TitlePopUpObject enableObj = null;

    public SoundBox onEnableSoundBox = null;
    public SoundBox onDisableSoundBox = null;

    public virtual void DoWork()
    {
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
            enableObj.lastEnabler = this;

            enableObj.gameObject.SetActive(true);
        }
    }
    public void Disable()
    {
        if (enableObj != null)
        {
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
