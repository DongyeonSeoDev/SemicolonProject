using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct AStarData
{
    public int f;
    public int g;
    public int h;
}

public static class FindPath
{
    private const int MAX_F_VALUE = 100000;

    private static Stack<Vector2Int> positionList = new Stack<Vector2Int>();
    private static Stack<int> addMoveValueList = new Stack<int>();

    private static bool[] isWall;

    private static Vector2Int currentPosition;
    private static Vector2Int targetPosition;
    private static AStarData currentAStarData;
    private static AStarData targetAStarData;

    private static int currentMoveValue;
    private static int targetMoveValue;

    public static StageData SetStageData(Tilemap[] tilemap, Vector2Int limitMinPosition, Vector2Int limitMaxPosition, string name)
    {
        StageData stageData = new StageData();

        ResetStageData(stageData, limitMinPosition, limitMaxPosition, name);

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

        return stageData;
    }

    private static void ResetStageData(StageData stageData, Vector2Int limitMinPosition, Vector2Int limitMaxPosition, string name)
    {
        stageData.stageName = name;

        stageData.limitMinPosition = limitMinPosition;
        stageData.limitMaxPosition = limitMaxPosition;

        stageData.offsetX = -limitMinPosition.x;
        stageData.offsetY = -limitMinPosition.y;

        stageData.stageWidth = limitMaxPosition.x - limitMinPosition.x + 1;

        stageData.isWall = new bool[stageData.stageWidth * (limitMaxPosition.y - limitMinPosition.y + 1)];
    }

    private static int GetBoolPosition(StageData stageData, int x, int y) => (x + stageData.offsetX) + (stageData.stageWidth * (y + stageData.offsetY));

    private static bool IsPass(StageData stageData, int x, int y)
    {
        if (stageData.limitMinPosition.x > x || stageData.limitMinPosition.y > y
            || stageData.limitMaxPosition.x < x || stageData.limitMaxPosition.y < y)
        {
            return false;
        }

        return !isWall[GetBoolPosition(stageData, x, y)];
    }

    private static AStarData GetAStarData(int gValue, int hValue) => new AStarData { g = gValue, h = hValue, f = gValue + hValue };
    private static int GetGValue(int currentPositionX, int currentPositionY, int targetPositionX, int targetPositionY) => (Mathf.Abs(currentPositionX - targetPositionX) + Mathf.Abs(currentPositionY - targetPositionY)) * 10;

    private static void GetDirectionCheck(StageData stageData, int directionX, int directionY, Vector2Int endPosition, int moveValue)
    {
        if (IsPass(stageData, directionX, directionY))
        {
            targetAStarData = GetAStarData(GetGValue(directionX, directionY, endPosition.x, endPosition.y), currentMoveValue + moveValue);

            if (targetAStarData.f < currentAStarData.f)
            {
                targetMoveValue = moveValue;
                currentAStarData = targetAStarData;
                targetPosition = new Vector2Int(directionX, directionY);
            }
        }
    }

    public static Vector2Int NextPosition(StageData stageData, Vector2Int startPosition, Vector2Int endPosition)
    {
        currentPosition = startPosition;
        currentMoveValue = 0;
        currentAStarData.f = MAX_F_VALUE;

        isWall = new bool[stageData.isWall.Length];

        Array.Copy(stageData.isWall, isWall, stageData.isWall.Length);

        int count = 0;

        while (count < 1000)
        {
            count++;
            currentAStarData.f = MAX_F_VALUE;

            if (Vector2Int.Distance(currentPosition, endPosition) < 1.5f)
            {
                while (positionList.Count > 1)
                {
                    positionList.Pop();
                }

                return positionList.Pop();
            }

            #region 방향 확인
            GetDirectionCheck(stageData, currentPosition.x + 1, currentPosition.y, endPosition, 10);
            GetDirectionCheck(stageData, currentPosition.x - 1, currentPosition.y, endPosition, 10);
            GetDirectionCheck(stageData, currentPosition.x, currentPosition.y + 1, endPosition, 10);
            GetDirectionCheck(stageData, currentPosition.x, currentPosition.y - 1, endPosition, 10);
            GetDirectionCheck(stageData, currentPosition.x + 1, currentPosition.y + 1, endPosition, 14);
            GetDirectionCheck(stageData, currentPosition.x - 1, currentPosition.y + 1, endPosition, 14);
            GetDirectionCheck(stageData, currentPosition.x + 1, currentPosition.y - 1, endPosition, 14);
            GetDirectionCheck(stageData, currentPosition.x - 1, currentPosition.y - 1, endPosition, 14);
            #endregion

            if (currentAStarData.f == MAX_F_VALUE)
            {
                currentPosition = positionList.Pop();
                currentMoveValue -= addMoveValueList.Pop();
            }
            else
            {
                positionList.Push(targetPosition);
                addMoveValueList.Push(targetMoveValue);

                currentPosition = targetPosition;
                currentMoveValue += targetMoveValue;

                isWall[GetBoolPosition(stageData, currentPosition.x, currentPosition.y)] = true;
            }
        }

        return startPosition;
    }
}