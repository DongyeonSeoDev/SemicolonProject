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
            Enable();
        }
    }
    public void Enable()
    {
        if (enableObj != null)
        {
            if (onEnableSoundBox != null)
            {
                SoundManager.Instance.PlaySoundBox(onEnableSoundBox);
            }

            enableObj.lastEnabler = this;

            enableObj.gameObject.SetActive(true);

            titlePopUpObject.canFadeOut = false;
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

            enableObj.gameObject.SetActive(false);

            titlePopUpObject.canFadeOut = true;
        }
    }
}
