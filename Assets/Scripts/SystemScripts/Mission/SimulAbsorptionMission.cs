public class SimulAbsorptionMission : Mission
{
    public SimulAbsorptionMission(string title) : base(title)
    {
        missionType = MissionType.SIMULABSORPTION;
    }

    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("PlayerManyDrain", MissionSuccess);
        EventManager.StopListening("StageClear", MissionFailure);
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = DifficultyLevel.HARD;
    }

    public override void Start()
    {
        isEnd = false;
        isClear = false;
        EventManager.StartListening("PlayerManyDrain", MissionSuccess); //3마리 흡수 판정 후에 몹 사망 판정 시켜야 함. 호출 순서 주의
        EventManager.StartListening("StageClear", MissionFailure);
    }

    public override void Update()
    {
        
    }
}
