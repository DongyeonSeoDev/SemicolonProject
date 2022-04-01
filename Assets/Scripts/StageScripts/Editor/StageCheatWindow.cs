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

    private string itemName;
    private int itemCnt;

    [MenuItem("Cheat/Normal Cheat")]
    public static void ShowEnemySpawnWindow()
    {
        GetWindow(typeof(StageCheatWindow));
    }

    private void CurrentStageClear()
    {
        if(StageManager.Instance.IsStageClear)
        {
            UIManager.Instance.RequestSystemMsg("이미 문 열려있다");
            return;
        }

        StageManager.Instance.SetClearStage();
        UIManager.Instance.RequestSystemMsg("강제로 문을 열음");
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
                UIManager.Instance.RequestSystemMsg("저주 걸림");
            }
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Remove All Imprecation"))
        {
            StateManager.Instance.RemoveAllStateAbnormality();
            UIManager.Instance.RequestSystemMsg("모든 저주 해제");
        }

        GUILayout.Space(20);

        EditorGUIUtility.wideMode = true;
        
        itemName = EditorGUILayout.TextField("Item Name", itemName);
        itemCnt = EditorGUILayout.IntField("Item Count", itemCnt);
        
        
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Get Item"))
        {
            Inventory.Instance.GetItem(new ItemInfo(itemName, itemCnt));
        }
        if (GUILayout.Button("Show Items"))
        {
            foreach(ItemSO item in GameManager.Instance.ItemDataDic.Values)
            {
                Debug.Log(item.itemName + " : " + item.name);
            }
        }
        GUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }
}
