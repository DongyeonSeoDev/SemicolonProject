

public class NoDamagedMission : Mission
{
    private int curDamagedCnt;
    private int damagedLimCnt;

    private int heart;

    
    public NoDamagedMission()
    {
        missionType = MissionType.NODAMAGED;
    }
    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("PlayerGetDamaged", CheckDamageCount);
        EventManager.StopListening("StageClear", MissionSuccess);
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = lv;
        switch (lv)
        {
            case DifficultyLevel.EASY:
                damagedLimCnt = 3;
                break;
            case DifficultyLevel.NORMAL:
                damagedLimCnt = 1;
                break;
            case DifficultyLevel.HARD:
                damagedLimCnt = 0;
                break;
        }
        heart = damagedLimCnt + 1;
        string s = heart.ToString();
        missionName = damagedLimCnt > 0 ? $"{s}회 이상 피격당하지 않고 클리어하세요 ({s}/{s})" : $"{s}회 이상 피격당하지 않고 클리어하세요 (<color=red>{s}</color>/{s})";
        EventManager.StartListening("PlayerGetDamaged", CheckDamageCount);
        EventManager.StartListening("StageClear", MissionSuccess);
    }

    public override void Start()
    {
        curDamagedCnt = 0;
        base.Start();
    }

    public override void Update()
    {
        
    }

    private void CheckDamageCount()
    {
        curDamagedCnt++;
        int rest = heart - curDamagedCnt;
        if(rest > 1)
            SetMissionNameText($"{heart}회 이상 피격당하지 않고 클리어하세요 ({rest}/{heart})");
        else
            SetMissionNameText($"{heart}회 이상 피격당하지 않고 클리어하세요 (<color=red>{rest}</color>/{heart})");

        if (curDamagedCnt > damagedLimCnt)  //미션 실패
        {
            MissionFailure();
        }
        else
        {
            BattleUIManager.Instance.ShakeMissionPanel(0.4f, 10);
        }
    }
}
