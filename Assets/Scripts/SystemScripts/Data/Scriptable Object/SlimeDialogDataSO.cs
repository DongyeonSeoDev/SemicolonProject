using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeDialogDataSO", menuName = "Scriptable Object/SlimeDialogDataSO", order = int.MaxValue)]
public class SlimeDialogDataSO : ScriptableObject
{
    [System.Serializable]
    public class DialogData
    {
        public string key;
        public SubtitleData subtitleData;
    }

    public List<DialogData> slimeDialogDatas;
}
