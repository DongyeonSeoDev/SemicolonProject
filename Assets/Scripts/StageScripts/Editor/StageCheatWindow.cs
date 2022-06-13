using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Water;

public class StageCheatWindow : EditorWindow
{
    public Player SlimePlayer => SlimeGameManager.Instance.Player;

    //private bool useClearStageKey = false;

    //stage ����
    private int floor;

    //Player ����
    private Stat playerStat = new Stat();
    private float useEnergyAmount = 1f;
    private float upMountingPercentageValueWhenEnemyDead = 2f;
    private int upUnderstandingRateValueWhenEnemyDead = 1;

    //�� ��ü ������
    private float bodyChangeTime = 10f;

    //��ų ������
    private float[] skillDelays = new float[3];

    //ȸ����
    private int recoveryHp = 20;
    //���� ����
    private StateAbnormality sa;

    //������ ����
    private string itemId;
    private int itemCnt;

    //�� ���̵�
    private Enemy.EnemyType mobId;

    //���� ����
    private int toolbarIdx;
    private string[] toolbars = { "Useable", "Util", "Temporary" };

    //���� ����Ű
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
            UIManager.Instance.RequestSystemMsg("�̹� �� �����ִ�");
            return;
        }

        StageManager.Instance.SetClearStage();
        UIManager.Instance.RequestSystemMsg("������ ���� ����");
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

                /*if(useClearStageKey && Event.current.isKey && Event.current.keyCode == KeyCode.F6)
                {
                    CurrentStageClear();
                    
                }*/
                

                GUILayout.Space(20);
                GUILayout.Label("[Player Cheat]", EditorStyles.boldLabel);

                //�÷��̾� ���� ����
                playerStat.eternalStat.maxHp.statValue = EditorGUILayout.FloatField("Max Hp", playerStat.eternalStat.maxHp.statValue);
                playerStat.eternalStat.minDamage.statValue = EditorGUILayout.FloatField("Min Damage", playerStat.eternalStat.minDamage.statValue);
                playerStat.eternalStat.maxDamage.statValue = EditorGUILayout.FloatField("Max Damage", playerStat.eternalStat.maxDamage.statValue);
                playerStat.eternalStat.defense.statValue = EditorGUILayout.FloatField("Defense", playerStat.eternalStat.defense.statValue);
                playerStat.eternalStat.speed.statValue = EditorGUILayout.FloatField("Speed", playerStat.eternalStat.speed.statValue);

                GUILayout.Space(10);

                if (GUILayout.Button("Apply Player Eternal Stat")) //�÷��̾� ���� ����
                {
                    SlimePlayer.PlayerStat = playerStat;
                }

                GUILayout.Space(10);

                useEnergyAmount = EditorGUILayout.FloatField("Slime Attack Need Energe", useEnergyAmount); //���� ������ �Ҹ� ����

                GUILayout.Space(5);

                GUILayout.Label("(������ ����. �� �ٲ�� �ٽ� Apply �ʿ��� ����)", EditorStyles.label);

                //���� ������ �Ҹ� ����
                if (GUILayout.Button("Apply Attack Need Energe"))
                {
                    PlayerShoot shoot = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerShoot>();
                    if(shoot != null)
                    {
                        shoot.GetFieldInfo<PlayerShoot>("useEnergyAmount").SetValue(shoot, useEnergyAmount);
                    }
                }

                GUILayout.Space(20);
                //���Ȯ�� ����
                upMountingPercentageValueWhenEnemyDead = EditorGUILayout.FloatField("Drain Probability", upMountingPercentageValueWhenEnemyDead);
                if (GUILayout.Button("Apply Drain Probability")) //���Ȯ�� ����
                {             
                    SlimePlayer.GetFieldInfo<Player>("upMountingPercentageValueWhenEnemyDead").SetValue(SlimePlayer, upMountingPercentageValueWhenEnemyDead);
                }

                GUILayout.Space(10);
                //���� ���� �� ������ ��ȭ���� �� ����
                upUnderstandingRateValueWhenEnemyDead = EditorGUILayout.IntField("Up Understanding Rate", upUnderstandingRateValueWhenEnemyDead);
                GUILayout.Label("(���� ���� �� ������ ��ȭ���� ��)", EditorStyles.label);
                if (GUILayout.Button("Apply Understanding Rate"))  //���� ���� �� ������ ��ȭ���� ��
                {                    
                    SlimePlayer.GetFieldInfo<Player>("upUnderstandingRateValueWhenEnemyDead").SetValue(SlimePlayer, upUnderstandingRateValueWhenEnemyDead);
                }

                GUILayout.Space(20);
                GUILayout.Label("[Body Change Cheat]", EditorStyles.boldLabel);

                bodyChangeTime = EditorGUILayout.FloatField("BodyChangeTime", bodyChangeTime);  //�� ��ü ������ ����
                if (GUILayout.Button("Apply Body Change Time"))  //�� ��ü ������ ����
                {
                    SlimeGameManager.Instance.GetFieldInfo<SlimeGameManager>("bodyChangeTime").SetValue(SlimeGameManager.Instance, bodyChangeTime);
                }

                GUILayout.Space(20);
                GUILayout.Label("[Skill Cheat]", EditorStyles.boldLabel);

                //��ų ��Ž ����
                skillDelays[0] = EditorGUILayout.FloatField("Default Attack Delay", skillDelays[0]);
                skillDelays[1] = EditorGUILayout.FloatField("Special Attack1 Delay", skillDelays[1]);
                skillDelays[2] = EditorGUILayout.FloatField("Special Attack2 Delay", skillDelays[2]);

                if(GUILayout.Button("Apply Attack Delay"))  //��ų���� ��Ž ����
                {
                    for(int i=0; i<SlimeGameManager.Instance.SkillDelays.Length;i++)
                    {
                        SlimeGameManager.Instance.SkillDelays[i] = skillDelays[i];
                    }
                }

                GUILayout.Space(20);

                GUILayout.Label("[Status Cheat]", EditorStyles.boldLabel);

                recoveryHp = EditorGUILayout.IntField("Heal Current Hp n%", recoveryHp);  //ȸ���� ����

                GUILayout.Space(7);

                if (GUILayout.Button("Player Heal"))  //ȸ��
                {
                    SlimePlayer.GetHeal(recoveryHp);
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

                
                GUILayout.Label("[Page]", EditorStyles.boldLabel);
                if (GUILayout.Button("Open Out GitHub Page"))
                {
                    Application.OpenURL("https://github.com/DongyeonSeoDev/SemicolonProject");
                }

                EditorGUI.BeginDisabledGroup(!Application.isPlaying);
                GUILayout.Space(20);
                GUILayout.Label("[ScreenShot]", EditorStyles.boldLabel);
                GUILayout.Label("(������ġ�� �ϴ� Assets/ScreenShot)\n(�Ƹ� �����Ϳ����� ���� ����)", EditorStyles.label);
                GUILayout.Label("����Ű : " + ScreenShot.captureKeyCode.ToString(), EditorStyles.label);

                GUILayout.Space(20);
                keyCode = (KeyCode)EditorGUILayout.EnumPopup("���� ����Ű", keyCode);
                if (GUILayout.Button("���� ����Ű ����"))
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
