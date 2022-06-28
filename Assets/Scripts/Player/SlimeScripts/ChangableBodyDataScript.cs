using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangableBodyDataScript : MonoBehaviour
{
    [Serializable]
    public struct ChangableBodyNameData
    {
        public string name;
        public Enemy.EnemyType bodyId;
        //public Sprite sprite;
    }

    [SerializeField]
    private List<ChangableBodyNameData> changableBodyNames = new List<ChangableBodyNameData>();

    private Dictionary<string, ChangableBodyNameData> changableBodyNameDict = new Dictionary<string, ChangableBodyNameData>();
    public Dictionary<string, ChangableBodyNameData> ChangableBodyNameDict
    {
        get { return changableBodyNameDict; }
    }

    private void Awake()
    {
        changableBodyNameDict.Clear();

        changableBodyNames.ForEach(x => changableBodyNameDict.Add(x.bodyId.ToString(), x));
    }
}
