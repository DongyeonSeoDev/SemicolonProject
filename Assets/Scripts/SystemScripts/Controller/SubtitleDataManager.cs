using System.Collections.Generic;
using UnityEngine;

public class SubtitleDataManager : SingletonClass<SubtitleDataManager>
{
    private Dictionary<string, SubtitleData> slimeDialogDict;

    private SlimeDialogDataSO slimeDialogDataSO;

    public void Init()
    {
        slimeDialogDict = new Dictionary<string, SubtitleData>();
        slimeDialogDataSO = Resources.Load<SlimeDialogDataSO>("System/Dialog/SlimeDialogDataSO");

        for(int i = 0; i < slimeDialogDataSO.slimeDialogDatas.Count; i++)
        {
            slimeDialogDict.Add(slimeDialogDataSO.slimeDialogDatas[i].key, slimeDialogDataSO.slimeDialogDatas[i].subtitleData);
        }
    }

    public SubtitleData GetSubtitle(string key)
    {
        if(slimeDialogDict.ContainsKey(key))
        {
            return slimeDialogDict[key];
        }

        Debug.LogWarning("존재하지 않는 대사 키 : " + key);
        return new SubtitleData();
    }
}
