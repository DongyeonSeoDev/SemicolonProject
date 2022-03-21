#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EnemySpawnWindow : EditorWindow
{
    private string id;
    private Enemy.Type type;
    private Vector3 spawnPosition;
    private string stageId;

    [MenuItem("EnemyWindow/EnemySpawn")]
    public static void ShowEnemySpawnWindow()
    {
        GetWindow(typeof(EnemySpawnWindow));
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        id = EditorGUILayout.TextField("ID", id);
        GUILayout.Space(10);

        type = (Enemy.Type)EditorGUILayout.EnumPopup("EnemyID", type);
        GUILayout.Space(10);

        EditorGUIUtility.wideMode = true;
        spawnPosition = EditorGUILayout.Vector3Field("SpawnPosition", spawnPosition);
        EditorGUIUtility.wideMode = false;
        GUILayout.Space(10);

        stageId = EditorGUILayout.TextField("StageID", stageId);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            Debug.Log(id + type + spawnPosition + stageId);

            id = null;
            type = default(Enemy.Type);
            spawnPosition = default(Vector3);
            stageId = null;
        }

        if (GUILayout.Button("Load"))
        {
            GetWindow(typeof(EnemySpawnListWindow));
        }

        if (GUILayout.Button("Find"))
        {
            Debug.Log(id + type + spawnPosition + stageId);
        }

        GUILayout.EndHorizontal();
    }
}
#endif