using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct EnemySpawnData
{
    public int enemyId;
    public Vector3 position;
    public string stageId;

    public static implicit operator EnemySpawnData(string data)
    {
        EnemySpawnData enemySpawnData = new EnemySpawnData();
        int result = 0;

        float[] positionArray = new float[3];
        string[] datas = data.Split(',');

        if (datas.Length != 3)
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemySpawnData);
        }

        if (!int.TryParse(datas[0], out result))
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemySpawnData);
        }

        enemySpawnData.enemyId = result;
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
    private StringBuilder sb = new StringBuilder(128);
   // private int value;

    protected override string path { get => "Data/EnemySpawnData"; }

    protected override void HowToRead(string[] data)
    {
        if (data.Length < 2 || data[0].Contains("id"))
        {
            return;
        }

        for (int i = 1; i < data.Length; i++)
        {
            sb.Append(data[i].Trim() + ',');
        }

        csvData.Add(data[0].Trim(), sb.ToString().Substring(0, sb.Length - 1));
        data[3] = data[3].Trim();

        if (enemySpawnDatas.ContainsKey(data[3]))
        {
            enemySpawnDatas[data[3]].Add(sb.ToString().Substring(0, sb.Length - 1));
        }
        else
        {
            enemySpawnDatas.Add(data[3], new List<EnemySpawnData>());
            enemySpawnDatas[data[3]].Add(sb.ToString().Substring(0, sb.Length - 1));
        }

        sb.Clear();
    }
}
