using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootDirectionControl : MonoBehaviour
{
    [Serializable]
    public struct DirectionDataStruct
    {
        public List<float> dataList;
    }

    [SerializeField]
    private List<DirectionDataStruct> directionList = new List<DirectionDataStruct>(); // StatLv마다 총알이 날아갈 위치. // MousePosDirection 기준 Local값, 
                                                                            // 마우스 위치로 날아가게 하려면 모두 1값을 넣자
    public List<DirectionDataStruct> DirectionList
    {
        get { return directionList; }
    }

}
