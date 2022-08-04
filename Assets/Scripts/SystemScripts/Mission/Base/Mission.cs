
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
    public abstract void End(bool breakDoor = false);  //문을 부수고 지나가서 End가 호출된건지

    public abstract void SetLv(DifficultyLevel lv); //위의 Start함수보다 빨리 실행

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
