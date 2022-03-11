using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVManager
{
    private static Dictionary<string, string> csvData = new Dictionary<string, string>();
    private static string path = Path.Combine(Application.dataPath, "Data", "Test.csv");
    private static bool isRead = false;

    private static void ReadData()
    {
        isRead = true;

        using (StreamReader streamReader = new StreamReader(path))
        {
            while (!streamReader.EndOfStream)
            {
                string[] data = streamReader.ReadLine().Split(',');
                csvData.Add(data[0], data[1]);
            }
        }
    }

    private static void WriteData()
    {
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            foreach (KeyValuePair<string, string> data in csvData)
            {
                streamWriter.WriteLine(data.Key + ',' + data.Value);
            }
        }
    }

    public static void SetData(string key, string value)
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

    public static string GetData(string key)
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
}
