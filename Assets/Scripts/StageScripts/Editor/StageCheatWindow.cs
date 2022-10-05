using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Water;
using System;
using System.IO;

public class StageCheatWindow : EditorWindow
{
    public Player SlimePlayer => SlimeGameManager.Instance.Player;

    //private bool useClearStageKey = false;

    //stage ����
    private int floor = 1;
    private UnityEngine.Object stageSO;

    //���� ����
    private StateAbnormality sa;

    //������ ����
    private string itemId = "Restorativeherb";
    private int itemCnt = 1;

    //�� ���̵�
    private Enemy.EnemyType mobId;

    //���� ����
    private int toolbarIdx;
    private string[] toolbars = { "Useable", "Util", "Temporary" };

    //���� ����Ű
    private KeyCode keyCode = KeyCode.G;

    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("Cheat/Stage Cheat")]
    public static void ShowStageCheatWindow()
    {
        GetWindow(typeof(StageCheatWindow));
    }

    private void CurrentStageClear()
    {
        if (StageManager.Instance.IsStageClear)
        {
            UIManager.Instance.RequestSystemMsg("�̹� �� �����ִ�");
            return;
        }

        StageManager.Instance.SetClearStage();
        UIManager.Instance.RequestSystemMsg("������ ���� ����");
        Enemy.EnemyManager.Instance.PlayerDeadEvent();
    }

    private void ChangeNextStageToBoss()
    {
        StageBundleDataSO so = StageManager.Instance.GetStageBundleData(floor);
        StageDataSO data = so.stages.Find(x => x.areaType == AreaType.BOSS);
        StageManager.Instance.GetFieldInfo("currentStageNumber").SetValue(StageManager.Instance, so.LastStageNumber - 1);
        StageManager.Instance.CurrentStageGround.stageDoors.ForEach(x => x.nextStageData = data);
    }

    private void ChangeNextStage()
    {
        if(stageSO != null)
        {
            StageManager.Instance.CurrentStageGround.stageDoors.ForEach(x => x.nextStageData = (StageDataSO)stageSO);
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.MinWidth(200), GUILayout.MaxWidth(1000), GUILayout.ExpandWidth(true), GUILayout.MinHeight(200), GUILayout.MaxHeight(1000), GUILayout.ExpandHeight(true));

        toolbarIdx = GUILayout.Toolbar(toolbarIdx, toolbars);
        GUILayout.Space(10);
        

        switch (toolbarIdx)
        {
            case 0:
                EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                GUILayout.Label("[Stage Cheat]", EditorStyles.boldLabel);
                //useClearStageKey = GUILayout.Toggle(useClearStageKey, "�������� �ѱ�� ����Ű ��� (F6)");
                //GUILayout.Label("(���Ͱ� ������ ����)", EditorStyles.label);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Current Stage Clear (F10)"))  //�������� ���� Ŭ����
                {
                    CurrentStageClear();
                }
                if (GUILayout.Button("Current All monster Die"))
                {
                    Enemy.EnemyManager.Instance.PlayerDeadEvent();
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                //GUILayout.Label("(���� �������� )", EditorStyles.boldLabel);
                floor = EditorGUILayout.IntField("target floor", floor);  //�� �� ���� ������ ������
                if (GUILayout.Button("Next Stage Is Boss"))  //�� ���������� ���� ���������� ����
                {
                    ChangeNextStageToBoss();
                }

                GUILayout.Space(10);

                stageSO = EditorGUILayout.ObjectField(stageSO, typeof(StageDataSO), true);

                if(GUILayout.Button("Set Next Stage"))
                {
                    ChangeNextStage();
                }

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                sa = (StateAbnormality)EditorGUILayout.EnumPopup("Imprecation", sa);  //���� ����
                if (GUILayout.Button("Get Imprecation"))  //���� �ޱ�
                {
                    if (sa != StateAbnormality.None)
                    {
                        StateManager.Instance.StartStateAbnormality(sa);
                        UIManager.Instance.RequestSystemMsg("���� �ɸ�");
                    }
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Remove All Imprecation"))  //���� ����
                {
                    StateManager.Instance.RemoveAllStateAbnormality();
                    UIManager.Instance.RequestSystemMsg("��� ���� ����");
                }

                GUILayout.Space(20);
                GUILayout.Label("[Inventory Cheat]", EditorStyles.boldLabel);

                //������ ����
                itemId = EditorGUILayout.TextField("Item Id", itemId);
                itemCnt = EditorGUILayout.IntField("Item Count", itemCnt);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Get Item"))  //������ ������ ȹ��
                {
                    Inventory.Instance.GetItem(new ItemInfo(itemId, itemCnt));
                }
                if (GUILayout.Button("Show Items"))  //��� ������ ������ ���
                {
                    foreach (ItemSO item in GameManager.Instance.ItemDataDic.Values)
                    {
                        Debug.Log(item.itemName + " : " + item.name);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);
                EditorGUI.EndDisabledGroup();
                break;

            case 1:

                EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                GUILayout.Space(20);
                GUILayout.Label("[ScreenShot]", EditorStyles.boldLabel);
                GUILayout.Label("(������ġ: Assets/ScreenShot)\n(�����Ϳ����� ���� ����)", EditorStyles.label);
                GUILayout.Label("����Ű : " + ScreenShot.captureKeyCode.ToString(), EditorStyles.label);

                GUILayout.Space(20);
                keyCode = (KeyCode)EditorGUILayout.EnumPopup("���� ����Ű", keyCode);
                if (GUILayout.Button("���� ����Ű ���� ����"))
                {
                    ScreenShot.captureKeyCode = keyCode;
                }
                EditorGUI.EndDisabledGroup();

                break;

            case 2:
                EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                GUILayout.Label("[Saved Body Cheat]", EditorStyles.boldLabel);
                GUILayout.Label("(�� ����� ���� �����ϰ� �� ���Կ� �ƹ��͵� ���� ���� ����. \n�׽�Ʈ�� �ϰ� ��������)", EditorStyles.label);

                mobId = (Enemy.EnemyType)EditorGUILayout.EnumPopup("Monster Id", mobId);
                if (GUILayout.Button("Save Body"))
                {
                    PlayerEnemyUnderstandingRateManager.Instance.SetMountObj(mobId.ToString());
                }
                EditorGUI.EndDisabledGroup();
                break;
        }

        EditorGUI.EndDisabledGroup();

        GUILayout.EndScrollView();
    }
}
