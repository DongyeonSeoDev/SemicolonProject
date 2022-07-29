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
        EventManager.StartListening("PlayerManyDrain", MissionSuccess); //3���� ��� ���� �Ŀ� �� ��� ���� ���Ѿ� ��. ȣ�� ���� ����
        EventManager.StartListening("StageClear", MissionFailure);
    }

    public override void Update()
    {
        
    }
}
