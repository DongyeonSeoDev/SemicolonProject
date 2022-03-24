#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EnemySpawnWindow : EditorWindow
{
    private Enemy.EnemySpawn enemySpawn;
    private Enemy.Type type;
    private Vector3 spawnPosition;
    private string stageId;
    private bool isPlaying = false;

    [MenuItem("EnemyWindow/EnemySpawn")]
    public static void ShowEnemySpawnWindow()
    {
        GetWindow(typeof(EnemySpawnWindow));
    }

    public void ShowEnemySpawnListWindow()
    {
        GetWindow(typeof(EnemySpawnListWindow));
    }

    private void ResetText()
    {
        type = default(Enemy.Type);
        spawnPosition = default(Vector3);
        stageId = null;
    }

    private bool Check() => !(type == default(Enemy.Type) || string.IsNullOrEmpty(stageId));

    private void Add()
    {
        if (Check())
        {
            EnemySpawnData data;

            data.enemyId = type;
            data.position = spawnPosition;
            data.stageId = stageId;
            data.name = data.enemyId + " " + data.stageId + " " + data.position;

            enemySpawn.addSpawnDataQueue.Enqueue(data);

            Debug.Log("Add(EnemySpawnWindow): 데이터가 큐에 추가되었습니다.");
        }
        else
        {
            Debug.LogWarning("Warning(EnemySpawnWindow): 빈 데이터가 있습니다.");
        }

        ResetText();

        ShowEnemySpawnListWindow();
    }

    private void Remove()
    {
        if (Check())
        {
            EnemySpawnData data;

            data.enemyId = type;
            data.position = spawnPosition;
            data.stageId = stageId;
            data.name = data.enemyId + " " + data.stageId + " " + data.position;

            enemySpawn.removeSpawnDataQueue.Enqueue(data);

            Debug.Log("Delete(EnemySpawnWindow): 데이터가 큐에 추가되었습니다.");
        }
        else
        {
            Debug.LogWarning("Warning(EnemySpawnWindow): 빈 데이터가 있습니다.");
        }

        ResetText();

        ShowEnemySpawnListWindow();
    }

    private void Update()
    {
        if (Application.isPlaying != isPlaying)
        {
            isPlaying = Application.isPlaying;
            OnEnable();
        }
    }

    private void OnEnable()
    {
        Debug.Log("Reload(EnemySpawnWindow): 데이터를 불러왔습니다.");
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
            Add();
        }

        if (GUILayout.Button("Remove"))
        {
            Remove();
        }

        if (GUILayout.Button("Show"))
        {
            ShowEnemySpawnListWindow();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reload"))
        {
            OnEnable();
        }

        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }
}
#endif