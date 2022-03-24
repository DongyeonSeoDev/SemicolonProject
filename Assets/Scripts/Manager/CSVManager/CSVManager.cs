using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class CSVManager
{
    private bool isRead = false;

    protected abstract string path { get; }

    protected abstract void HowToRead(string[] data);
    protected abstract string HowToWrite();

    public void GetData()
    {
        if (!isRead)
        {
            ReadData();
        }
    }

    public void SetData()
    {
        WriteData();
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

    private void WriteData()
    {
        string data = HowToWrite();
        File.WriteAllText(Path.Combine(Application.dataPath, "Resources", path + "TestSave.csv"), data, Encoding.UTF8);
    }
}
