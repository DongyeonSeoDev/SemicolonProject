using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTitlePopUpObject : TitleObject
{
    public TitlePopUpObject enableObj = null;

    public override void DoWork()
    {
        base.DoWork();

        if(!enableObj.gameObject.activeSelf)
        {
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
}
