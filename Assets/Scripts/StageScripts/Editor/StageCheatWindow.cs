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

    //stage 관련
    private int floor = 1;
    private UnityEngine.Object stageSO;

    //저주 선택
    private StateAbnormality sa;

    //아이템 관련
    private string itemId = "Restorativeherb";
    private int itemCnt = 1;

    //몹 아이디
    private Enemy.EnemyType mobId;

    //툴바 관련
    private int toolbarIdx;
    private string[] toolbars = { "Useable", "Util", "Temporary" };

    //스샷 단축키
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
            UIManager.Instance.RequestSystemMsg("이미 문 열려있다");
            return;
        }

        StageManager.Instance.SetClearStage();
        UIManager.Instance.RequestSystemMsg("강제로 문을 열음");
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
                //useClearStageKey = GUILayout.Toggle(useClearStageKey, "스테이지 넘기기 단축키 사용 (F6)");
                //GUILayout.Label("(몬스터가 나오고 눌러)", EditorStyles.label);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Current Stage Clear (F10)"))  //스테이지 강제 클리어
                {
                    CurrentStageClear();
                }
                if (GUILayout.Button("Current All monster Die"))
                {
                    Enemy.EnemyManager.Instance.PlayerDeadEvent();
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                //GUILayout.Label("(다음 스테이지 )", EditorStyles.boldLabel);
                floor = EditorGUILayout.IntField("target floor", floor);  //몇 층 보스 꺼낼지 정하자
                if (GUILayout.Button("Next Stage Is Boss"))  //담 스테이지는 보스 스테이지로 해줌
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
                sa = (StateAbnormality)EditorGUILayout.EnumPopup("Imprecation", sa);  //저주 선택
                if (GUILayout.Button("Get Imprecation"))  //저주 받기
                {
                    if (sa != StateAbnormality.None)
                    {
                        StateManager.Instance.StartStateAbnormality(sa);
                        UIManager.Instance.RequestSystemMsg("저주 걸림");
                    }
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Remove All Imprecation"))  //저주 해제
                {
                    StateManager.Instance.RemoveAllStateAbnormality();
                    UIManager.Instance.RequestSystemMsg("모든 저주 해제");
                }

                GUILayout.Space(20);
                GUILayout.Label("[Inventory Cheat]", EditorStyles.boldLabel);

                //아이템 선택
                itemId = EditorGUILayout.TextField("Item Id", itemId);
                itemCnt = EditorGUILayout.IntField("Item Count", itemCnt);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Get Item"))  //선택한 아이템 획득
                {
                    Inventory.Instance.GetItem(new ItemInfo(itemId, itemCnt));
                }
                if (GUILayout.Button("Show Items"))  //모든 아이템 정보를 띄움
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
                GUILayout.Label("(저장위치: Assets/ScreenShot)\n(에디터에서만 스샷 가능)", EditorStyles.label);
                GUILayout.Label("단축키 : " + ScreenShot.captureKeyCode.ToString(), EditorStyles.label);

                GUILayout.Space(20);
                keyCode = (KeyCode)EditorGUILayout.EnumPopup("스샷 단축키", keyCode);
                if (GUILayout.Button("스샷 단축키 변경 적용"))
                {
                    ScreenShot.captureKeyCode = keyCode;
                }
                EditorGUI.EndDisabledGroup();

                break;

            case 2:
                EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                GUILayout.Label("[Saved Body Cheat]", EditorStyles.boldLabel);
                GUILayout.Label("(이 기능은 게임 시작하고 몹 슬롯에 아무것도 없을 때만 가능. \n테스트만 하고 종료하자)", EditorStyles.label);

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
