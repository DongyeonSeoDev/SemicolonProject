using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ChangeBodyData
{
    public string bodyName;
    public Enemy.EnemyType bodyId;
    public GameObject body;
    public EternalStat additionalBodyStat; // 변신 후의 플레이어의 Additional스탯, (이해도 100% 기준)
    public Sprite bodyImg;
    public ItemSO dropItem;
    [TextArea] public string bodyExplanation;
    [TextArea] public string featureExplanation;
    [TextArea] public string hint;
}
public class ChangableBodyDataScript : MonoBehaviour
{
    [SerializeField]
    private List<ChangeBodyData> changableBodyList = new List<ChangeBodyData>();
    public List<ChangeBodyData> ChangableBodyList
    {
        get { return changableBodyList; }
    }

    private Dictionary<string, string> changableBodyNameDict = new Dictionary<string, string>();
    public Dictionary<string, string> ChangableBodyNameDict
    {
        get { return changableBodyNameDict; }
    }

    private void Awake()
    {
        changableBodyNameDict.Clear();

        changableBodyList.ForEach(x => changableBodyNameDict.Add(x.bodyId.ToString(), x.bodyName));
    }
}
