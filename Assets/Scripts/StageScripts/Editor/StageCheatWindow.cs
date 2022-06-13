using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Water;

public class StageCheatWindow : EditorWindow
{
    public Player SlimePlayer => SlimeGameManager.Instance.Player;

    //private bool useClearStageKey = false;

    //stage 관련
    private int floor;

    //Player 관련
    private Stat playerStat = new Stat();
    private float useEnergyAmount = 1f;
    private float upMountingPercentageValueWhenEnemyDead = 2f;
    private int upUnderstandingRateValueWhenEnemyDead = 1;

    //몸 교체 딜레이
    private float bodyChangeTime = 10f;

    //스킬 딜레이
    private float[] skillDelays = new float[3];

    //회복량
    private int recoveryHp = 20;
    //저주 선택
    private StateAbnormality sa;

    //아이템 관련
    private string itemId;
    private int itemCnt;

    //몹 아이디
    private Enemy.EnemyType mobId;

    //툴바 관련
    private int toolbarIdx;
    private string[] toolbars = { "Useable", "Util", "Temporary" };

    //스샷 단축키
    private KeyCode keyCode = KeyCode.G;

    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("Cheat/Normal Cheat")]
    public static void ShowEnemySpawnWindow()
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
        StageDataSO data = StageManager.Instance.GetStageBundleData(floor).stages.Find(x => x.areaType == AreaType.BOSS);
        StageManager.Instance.CurrentStageGround.stageDoors.ForEach(x => x.nextStageData = data);
    }

    private void OnEnable()
    {
        playerStat = SlimePlayer.PlayerStat;

        useEnergyAmount = 1f;
        upMountingPercentageValueWhenEnemyDead = 2f;
        upUnderstandingRateValueWhenEnemyDead = 1;
        bodyChangeTime = 10f;
        floor = 1;

        skillDelays = SlimeGameManager.Instance.SkillDelays;
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

                /*if(useClearStageKey && Event.current.isKey && Event.current.keyCode == KeyCode.F6)
                {
                    CurrentStageClear();
                    
                }*/
                

                GUILayout.Space(20);
                GUILayout.Label("[Player Cheat]", EditorStyles.boldLabel);

                //플레이어 스탯 정함
                playerStat.eternalStat.maxHp.statValue = EditorGUILayout.FloatField("Max Hp", playerStat.eternalStat.maxHp.statValue);
                playerStat.eternalStat.minDamage.statValue = EditorGUILayout.FloatField("Min Damage", playerStat.eternalStat.minDamage.statValue);
                playerStat.eternalStat.maxDamage.statValue = EditorGUILayout.FloatField("Max Damage", playerStat.eternalStat.maxDamage.statValue);
                playerStat.eternalStat.defense.statValue = EditorGUILayout.FloatField("Defense", playerStat.eternalStat.defense.statValue);
                playerStat.eternalStat.speed.statValue = EditorGUILayout.FloatField("Speed", playerStat.eternalStat.speed.statValue);

                GUILayout.Space(10);

                if (GUILayout.Button("Apply Player Eternal Stat")) //플레이어 스탯 적용
                {
                    SlimePlayer.PlayerStat = playerStat;
                }

                GUILayout.Space(10);

                useEnergyAmount = EditorGUILayout.FloatField("Slime Attack Need Energe", useEnergyAmount); //공격 에너지 소모량 정함

                GUILayout.Space(5);

                GUILayout.Label("(슬라임 전용. 몸 바뀌면 다시 Apply 필요할 수도)", EditorStyles.label);

                //공격 에너지 소모량 적용
                if (GUILayout.Button("Apply Attack Need Energe"))
                {
                    PlayerShoot shoot = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerShoot>();
                    if(shoot != null)
                    {
                        shoot.GetFieldInfo<PlayerShoot>("useEnergyAmount").SetValue(shoot, useEnergyAmount);
                    }
                }

                GUILayout.Space(20);
                //흡수확률 정함
                upMountingPercentageValueWhenEnemyDead = EditorGUILayout.FloatField("Drain Probability", upMountingPercentageValueWhenEnemyDead);
                if (GUILayout.Button("Apply Drain Probability")) //흡수확률 세팅
                {             
                    SlimePlayer.GetFieldInfo<Player>("upMountingPercentageValueWhenEnemyDead").SetValue(SlimePlayer, upMountingPercentageValueWhenEnemyDead);
                }

                GUILayout.Space(10);
                //적을 죽일 때 오르는 동화율의 값 정함
                upUnderstandingRateValueWhenEnemyDead = EditorGUILayout.IntField("Up Understanding Rate", upUnderstandingRateValueWhenEnemyDead);
                GUILayout.Label("(적을 죽일 때 오르는 동화율의 값)", EditorStyles.label);
                if (GUILayout.Button("Apply Understanding Rate"))  //적을 죽일 때 오르는 동화율의 값
                {                    
                    SlimePlayer.GetFieldInfo<Player>("upUnderstandingRateValueWhenEnemyDead").SetValue(SlimePlayer, upUnderstandingRateValueWhenEnemyDead);
                }

                GUILayout.Space(20);
                GUILayout.Label("[Body Change Cheat]", EditorStyles.boldLabel);

                bodyChangeTime = EditorGUILayout.FloatField("BodyChangeTime", bodyChangeTime);  //몸 교체 딜레이 정함
                if (GUILayout.Button("Apply Body Change Time"))  //몸 교체 딜레이 적용
                {
                    SlimeGameManager.Instance.GetFieldInfo<SlimeGameManager>("bodyChangeTime").SetValue(SlimeGameManager.Instance, bodyChangeTime);
                }

                GUILayout.Space(20);
                GUILayout.Label("[Skill Cheat]", EditorStyles.boldLabel);

                //스킬 쿨탐 정함
                skillDelays[0] = EditorGUILayout.FloatField("Default Attack Delay", skillDelays[0]);
                skillDelays[1] = EditorGUILayout.FloatField("Special Attack1 Delay", skillDelays[1]);
                skillDelays[2] = EditorGUILayout.FloatField("Special Attack2 Delay", skillDelays[2]);

                if(GUILayout.Button("Apply Attack Delay"))  //스킬들의 쿨탐 적용
                {
                    for(int i=0; i<SlimeGameManager.Instance.SkillDelays.Length;i++)
                    {
                        SlimeGameManager.Instance.SkillDelays[i] = skillDelays[i];
                    }
                }

                GUILayout.Space(20);

                GUILayout.Label("[Status Cheat]", EditorStyles.boldLabel);

                recoveryHp = EditorGUILayout.IntField("Heal Current Hp n%", recoveryHp);  //회복량 세팅

                GUILayout.Space(7);

                if (GUILayout.Button("Player Heal"))  //회복
                {
                    SlimePlayer.GetHeal(recoveryHp);
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

                
                GUILayout.Label("[Page]", EditorStyles.boldLabel);
                if (GUILayout.Button("Open Out GitHub Page"))
                {
                    Application.OpenURL("https://github.com/DongyeonSeoDev/SemicolonProject");
                }

                EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                GUILayout.Space(20);
                GUILayout.Label("[ScreenShot]", EditorStyles.boldLabel);
                GUILayout.Label("(저장위치는 일단 Assets/ScreenShot)\n(아마 에디터에서만 스샷 가능)", EditorStyles.label);
                GUILayout.Label("단축키 : " + ScreenShot.captureKeyCode.ToString(), EditorStyles.label);

                GUILayout.Space(20);
                keyCode = (KeyCode)EditorGUILayout.EnumPopup("스샷 단축키", keyCode);
                if (GUILayout.Button("스샷 단축키 변경"))
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
