

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
        missionName = $"{s}회 이상 피격당하지 않고 클리어하세요 ({s}/{s})";
        EventManager.StartListening("PlayerGetDamaged", CheckDamageCount);
        EventManager.StartListening("StageClear", MissionSuccess);
    }

    public override void Start()
    {
        curDamagedCnt = 0;
        isEnd = false;
        isClear = false;
    }

    public override void Update()
    {
        
    }

    private void CheckDamageCount()
    {
        curDamagedCnt++;

        SetMissionNameText($"{heart}회 이상 피격당하지 않고 클리어하세요 ({heart-curDamagedCnt}/{heart})");

        if(curDamagedCnt > damagedLimCnt)  //미션 실패
        {
            MissionFailure();
        }
    }
}
