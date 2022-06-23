using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SaveFileStream
{
    #region 공용 데이터 관련
    public const string EternalOptionSaveFileName = "EtnOptSaveFile1";
    private static EternalOptionData saveOptionData;
    public static EternalOptionData SaveOptionData => saveOptionData;
    #endregion

    private static Dictionary<string, SaveData> saveDataDic = new Dictionary<string, SaveData>();

    public static string currentSaveFileName;

    public const string CryptoKey = "XHUooeUjJzMKdt";

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

    #region Game Data
    public static void LoadGameSaveData(string saveFileName)
    {
        if(!saveDataDic.ContainsKey(saveFileName))
        {
            string path = saveFileName.PersistentDataPath();
            if (File.Exists(path))
            {
                string json = Crypto.Decrypt(File.ReadAllText(path), CryptoKey);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                saveDataDic.Add(saveFileName, data);
            }
        }
    }

    public static SaveData GetSaveData(string saveFileName)
    {
        if (saveDataDic.ContainsKey(saveFileName))
        {
            return saveDataDic[saveFileName];
        }

        SaveData data = new SaveData();
        saveDataDic.Add(saveFileName, data);
        return data;
    }

    public static void DeleteGameSaveData(string saveFileName)
    {
        Delete(saveFileName, true);
        if(saveDataDic.ContainsKey(saveFileName))
        {
            saveDataDic.Remove(saveFileName);
        }
    }

    #endregion

    #region 공용 옵션 데이터 Save&Load
    public static void LoadOption()
    {
        if (saveOptionData == null)
        {
            saveOptionData = new EternalOptionData();
            string path = EternalOptionSaveFileName.PersistentDataPath();
            if (File.Exists(path))
            {
                saveOptionData = JsonUtility.FromJson<EternalOptionData>(File.ReadAllText(path));
            }
        }
    }

    public static void SaveOption()
    {
        Save(EternalOptionSaveFileName.PersistentDataPath(), JsonUtility.ToJson(saveOptionData));
    }
    #endregion
}
