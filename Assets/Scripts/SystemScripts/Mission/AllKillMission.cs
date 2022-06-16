using UnityEngine;
public class AllKillMission : Mission
{
    public AllKillMission(string title) : base(title)
    {
        missionType = MissionType.ALLKILL;
    }
    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("StageClear", Clear);
    }

    public override void Start()
    {
        isEnd = false;
        EventManager.StartListening("StageClear", Clear);
    }

    public override void Update()
    {
        
    }

    private void Clear()
    {
        isEnd = true;
    }
}
