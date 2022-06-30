

public class AllAbsorptionMission : Mission
{
    public AllAbsorptionMission(string title) : base(title)
    {
        missionType = MissionType.ALLABSORPTION;
    }
    public override void End(bool breakDoor = false)
    {
        Enemy.EnemyManager.Instance.isOnlyAbsorption = false;
    }

    public override void Start()
    {
        isEnd = false;
        Enemy.EnemyManager.Instance.isOnlyAbsorption = true;
    }

    public override void Update()
    {
       
    }
}
