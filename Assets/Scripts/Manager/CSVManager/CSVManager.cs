using System.Collections.Generic;
using UnityEngine;

public abstract class CSVManager
{
    protected Dictionary<string, string> csvData = new Dictionary<string, string>();

    private bool isRead = false;

    protected abstract string path { get; }

    protected virtual void HowToRead(string[] data)
    {
        if (data.Length < 2)
        {
            return;
        }

        csvData.Add(data[0].Trim(), data[1].Trim());
    }

    public string GetData(string key)
    {
        if (!isRead)
        {
            ReadData();
        }

        if (csvData.ContainsKey(key))
        {
            return csvData[key];
        }

        return null;
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
