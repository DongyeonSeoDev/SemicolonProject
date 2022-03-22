using System.Collections.Generic;
using UnityEngine;

public abstract class CSVManager
{
    private bool isRead = false;

    protected abstract string path { get; }

    protected abstract void HowToRead(string[] data);

    public void GetData()
    {
        if (!isRead)
        {
            ReadData();
        }
    }

    private void ReadData()
    {
        isRead = true;

        string[] datas = Resources.Load(path).ToString().Split('\n');

        foreach (string data in datas)
        {
            HowToRead(data.Split(','));
        }
    }
}
