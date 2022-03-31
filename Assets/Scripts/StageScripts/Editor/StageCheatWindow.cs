using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class StageCheatWindow : EditorWindow
{
    

    [MenuItem("StageWindow/Cheat")]
    public static void ShowEnemySpawnWindow()
    {
        GetWindow(typeof(StageCheatWindow));
    }

    private void CurrentStageClear()
    {
        if(StageManager.Instance.IsStageClear)
        {
            UIManager.Instance.RequestSystemMsg("�̹� �� �������ݾ� �Ӹ�");
            return;
        }

        StageManager.Instance.SetClearStage();
        UIManager.Instance.RequestSystemMsg("������ ���� ����");
        Enemy.EnemyManager.Instance.PlayerDeadEvent();
    }

    private void Awake()
    {
        Debug.Log("1");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Current Stage Clear"))
        {
            CurrentStageClear();
        }
        if (GUILayout.Button("All monster Die"))
        {
            Enemy.EnemyManager.Instance.PlayerDeadEvent();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        if (GUILayout.Button("Apply Player Stat"))
        {

        }

        EditorGUI.EndDisabledGroup();
    }
}
