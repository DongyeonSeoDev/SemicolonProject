#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AddEnemySpawnData))]
public class EnemySpawnInspector : Editor
{
    private Enemy.EnemySpawn enemySpawn;
    private AddEnemySpawnData data;
    private Enemy.Type type;
    private string stageId;

    private bool Check() => !(type == default(Enemy.Type) || string.IsNullOrEmpty(stageId));

    private void OnEnable()
    {
        enemySpawn = FindObjectOfType<Enemy.EnemySpawn>();
        data = FindObjectOfType<AddEnemySpawnData>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(Application.isPlaying);

        EditorGUILayout.Space(2);
        type = (Enemy.Type)EditorGUILayout.EnumPopup("EnemyID", type);
        EditorGUILayout.Space(2);
        stageId = EditorGUILayout.TextField("StageID", stageId);
        EditorGUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("SpawnPosition", EditorStyles.boldLabel);
        GUILayout.Label(data.transform.position.ToString(), EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

        if (GUILayout.Button("Add"))
        {
            if (Check())
            {
                EnemySpawnData spawnData;

                spawnData.enemyId = type;
                spawnData.position = data.transform.position;
                spawnData.stageId = stageId;
                spawnData.name = spawnData.enemyId + " " + spawnData.stageId + " " + spawnData.position;

                enemySpawn.addSpawnDataQueue.Enqueue(spawnData);
            }

            EditorWindow.GetWindow(typeof(EnemySpawnWindow));
        }

        GUILayout.Space(5);
        EditorGUI.EndDisabledGroup();
    }
}
#endif