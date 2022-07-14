using System.Collections.Generic;
using UnityEngine;

public class SubtitleDataManager : SingletonClass<SubtitleDataManager>
{
    private Dictionary<string, SubtitleData> slimeDialogDict;

    private SlimeDialogDataSO slimeDialogDataSO;  //���¿� �ִ°� �ҷ����°Ŷ� ���Ѿ�� ������� (�̱��� Ŭ������)

    public void Init()
    {
        if (slimeDialogDict == null)  //�̱��� Ŭ������ �� �Ѿ�� �� ȣ���ص� null�� �ƴ϶� �� �Ѱ���
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

        Debug.LogWarning("�������� �ʴ� ��� Ű : " + key);
        return new SubtitleData();
    }
}
