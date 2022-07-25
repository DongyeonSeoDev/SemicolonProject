
public class AllKillMission : Mission
{
    public AllKillMission(string title) : base(title)
    {
        missionType = MissionType.ALLKILL;
    }
    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("StageClear", MissionSuccess);
    }

    public override void Start()
    {
        isEnd = false;
        isClear = false;
        EventManager.StartListening("StageClear", MissionSuccess);
    }

    public override void Update()
    {
        
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = DifficultyLevel.EASY;
    }

   
}
