using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameTitle : TitleMenuObject
{
    public override void DoWork()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
