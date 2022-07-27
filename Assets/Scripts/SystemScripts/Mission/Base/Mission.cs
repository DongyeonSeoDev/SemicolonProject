
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
    public abstract void End(bool breakDoor = false);  //문을 부수고 지나가서 End가 호출된건지

    public abstract void SetLv(DifficultyLevel lv);

    public virtual void MissionFailure()
    {
        if (isEnd) return;

        //임시 미션 실패 알림
        UIManager.Instance.InsertTopCenterNoticeQueue("미션 실패", BattleUIManager.Instance.missionFailVG, 70, null, 1.5f);

        isEnd = true;
    }

    public virtual void MissionSuccess()
    {
        if (isEnd) return;

        //임시 미션 성공 알림
        UIManager.Instance.InsertTopCenterNoticeQueue("미션 성공", 70, null, 1.5f);

        NGlobal.playerStatUI.AddPlayerStatPointExp(250);

        isClear = true;
        isEnd = true;
    }
}
