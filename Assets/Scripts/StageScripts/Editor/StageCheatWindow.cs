using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class StageCheatWindow : EditorWindow
{
    public Player SlimePlayer => SlimeGameManager.Instance.Player;

    private Stat playerStat = new Stat();
    private int recoveryHp;
    private StateAbnormality sa;

    [MenuItem("Cheat/Normal Cheat")]
    public static void ShowEnemySpawnWindow()
    {
        GetWindow(typeof(StageCheatWindow));
    }

    private void CurrentStageClear()
    {
        if(StageManager.Instance.IsStageClear)
        {
            UIManager.Instance.RequestSystemMsg("�̹� �� �����ִ�");
            return;
        }

        StageManager.Instance.SetClearStage();
        UIManager.Instance.RequestSystemMsg("������ ���� ����");
        Enemy.EnemyManager.Instance.PlayerDeadEvent();
    }

    private void OnEnable()
    {
        playerStat = SlimePlayer.PlayerStat;
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

        playerStat.eternalStat.maxHp = EditorGUILayout.FloatField("Max Hp", playerStat.eternalStat.maxHp);
        playerStat.eternalStat.minDamage = EditorGUILayout.IntField("Min Damage", playerStat.eternalStat.minDamage);
        playerStat.eternalStat.maxDamage = EditorGUILayout.IntField("Max Damage", playerStat.eternalStat.maxDamage);
        playerStat.eternalStat.defense = EditorGUILayout.IntField("Defense", playerStat.eternalStat.defense);
        playerStat.eternalStat.speed = EditorGUILayout.FloatField("Speed", playerStat.eternalStat.speed);

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Player Eternal Stat"))
        {
            SlimePlayer.PlayerStat = playerStat;
        }

        GUILayout.Space(15);

        recoveryHp = EditorGUILayout.IntField("Heal Current Hp n%", 20);

        GUILayout.Space(7);

        if (GUILayout.Button("Player Heal"))
        {
            SlimePlayer.GetHeal(recoveryHp);
        }

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        sa = (StateAbnormality)EditorGUILayout.EnumPopup("Imprecation", sa);
        if (GUILayout.Button("Get Imprecation"))
        {
            if (sa != StateAbnormality.None)
            {
                StateManager.Instance.StartStateAbnormality(sa);
                UIManager.Instance.RequestSystemMsg("���� �ɸ�");
            }
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Remove All Imprecation"))
        {
            StateManager.Instance.RemoveAllStateAbnormality();
            UIManager.Instance.RequestSystemMsg("��� ���� ����");
        }

        GUILayout.Space(20);

        EditorGUI.EndDisabledGroup();
    }
}
