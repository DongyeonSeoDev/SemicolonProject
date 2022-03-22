#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EnemySpawnWindow : EditorWindow
{
    private Enemy.EnemySpawn enemySpawn;
    private Enemy.Type type;
    private Vector3 spawnPosition;
    private string stageId;

    [MenuItem("EnemyWindow/EnemySpawn")]
    public static void ShowEnemySpawnWindow()
    {
        GetWindow(typeof(EnemySpawnWindow));
    }

    private void ResetText()
    {
        type = default(Enemy.Type);
        spawnPosition = default(Vector3);
        stageId = null;
    }

    private bool Check() => !(type == default(Enemy.Type) || string.IsNullOrEmpty(stageId));

    private void OnEnable()
    {
        enemySpawn = FindObjectOfType<Enemy.EnemySpawn>();
    }

    private void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        GUILayout.Space(10);
        type = (Enemy.Type)EditorGUILayout.EnumPopup("EnemyID", type);
        GUILayout.Space(10);

        EditorGUIUtility.wideMode = true;
        spawnPosition = EditorGUILayout.Vector3Field("SpawnPosition", spawnPosition);
        EditorGUIUtility.wideMode = false;
        GUILayout.Space(10);

        stageId = EditorGUILayout.TextField("StageID", stageId);
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add"))
        {
            if (Check())
            {
                EnemySpawnData data;

                data.enemyId = type;
                data.position = spawnPosition;
                data.stageId = stageId;
                data.name = data.enemyId + " " + data.stageId + " " + data.position;

                enemySpawn.addSpawnDataQueue.Enqueue(data);
            }

            ResetText();

            GetWindow(typeof(EnemySpawnListWindow));
        }

        if (GUILayout.Button("Remove"))
        {
            if (Check())
            {
                EnemySpawnData data;

                data.enemyId = type;
                data.position = spawnPosition;
                data.stageId = stageId;
                data.name = data.enemyId + " " + data.stageId + " " + data.position;

                enemySpawn.removeSpawnDataQueue.Enqueue(data);
            }

            ResetText();

            GetWindow(typeof(EnemySpawnListWindow));
        }

        if (GUILayout.Button("List"))
        {
            GetWindow(typeof(EnemySpawnListWindow));
        }

        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }
}
#endif