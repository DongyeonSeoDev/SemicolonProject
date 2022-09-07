
public abstract class Mission 
{
    public bool isEnd;
    public bool isClear, isFail;
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

    public virtual void Start()
    {
        isEnd = false;
        isClear = false;
        isFail = false;
    }
    public abstract void Update();
    public abstract void End(bool breakDoor = false);  //문을 부수고 지나가서 End가 호출된건지

    public abstract void SetLv(DifficultyLevel lv); //위의 Start함수보다 빨리 실행

    public virtual void MissionFailure()
    {
        if (isFail) return;

        UIManager.Instance.InsertTopCenterNoticeQueue("미션 실패", BattleUIManager.Instance.missionFailVG, 70, null, 1.5f);

        isFail = true;
        isEnd = true;
    }

    public virtual void MissionSuccess()
    {
        if (isClear) return;

        UIManager.Instance.InsertTopCenterNoticeQueue("미션 성공", 70, null, 1.5f);

        int count = MonsterCollection.Instance.GetSavedMonsterBodyCount();
        if (UnityEngine.Random.Range(0, 2) == 0 || count == 0)
        { 
            float er = expReward[(int)missionLevel];
            NGlobal.playerStatUI.AddPlayerStatPointExp(er);
            UIManager.Instance.InsertNoticeQueue("스탯 포인트 경험치 <size=150%>" + er + "</size> 획득");
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
