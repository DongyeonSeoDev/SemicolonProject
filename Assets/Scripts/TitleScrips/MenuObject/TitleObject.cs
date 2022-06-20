using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TitleObject : MonoBehaviour 
{
    public TitleMenu menu = null;
    public int curTitleObjIdx = 0;

    public virtual void Start()
    {
        menu = transform.parent.GetComponent<TitleMenu>();
    }
    public virtual void DoWork()
    {

    }
    public void SetMenuIdxThis()
    {
        menu.SetMenuIdx(curTitleObjIdx);
    }
}
