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

    public void SetData(bool isBackUp)
    {
        WriteData(isBackUp);
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

    private void WriteData(bool isBackUp)
    {
        string data = HowToWrite();

        string backUpPath = Path.Combine(Application.dataPath, "Resources", path + "_BackUp" + System.DateTime.Now.ToString("_yyyyMMdd_HHmmss") + ".csv");
        string savePath = Path.Combine(Application.dataPath, "Resources", path + ".csv");

        if (isBackUp)
        {
            if (File.Exists(savePath) && !File.Exists(backUpPath))
            {
                File.Move(savePath, backUpPath);
                Debug.Log(backUpPath + " �� �����ִ� �����Ͱ� ��� �Ǿ����ϴ�.");
            }
        }

        File.WriteAllText(savePath, data, Encoding.UTF8);
        Debug.Log(savePath + " �� �����Ͱ� ���� �Ǿ����ϴ�.");
    }
}
