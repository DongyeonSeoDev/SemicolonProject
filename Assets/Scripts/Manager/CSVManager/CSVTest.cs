using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVTest : CSVManager
{
    protected override string path { get => Path.Combine(Application.dataPath, "Data", "EnemySpawnData.csv"); }
}
