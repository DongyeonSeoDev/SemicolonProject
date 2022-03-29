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
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);

        if (GUILayout.Button("Boss Spawn"))
        {
            if (FindObjectOfType<Enemy.Boss1SkeletonKing>() == null)
            {
                SlimeGameManager.Instance.CurrentPlayerBody.transform.position = FindObjectOfType<BossTest>().playerStartPosition;
                EventManager.TriggerEvent("SpawnEnemy", "BossTest_01");
            }
            else
            {
                Debug.LogWarning("WARNING(BossTestInspector): ������ �̹� ��ȯ�Ǿ� �ֽ��ϴ�.");
            }
        }

        if (GUILayout.Button("Boss Move"))
        {
            var boss = FindObjectOfType<Enemy.Boss1SkeletonKing>();

            if (boss != null)
            {
                EventManager.TriggerEvent("EnemyMove", "BossTest_01");
            }
            else
            {
                Debug.LogWarning("WARNING(BossTestInspector): ������ ���� ��ȯ���� �ʾҽ��ϴ�.");
            }
        }

        EditorGUI.EndDisabledGroup();
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
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Boss Spawn", buttonLayout))
        {
            if (FindObjectOfType<Enemy.Boss1SkeletonKing>() == null)
            {
                SlimeGameManager.Instance.CurrentPlayerBody.transform.position = FindObjectOfType<BossTest>().playerStartPosition;
                EventManager.TriggerEvent("SpawnEnemy", "BossTest_01");
            }
            else
            {
                Debug.LogWarning("WARNING(BossTestWindow): ������ �̹� ��ȯ�Ǿ� �ֽ��ϴ�.");
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

            if (boss != null)
            {
                EventManager.TriggerEvent("EnemyMove", "BossTest_01");
            }
            else
            {
                Debug.LogWarning("WARNING(BossTestWindow): ������ ���� ��ȯ���� �ʾҽ��ϴ�.");
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUI.EndDisabledGroup();
    }
}
#endif