using System;
using System.IO;
using UnityEngine;

public class TitleDataController : MonoBehaviour
{
    private void Awake()
    {
        SaveFileStream.LoadOption();
    }
    private void OnApplicationQuit()
    {
        SaveFileStream.SaveOption();
    }
    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            SaveFileStream.SaveOption();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveFileStream.SaveOption();
        }
    }
}
