using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : NPC    //요리사 스크립트          //쥐 - OpenGameArt_AntumDeluge ,  
{
    [SerializeField] private List<Food> canFoodList = new List<Food>();  //만들 수 있는 음식 리스트
    public List<Food> CanFoodList { get { return canFoodList; } }

    private void Start()
    {
        SetUI(true); //Test
    }
}
