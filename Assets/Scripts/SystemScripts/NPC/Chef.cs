using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : NPC    //�丮�� ��ũ��Ʈ          //�� - OpenGameArt_AntumDeluge ,  
{
    [SerializeField] private List<Food> canFoodList = new List<Food>();  //���� �� �ִ� ���� ����Ʈ
    public List<Food> CanFoodList { get { return canFoodList; } }

    private void Start()
    {
        SetUI(true); //Test
    }
}
