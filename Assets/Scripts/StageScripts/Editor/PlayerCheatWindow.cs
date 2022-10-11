using UnityEditor;
using UnityEngine;
using Water;

public class PlayerCheatWindow : EditorWindow
{
    public Player SlimePlayer => SlimeGameManager.Instance.Player;

    //Player ����
    //private Stat playerStat = new Stat();
    private int addStatPoint = 10;
    private float useEnergyAmount = 1f;
    private float upMountingPercentageValueWhenEnemyDead = 2f;
    private int upUnderstandingRateValueWhenEnemyDead = 1;

    private SerializedObject statSerializedObject;
    private SerializedProperty statSerializedProperty;

    //�� ��ü ������
    private float bodyChangeTime = 10f;

    //��ų ������
    private float[] skillDelays = new float[3];

    //ȸ����
    private int recoveryHp = 20;

    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("Cheat/Player Cheat")]
    public static void ShowPlayerCheatWindow()
    {
        GetWindow(typeof(PlayerCheatWindow));
    }

    private void Refresh()
    {
        statSerializedObject.ApplyModifiedProperties();
        statSerializedObject.Update();
    }

    private void OnEnable()
    {
        skillDelays = new float[3] { 0.2f, 0.2f, 0.7f };

        statSerializedObject = new SerializedObject(SlimePlayer);
        statSerializedProperty = statSerializedObject.FindProperty("playerStat").FindPropertyRelative("additionalEternalStat");

        Refresh();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.MinWidth(200), GUILayout.MaxWidth(1000), GUILayout.ExpandWidth(true), GUILayout.MinHeight(200), GUILayout.MaxHeight(1000), GUILayout.ExpandHeight(true));
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);

        GUILayout.Space(20);
        GUILayout.Label("[Player Cheat]", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(statSerializedProperty, new GUIContent("Player Addi Stat Data"));

        #region �ּ�
        //�÷��̾� ���� ����
        /*playerStat.additionalEternalStat.maxHp.statValue = EditorGUILayout.FloatField("Max Hp", playerStat.eternalStat.maxHp.statValue);
        playerStat.additionalEternalStat.minDamage.statValue = EditorGUILayout.FloatField("Min Damage", playerStat.eternalStat.minDamage.statValue);
        playerStat.additionalEternalStat.maxDamage.statValue = EditorGUILayout.FloatField("Max Damage", playerStat.eternalStat.maxDamage.statValue);
        playerStat.additionalEternalStat.defense.statValue = EditorGUILayout.FloatField("Defense", playerStat.eternalStat.defense.statValue);
        playerStat.additionalEternalStat.speed.statValue = EditorGUILayout.FloatField("Speed", playerStat.eternalStat.speed.statValue);

        if (GUILayout.Button("Apply Player Additional Stat")) //�÷��̾� ���� ����
        {
            SlimePlayer.PlayerStat.additionalEternalStat = playerStat.additionalEternalStat;
        }*/
        #endregion

        GUILayout.Space(10);

        addStatPoint = EditorGUILayout.IntField("Stat Point", addStatPoint);
        if (GUILayout.Button("Add Point"))
        {
            GameManager.Instance.savedData.userInfo.playerStat.currentStatPoint += addStatPoint;
        }

        GUILayout.Space(10);

        useEnergyAmount = EditorGUILayout.FloatField("Slime Attack Need Energe", useEnergyAmount); //���� ������ �Ҹ� ����

        GUILayout.Label("(������ ����. �� �ٲ�� �ٽ� Apply �ʿ��� ����)", EditorStyles.label);

        //���� ������ �Ҹ� ����
        if (GUILayout.Button("Apply Attack Need Energe"))
        {
            PlayerShoot shoot = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerShoot>();
            if (shoot != null)
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

        if (GUILayout.Button("Apply Attack Delay"))  //��ų���� ��Ž ����
        {
            for (int i = 0; i < SlimeGameManager.Instance.SkillDelays.Length; i++)
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

        EditorGUI.EndDisabledGroup();
        GUILayout.EndScrollView();
    }

    private void Update()
    {
        Refresh();
    }
}
