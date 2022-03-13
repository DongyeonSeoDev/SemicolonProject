using System.Collections.Generic;
using System.IO;
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
            Debug.LogError("�߸��� ��ȯ �Դϴ�.");
            return default(EnemyLootData);
        }

        if (!int.TryParse(datas[0], out result))
        {
            Debug.LogError("�߸��� ��ȯ �Դϴ�.");
            return default(EnemyLootData);
        }

        enemySpawnData.enemyId = result;
        enemySpawnData.lootName = datas[1];

        if (!int.TryParse(datas[2], out result))
        {
            Debug.LogError("�߸��� ��ȯ �Դϴ�.");
            return default(EnemyLootData);
        }

        enemySpawnData.count = result;

        return enemySpawnData;
    }

    public static implicit operator string(EnemyLootData data)
    {
        return data.enemyId.ToString() + ',' + data.lootName + ',' + data.count;
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

    protected override string path { get => Path.Combine(Application.dataPath, "Data", "EnemyLootData.csv"); }

    protected override void HowToRead(string[] data)
    {
        for (int i = 1; i < data.Length; i++)
        {
            sb.Append(data[i] + ',');
        }

        csvData.Add(data[0], sb.ToString().Substring(0, sb.Length - 1));

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
                itemDictionary.Add(items[i].name, items[i]);
            }
        }
    }
}
