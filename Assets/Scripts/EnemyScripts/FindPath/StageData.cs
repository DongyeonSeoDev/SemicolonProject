using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonParse<T>
{
    public JsonParse(List<T> data)
    {
        jsonData = data;
    }

    public List<T> jsonData;
}

[System.Serializable]
public class StageData
{
    public bool[] isWall;

    public Vector2Int limitMinPosition;
    public Vector2Int limitMaxPosition;

    public int stageWidth;
    public int offsetX;
    public int offsetY;

    public string stageName;
}
