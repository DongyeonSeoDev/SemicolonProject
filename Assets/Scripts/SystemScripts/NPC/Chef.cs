using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : NPC
{
    [SerializeField] private List<Food> canFoodList = new List<Food>();  //���� �� �ִ� ���� ����Ʈ
    public List<Food> CanFoodList { get { return CanFoodList; } }
}
