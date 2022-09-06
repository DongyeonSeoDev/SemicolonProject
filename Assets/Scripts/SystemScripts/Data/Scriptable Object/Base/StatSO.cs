using UnityEngine;

public abstract class StatSO : ScriptableObject
{
    public ushort statId;  //고유 아이디
    public int maxStatLv; // 스탯 최대 레벨
    public float upStatValue; // 특정 스탯 포인트를 투자했을 때 오를 스탯의 값
    public string statName; //이 스탯의 이름 (예 : 체력, 공격력, 방어력) 
    public Sprite statSpr;  //스탯 아이콘
}
