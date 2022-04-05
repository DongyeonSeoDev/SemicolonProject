using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffStateDataSO", menuName = "Scriptable Object/BuffState Data", order = int.MaxValue)]
public class BuffStateDataSO : ScriptableObject
{
    public StateAbnormality imprecation;
    public BuffType buff;
    public string Id => imprecation != StateAbnormality.None ? imprecation.ToString() : buff.ToString();
    public bool IsBuff => imprecation != StateAbnormality.None ? false : true;

    public int duration;
    public Sprite sprite;
    public string stateName;
    [TextArea] public string explanation;
}
