using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public struct EnemySpawnData
{
    public int enemyId;
    public Vector3 position;
    public int stageId;

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

        if (!int.TryParse(datas[2], out result))
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemySpawnData);
        }

        enemySpawnData.stageId = result;

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
        return enemySpawnData.enemyId.ToString() + ',' + enemySpawnData.position.x + ';' + enemySpawnData.position.y + ';' + enemySpawnData.position.z + ',' + enemySpawnData.stageId;
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

    public List<List<EnemySpawnData>> enemySpawnDatas = new List<List<EnemySpawnData>>();
    private StringBuilder sb = new StringBuilder(128);
    private int value;

    protected override string path { get => Path.Combine(Application.dataPath, "Data", "EnemySpawnData.csv"); }

    protected override void HowToRead(string[] data)
    {
        for (int i = 1; i < data.Length; i++)
        {
            sb.Append(data[i] + ',');
        }

        csvData.Add(data[0], sb.ToString().Substring(0, sb.Length - 1));

        if (int.TryParse(data[3], out value))
        {
            while (enemySpawnDatas.Count <= value)
            {
                enemySpawnDatas.Add(new List<EnemySpawnData>());
            }

            enemySpawnDatas[value].Add(sb.ToString().Substring(0, sb.Length - 1));
        }

        sb.Clear();
    }
}
