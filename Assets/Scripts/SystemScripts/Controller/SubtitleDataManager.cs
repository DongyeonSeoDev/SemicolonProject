using System.Collections.Generic;
using UnityEngine;

public class SubtitleDataManager : SingletonClass<SubtitleDataManager>
{
    private Dictionary<string, SubtitleData> slimeDialogDict;

    private SlimeDialogDataSO slimeDialogDataSO;  //에셋에 있는거 불러오는거라 씬넘어가도 살아있음 (싱글톤 클래스라)

    public void Init()
    {
        if (slimeDialogDict == null)  //싱글톤 클래스라서 씬 넘어가고 또 호출해도 null이 아니라서 걍 넘겨줌
        {
            slimeDialogDict = new Dictionary<string, SubtitleData>();
            slimeDialogDataSO = Resources.Load<SlimeDialogDataSO>("System/Dialog/SlimeDialogDataSO");

            for (int i = 0; i < slimeDialogDataSO.slimeDialogDatas.Count; i++)
            {
                slimeDialogDict.Add(slimeDialogDataSO.slimeDialogDatas[i].key, slimeDialogDataSO.slimeDialogDatas[i].subtitleData);
            }
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
