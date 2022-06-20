

public class AllAbsorptionMission : Mission
{
    public AllAbsorptionMission(string title) : base(title)
    {
        missionType = MissionType.ALLABSORPTION;
    }
    public override void End(bool breakDoor = false)
    {
        
    }

    public override void Start()
    {
        isEnd = false;
        SlimeGameManager.Instance.PlayerBodyChange(Global.OriginBodyID);
    }

    public override void Update()
    {
       
    }
}
