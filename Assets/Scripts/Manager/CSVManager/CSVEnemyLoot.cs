using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct EnemyLootData
{
    public int enemyId;
    public string lootName;
    public int count;

    public static implicit operator EnemyLootData(string data)
    {
        EnemyLootData enemySpawnData = new EnemyLootData();
        int result = 0;

        string[] datas = data.Split(',');

        if (datas.Length != 3)
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemyLootData);
        }

        if (!int.TryParse(datas[0], out result))
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemyLootData);
        }

        enemySpawnData.enemyId = result;
        enemySpawnData.lootName = datas[1];

        if (!int.TryParse(datas[2], out result))
        {
            Debug.LogError("잘못된 변환 입니다.");
            return default(EnemyLootData);
        }

        enemySpawnData.count = result;

        return enemySpawnData;
    }

    public static implicit operator string(EnemyLootData data)
    {
        return data.enemyId.ToString().Trim() + ',' + data.lootName.Trim() + ',' + data.count;
    }
}

public class CSVEnemyLoot : CSVManager
{
    private static CSVEnemyLoot instance;
    public static CSVEnemyLoot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CSVEnemyLoot();
            }

            return instance;
        }
    }

    public List<List<EnemyLootData>> lootData = new List<List<EnemyLootData>>();
    public Dictionary<string, ItemSO> itemDictionary = new Dictionary<string, ItemSO>();

    private StringBuilder sb = new StringBuilder(128);
    private int value;
    private bool isReadItemData = false;

    protected override string path { get => "Data/EnemyLootData"; }

    protected override void HowToRead(string[] data)
    {
        if (data.Length < 2)
        {
            return;
        }

        for (int i = 1; i < data.Length; i++)
        {
            sb.Append(data[i].Trim() + ',');
        }

        csvData.Add(data[0].Trim(), sb.ToString().Substring(0, sb.Length - 1));
        data[1] = data[1].Trim();

        if (int.TryParse(data[1], out value))
        {
            while (lootData.Count <= value)
            {
                lootData.Add(new List<EnemyLootData>());
            }

            lootData[value].Add(sb.ToString().Substring(0, sb.Length - 1));
        }

        sb.Clear();

        if (!isReadItemData)
        {
            isReadItemData = true;

            ItemSO[] items = Resources.LoadAll<ItemSO>("System");

            for (int i = 0; i < items.Length; i++)
            {
                itemDictionary.Add(items[i].name.Trim(), items[i]);
            }
        }
    }
}
