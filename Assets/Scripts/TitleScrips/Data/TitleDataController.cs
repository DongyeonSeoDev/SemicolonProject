using System;
using System.IO;
using UnityEngine;

public class TitleDataController : MonoBehaviour
{
    public SaveSlot[] saveSlots;

    private void Awake()
    {
        SaveFileStream.LoadOption();
        for(int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i].Init();
        }
    }

    public void Save()
    {
        SaveFileStream.SaveOption();
    }

    #region OnApplication
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            Save();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    #endregion
}
