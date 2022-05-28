using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FindPath : MonoBehaviour
{
    public Tilemap[] tilemap;

    public Vector2Int limitMinPosition;
    public Vector2Int limitMaxPosition;

    public bool[] stageData;
    public int stageWidth;
    public int offsetX;
    public int offsetY;

    private void SetStageData()
    {
        for (int i = 0; i < tilemap.Length; i++)
        {
            BoundsInt cellBounds = tilemap[i].cellBounds;

            for (int j = cellBounds.min.x; j <= cellBounds.max.x; j++)
            {
                for (int k = cellBounds.min.y; k <= cellBounds.max.y; k++)
                {
                    if ((tilemap[i].GetTile(new Vector3Int(j, k, 0)) != null) || (tilemap[i].GetTile(new Vector3Int(j - 1, k, 0)) != null) || (tilemap[i].GetTile(new Vector3Int(j, k - 1, 0)) != null) || (tilemap[i].GetTile(new Vector3Int(j - 1, k - 1, 0)) != null))
                    {
                        stageData[GetBoolPosition(j, k)] = true;
                    }
                }
            }
        }
    }

    private void ResetStageData()
    {
        offsetX = -limitMinPosition.x;
        offsetY = -limitMinPosition.y;

        stageWidth = limitMaxPosition.x - limitMinPosition.x + 1;

        stageData = new bool[stageWidth * (limitMaxPosition.y - limitMinPosition.y + 1)];
    }

    public bool IsPassable(int x, int y) => stageData[GetBoolPosition(x, y)];
    private int GetBoolPosition(int x, int y) => (x + offsetX) + (stageWidth * (y + offsetY));
}