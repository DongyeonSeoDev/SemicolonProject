using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjdButton : PopUpsButtons
{
    [SerializeField]
    protected TitlePopUpObject titlePopUpObject = null;

    public ChildPopUpPannel enableObj = null;

    public SoundBox onEnableSoundBox = null;
    public SoundBox onDisableSoundBox = null;

    public override void OnClick()
    {
        base.OnClick();

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

            titlePopUpObject.canFadeOut = false;
        }
    }
    public void Disable()
    {
        if (enableObj != null)
        {
            enableObj.gameObject.SetActive(false);

            titlePopUpObject.canFadeOut = true;
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
