using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct AStarData
{
    public ushort moveValue;
    public ushort number;
    public int f;
    public int g;
    public int h; 
}

public static class FindPath
{
    private const int MAX_F_VALUE = 9999999;

    private static Stack<Vector2Int> positionList = new Stack<Vector2Int>();
    private static Stack<int> addMoveValueList = new Stack<int>();

    private static bool[] isWall;
    private static bool[] isEnemyPosition;
    private static AStarData[] astarData;

    private static Vector2Int currentPosition;
    private static Vector2Int targetPosition;
    private static AStarData currentAStarData;
    private static AStarData targetAStarData;

    private static int currentMoveValue;
    private static int targetMoveValue;

    private static ushort currentNumber = 0;

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

        for (int i = limitMinPosition.x; i <= limitMaxPosition.x; i++)
        {
            stageData.isWall[GetBoolPosition(stageData, i, limitMinPosition.y)] = true;
            stageData.isWall[GetBoolPosition(stageData, i, limitMaxPosition.y)] = true;
        }

        for (int i = limitMinPosition.y; i <= limitMaxPosition.y; i++)
        {
            stageData.isWall[GetBoolPosition(stageData, limitMinPosition.x, i)] = true;
            stageData.isWall[GetBoolPosition(stageData, limitMaxPosition.x, i)] = true;
        }

        return stageData;
    }

    public static void SetEnemyPosition(StageData stageData, Vector2Int position, bool value)
    {
        isEnemyPosition[GetBoolPosition(stageData, position.x, position.y)] = value;
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

        return !isWall[GetBoolPosition(stageData, x, y)] && !isEnemyPosition[GetBoolPosition(stageData, x, y)];
    }

    private static AStarData GetAStarData(StageData data, Vector2Int pos, int gValue, int hValue, ushort moveValue)
    {
        AStarData astar = new AStarData { g = gValue, h = hValue, f = gValue + hValue, number = currentNumber, moveValue = moveValue };

        if (astarData[GetBoolPosition(data, pos.x, pos.y)].f > -1 && astar.f > astarData[GetBoolPosition(data, pos.x, pos.y)].f)
        {
            return astarData[GetBoolPosition(data, pos.x, pos.y)];
        }

        astarData[GetBoolPosition(data, pos.x, pos.y)] = astar;

        return astar;
    }

    private static int GetGValue(int currentPositionX, int currentPositionY, int targetPositionX, int targetPositionY) => (Mathf.Abs(currentPositionX - targetPositionX) + Mathf.Abs(currentPositionY - targetPositionY)) * 10;

    private static void GetDirectionCheck(StageData stageData, int directionX, int directionY, Vector2Int endPosition, ushort moveValue)
    {
        if (IsPass(stageData, directionX, directionY))
        {
            targetAStarData = GetAStarData(stageData, new Vector2Int(directionX, directionY), GetGValue(directionX, directionY, endPosition.x, endPosition.y), currentMoveValue + moveValue, moveValue);

            if (targetAStarData.f < currentAStarData.f)
            {
                currentAStarData = targetAStarData;
                targetMoveValue = targetAStarData.moveValue;
                targetPosition = new Vector2Int(directionX, directionY);
            }
        }
    }

    public static Stack<Vector2Int> NextPosition(StageData stageData, Vector2Int startPosition, Vector2Int endPosition)
    {
        currentPosition = startPosition;
        currentMoveValue = 0;
        currentNumber = 1;
        currentAStarData.f = MAX_F_VALUE;

        isWall = new bool[stageData.isWall.Length];
        isEnemyPosition = new bool[stageData.isWall.Length];
        astarData = new AStarData[stageData.isWall.Length];

        Array.Copy(stageData.isWall, isWall, stageData.isWall.Length);

        isWall[GetBoolPosition(stageData, currentPosition.x, currentPosition.y)] = true;

        for (int i = 0; i < astarData.Length; i++)
        {
            astarData[i].number = 0;
            astarData[i].f = -1;
        }

        int count = 0;

        while (count < 1000)
        {
            count++;
            currentAStarData.f = MAX_F_VALUE;

            if (Vector2Int.Distance(currentPosition, endPosition) < 1.5f)
            {
                var positionCount = positionList.Count / 2;

                for (int i = 0; i < positionCount; i++)
                {
                    positionList.Pop();
                }

                Stack<Vector2Int> dataStack = new Stack<Vector2Int>();

                while (positionList.Count > 0)
                {
                    var a = positionList.Pop();

                    if (positionList.Count != 0)
                    {
                        Debug.DrawLine(new Vector3(a.x, a.y, 0f), new Vector3(positionList.Peek().x, positionList.Peek().y, 0f), Color.red, 5f);
                    }

                    dataStack.Push(a);
                }

                return dataStack;
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
                currentNumber--;
            }
            else
            {
                if (currentAStarData.number != currentNumber)
                {
                    int popCount = currentNumber - currentAStarData.number;

                    for (int i = 0; i < popCount; i++)
                    {
                        positionList.Pop();
                        currentMoveValue -= addMoveValueList.Pop();
                        currentNumber--;
                    }

                    positionList.Push(targetPosition);
                    addMoveValueList.Push(targetMoveValue);
                }
                else
                {
                    positionList.Push(targetPosition);
                    addMoveValueList.Push(targetMoveValue);
                }

                currentPosition = targetPosition;
                currentMoveValue += targetMoveValue;

                isWall[GetBoolPosition(stageData, currentPosition.x, currentPosition.y)] = true;

                currentNumber++;
            }
        }

        return new Stack<Vector2Int>();
    }
}