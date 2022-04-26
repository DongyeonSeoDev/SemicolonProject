using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class UtilEditor
{
    public static void PauseEditor()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPaused = true;
        }
    }

    public static void ResumeEditor()
    {
        EditorApplication.isPaused = false;
    }
}
