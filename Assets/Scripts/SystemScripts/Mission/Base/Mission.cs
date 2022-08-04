
public abstract class Mission 
{
    public bool isEnd;
    public bool isClear;
    public MissionType missionType;
    public DifficultyLevel missionLevel;
    public string missionName;

    public static float[] expReward = new float[3] {300, 700, 1500};
    public static float[] assimReward = new float[3] {10, 35, 80};

    public Mission() { }
    public Mission(string title)
    {
        missionName = title;    
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void End(bool breakDoor = false);  //���� �μ��� �������� End�� ȣ��Ȱ���

    public abstract void SetLv(DifficultyLevel lv); //���� Start�Լ����� ���� ����

    public virtual void MissionFailure()
    {
        if (isEnd) return;

        //�ӽ� �̼� ���� �˸�
        UIManager.Instance.InsertTopCenterNoticeQueue("�̼� ����", BattleUIManager.Instance.missionFailVG, 70, null, 1.5f);

        isEnd = true;
    }

    public virtual void MissionSuccess()
    {
        if (isEnd) return;

        //�ӽ� �̼� ���� �˸�
        UIManager.Instance.InsertTopCenterNoticeQueue("�̼� ����", 70, null, 1.5f);

        int count = MonsterCollection.Instance.GetSavedMonsterBodyCount();
        if (UnityEngine.Random.Range(0, 2) == 0 || count == 0)
        { 
            float er = expReward[(int)missionLevel];
            NGlobal.playerStatUI.AddPlayerStatPointExp(er);
        }
        else
        {
            MonsterCollection.Instance.SetMonsterAssim((int)assimReward[(int)missionLevel]);
        }

        isClear = true;
        isEnd = true;
    }

    public virtual void SetMissionNameText(string msName)
    {
        BattleUIManager.Instance.missionContent.text = missionName = msName;
    }
}
