using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageCheatWindow : EditorWindow
{
    public Player SlimePlayer => SlimeGameManager.Instance.Player;

    private Stat playerStat = new Stat();
    private int recoveryHp;
    private StateAbnormality sa;

    private string itemId;
    private int itemCnt;

    private Enemy.EnemyType mobId;

    private int toolbarIdx;
    private string[] toolbars = {"Useable", "Temporary"};

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
        toolbarIdx = GUILayout.Toolbar(toolbarIdx, toolbars);

        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        switch (toolbarIdx)
        {
            case 0:
                

                GUILayout.Label("[Stage Cheat]", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Current Stage Clear"))
                {
                    CurrentStageClear();
                }
                /*if (GUILayout.Button("Current All monster Die"))
                {
                    Enemy.EnemyManager.Instance.PlayerDeadEvent();
                }*/
                GUILayout.EndHorizontal();

                GUILayout.Space(20);
                GUILayout.Label("[Player Cheat]", EditorStyles.boldLabel);

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

                GUILayout.Space(20);
                GUILayout.Label("[Status Cheat]", EditorStyles.boldLabel);

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
                GUILayout.Label("[Inventory Cheat]", EditorStyles.boldLabel);

                itemId = EditorGUILayout.TextField("Item Id", itemId);
                itemCnt = EditorGUILayout.IntField("Item Count", itemCnt);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Get Item"))
                {
                    Inventory.Instance.GetItem(new ItemInfo(itemId, itemCnt));
                }
                if (GUILayout.Button("Show Items"))
                {
                    foreach (ItemSO item in GameManager.Instance.ItemDataDic.Values)
                    {
                        Debug.Log(item.itemName + " : " + item.name);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);
                break;
            case 1:
                GUILayout.Label("[Saved Body Cheat]", EditorStyles.boldLabel);

                mobId = (Enemy.EnemyType)EditorGUILayout.EnumPopup("Monster Id", mobId);
                if (GUILayout.Button("Save Body"))
                {
                    PlayerEnemyUnderstandingRateManager.Instance.SetMountObj(mobId.ToString());
                }

                
                break;
        }

        EditorGUI.EndDisabledGroup();

    }
}