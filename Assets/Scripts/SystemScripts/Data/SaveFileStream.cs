using System.IO;
using UnityEngine;

public static class SaveFileStream
{
    public const string EternalOptionSaveFileName = "EtnOptSaveFile1";
    private static EternalOptionData saveOptionData;
    public static EternalOptionData SaveOptionData => saveOptionData;


    public static string currentSaveFileName;


    public static void Delete(string path, bool onlyFileName)
    {
        if (onlyFileName)
            path = path.PersistentDataPath();

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Delete Save File : " + path);
        }
        else
        {
            Debug.Log("Not Exist Path : " + path);
        }
    }

    public static void Save(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public static void LoadOption()
    {
        saveOptionData = new EternalOptionData();
        string path = EternalOptionSaveFileName.PersistentDataPath();
        if(File.Exists(path))
        {
            saveOptionData = JsonUtility.FromJson<EternalOptionData>(File.ReadAllText(path));
        }
    }

    public static void SaveOption()
    {
        Save(EternalOptionSaveFileName.PersistentDataPath(), JsonUtility.ToJson(saveOptionData));
    }
}
