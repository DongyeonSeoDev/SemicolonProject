using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SetStageDataEditor : Editor
{
    public static string getStagePath = Path.Combine("Stage", "StagePrefab");
    public static string stageSavePath = Path.Combine(Application.dataPath, "Resources", "Enemy", "StageData", "StageData.txt");

    [MenuItem("EnemyWindow/SetStageData")]
    public static void Click()
    {
        List<StageData> stageDataList = new List<StageData>();

        GameObject[] stages = Resources.LoadAll<GameObject>(getStagePath);

        for (int i = 0; i < stages.Length; i++)
        {
            StageGround ground = stages[i].GetComponent<StageGround>();

            if (ground.isEnemyStage)
            {
                if (ground.name.Contains("Pref"))
                {
                    stageDataList.Add(FindPath.SetStageData(ground.noPassTilemap, ground.limitMinPosition, ground.limitMaxPosition, ground.name.Substring(0, ground.name.IndexOf("Pref"))));
                }
                else
                {
                    stageDataList.Add(FindPath.SetStageData(ground.noPassTilemap, ground.limitMinPosition, ground.limitMaxPosition, ground.name));
                }
            }
        }

        File.WriteAllText(stageSavePath, JsonUtility.ToJson(new JsonParse<StageData>(stageDataList)));

        Debug.Log("Stage Data Reset!");
        Debug.Log("GetStagePath: " + Path.Combine(Application.dataPath, "Resources", getStagePath));
        Debug.Log("StageSavePath: " + stageSavePath);
    }
}
