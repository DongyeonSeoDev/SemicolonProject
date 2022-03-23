using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

[System.Serializable]
public struct EnemySpawnData
{
    public string name;
    public Enemy.Type enemyId;
    public Vector3 position;
    public string stageId;

    public static implicit operator EnemySpawnData(string data)
    {
        EnemySpawnData enemySpawnData = new EnemySpawnData();

        float[] positionArray = new float[3];
        string[] datas = data.Split(',');

        if (datas.Length != 3)
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemySpawnData);
        }

        Enemy.Type type;

        if (!Enum.TryParse<Enemy.Type>(datas[0], out type))
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemySpawnData);
        }

        if ((int)type >= 100)
        {
            Debug.LogError("이것은 Enemy가 아닙니다. 만약 Enemy라면 값을 변경해주세요. 현재 Enemy < 100");
            return default(EnemySpawnData);
        }

        enemySpawnData.enemyId = type;
        enemySpawnData.stageId = datas[2];

        datas = datas[1].Split(';');

        if (datas.Length != 3)
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemySpawnData);
        }

        for (int i = 0; i < 3; i++)
        {
            if (!float.TryParse(datas[i], out positionArray[i]))
            {
                Debug.LogError("잘못된 변환 입니다.");
                return default(EnemySpawnData);
            }
        }

        enemySpawnData.position = new Vector3(positionArray[0], positionArray[1], positionArray[2]);
        enemySpawnData.name = enemySpawnData.enemyId + " " + enemySpawnData.stageId + " " + enemySpawnData.position;

        return enemySpawnData;
    }

    public static implicit operator string(EnemySpawnData enemySpawnData)
    {
        return enemySpawnData.enemyId.ToString().Trim() + ',' + enemySpawnData.position.x + ';' + enemySpawnData.position.y + ';' + enemySpawnData.position.z + ',' + enemySpawnData.stageId.Trim();
    }
}

public class CSVEnemySpawn : CSVManager
{
    private static CSVEnemySpawn instance;
    public static CSVEnemySpawn Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CSVEnemySpawn();
            }

            return instance;
        }
    }

    public Dictionary<string, List<EnemySpawnData>> enemySpawnDatas = new Dictionary<string, List<EnemySpawnData>>();
    public List<EnemySpawnData> enemySpawnData = new List<EnemySpawnData>();
    private StringBuilder sb = new StringBuilder(128);

    private readonly string firstData = "EnemyID,Position,StageID";

    protected override string path { get => "Data/EnemySpawnData"; }

    protected override void HowToRead(string[] data)
    {
        if (data.Length < 2)
        {
            return;
        }

        sb.Clear();

        for (int i = 0; i < data.Length; i++)
        {
            sb.Append(data[i].Trim() + ',');
        }

        if (sb.ToString().Substring(0, sb.Length - 1) == firstData)
        {
            return;
        }

        data[2] = data[2].Trim();

        EnemySpawnData spawnData = sb.ToString().Substring(0, sb.Length - 1);
        bool isCheck = false;

        foreach (var x in enemySpawnDatas.Values)
        {
            foreach (var y in x)
            {
                if (y.name == spawnData.name)
                {
                    isCheck = true;

                    break;
                }
            }

            if (isCheck)
            {
                break;
            }
        }

        if (!isCheck)
        {
            if (enemySpawnDatas.ContainsKey(data[2]))
            {
                enemySpawnDatas[data[2]].Add(sb.ToString().Substring(0, sb.Length - 1));
            }
            else
            {
                enemySpawnDatas.Add(data[2], new List<EnemySpawnData>());
                enemySpawnDatas[data[2]].Add(sb.ToString().Substring(0, sb.Length - 1));
            }
        }
    }

    protected override string HowToWrite()
    {
        sb.Clear();
        sb.Append(firstData + '\n');

        for (int i = 0; i < enemySpawnData.Count; i++)
        {
            sb.Append(enemySpawnData[i] + '\n');
        }

        return sb.ToString();
    }
}
