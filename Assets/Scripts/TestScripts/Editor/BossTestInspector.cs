using UnityEditor;
using UnityEngine;

#if ENABLE_UNITYEVENTS
[CustomEditor(typeof(BossTest))]
public class BossTestInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Boss Spawn"))
        {
            if (Application.isPlaying && FindObjectOfType<Enemy.Boss1SkeletonKing>() == null)
            {
                SlimeGameManager.Instance.CurrentPlayerBody.transform.position = FindObjectOfType<BossTest>().playerStartPosition;
                EventManager.TriggerEvent("SpawnEnemy", "BossTest_01");
            }
        }

        if (GUILayout.Button("Boss Move"))
        {
            var boss = FindObjectOfType<Enemy.Boss1SkeletonKing>();

            if (Application.isPlaying && boss != null)
            {
                boss.GetComponent<Enemy.Boss1SkeletonKing>().Move();
            }
        }

        GUILayout.Space(5);
    }
}

public class BossTestWindow : EditorWindow
{
    private GUILayoutOption[] buttonLayout = new GUILayoutOption[]
    {
        GUILayout.Width(300),
        GUILayout.Height(30)
    };

    [MenuItem("EnemyWindow/BossTest")]
    static void Init()
    {
        GetWindow(typeof(BossTestWindow));
    }

    public void OnGUI()
    {
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Boss Spawn", buttonLayout))
        {
            if (Application.isPlaying && FindObjectOfType<Enemy.Boss1SkeletonKing>() == null)
            {
                SlimeGameManager.Instance.CurrentPlayerBody.transform.position = FindObjectOfType<BossTest>().playerStartPosition;
                EventManager.TriggerEvent("SpawnEnemy", "BossTest_01");
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Boss Move", buttonLayout))
        {
            var boss = FindObjectOfType<Enemy.Boss1SkeletonKing>();

            if (Application.isPlaying && boss != null)
            {
                boss.GetComponent<Enemy.Boss1SkeletonKing>().Move();
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }
}
#endif