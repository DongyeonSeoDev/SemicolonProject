using UnityEngine;
using UnityEngine.Tilemaps;

public static class FindPath
{
    public static void SetStageData(StageData stageData, Tilemap[] tilemap, Vector2Int limitMinPosition, Vector2Int limitMaxPosition)
    {
        ResetStageData(stageData, limitMinPosition, limitMaxPosition);

        for (int i = 0; i < tilemap.Length; i++)
        {
            BoundsInt cellBounds = tilemap[i].cellBounds;

            for (int j = cellBounds.min.x; j <= cellBounds.max.x; j++)
            {
                for (int k = cellBounds.min.y; k <= cellBounds.max.y; k++)
                {
                    if ((tilemap[i].GetTile(new Vector3Int(j, k, 0)) != null) || (tilemap[i].GetTile(new Vector3Int(j - 1, k, 0)) != null) || (tilemap[i].GetTile(new Vector3Int(j, k - 1, 0)) != null) || (tilemap[i].GetTile(new Vector3Int(j - 1, k - 1, 0)) != null))
                    {
                        stageData.isWall[GetBoolPosition(stageData, j, k)] = true;
                    }
                }
            }
        }
    }

    private static void ResetStageData(StageData stageData, Vector2Int limitMinPosition, Vector2Int limitMaxPosition)
    {
        stageData.limitMinPosition = limitMinPosition;
        stageData.limitMaxPosition = limitMaxPosition;

        stageData.offsetX = -limitMinPosition.x;
        stageData.offsetY = -limitMinPosition.y;

        stageData.stageWidth = limitMaxPosition.x - limitMinPosition.x + 1;

        stageData.isWall = new bool[stageData.stageWidth * (limitMaxPosition.y - limitMinPosition.y + 1)];
    }

    private static int GetBoolPosition(StageData stageData, int x, int y) => (x + stageData.offsetX) + (stageData.stageWidth * (y + stageData.offsetY));
}