using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : TitleObject
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
