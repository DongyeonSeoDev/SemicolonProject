using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class CSVManager
{
    protected Dictionary<string, string> csvData = new Dictionary<string, string>();

    private bool isRead = false;

    protected abstract string path { get; }

    protected virtual void HowToRead(string[] data)
    {
        csvData.Add(data[0], data[1]);
    }

    protected virtual void HowToWrite(StreamWriter streamWriter, KeyValuePair<string, string> data)
    {
        streamWriter.WriteLine(data.Key + ',' + data.Value);
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

    public void SetData(string key, string value)
    {
        if (!isRead)
        {
            ReadData();
        }

        if (!csvData.ContainsKey(key))
        {
            csvData.Add(key, value);
        }
        else
        {
            csvData[key] = value;
        }

        WriteData();
    }

    private void ReadData()
    {
        isRead = true;

        using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
        {
            while (!streamReader.EndOfStream)
            {
                HowToRead(streamReader.ReadLine().Split(','));
            }
        }
    }

    private void WriteData()
    {
        using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
        {
            foreach (KeyValuePair<string, string> data in csvData)
            {
                HowToWrite(streamWriter, data);
            }
        }
    }
}
