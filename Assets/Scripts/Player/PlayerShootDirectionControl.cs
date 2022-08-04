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
    private List<DirectionDataStruct> directionList = new List<DirectionDataStruct>(); // StatLv���� �Ѿ��� ���ư� ��ġ. // MousePosDirection ���� Local��, 
                                                                            // ���콺 ��ġ�� ���ư��� �Ϸ��� ��� 1���� ����
    public List<DirectionDataStruct> DirectionList
    {
        get { return directionList; }
    }

}
