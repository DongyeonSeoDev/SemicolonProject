
public abstract class Mission 
{
    public bool isEnd;
    public bool isClear;
    public MissionType missionType;
    public DifficultyLevel missionLevel;
    public string missionName;

    public Mission() { }
    public Mission(string title)
    {
        missionName = title;    
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void End(bool breakDoor = false);  //���� �μ��� �������� End�� ȣ��Ȱ���

    public abstract void SetLv(DifficultyLevel lv);

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

        NGlobal.playerStatUI.AddPlayerStatPointExp(250);

        isClear = true;
        isEnd = true;
    }
}
