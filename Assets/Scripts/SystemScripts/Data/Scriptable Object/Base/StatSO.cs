using UnityEngine;

public abstract class StatSO : ScriptableObject
{
    public ushort statId;
    public string statName; //이 스탯의 이름 (예 : 체력, 공격력, 방어력) 
}
