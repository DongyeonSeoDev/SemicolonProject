using UnityEngine;
public class AllKillMission : Mission
{
    public AllKillMission(string title) : base(title)
    {
        missionType = MissionType.ALLKILL;
    }
    public override void End(bool breakDoor = false)
    {
       
    }

    public override void Start()
    {
        isEnd = false;
        
    }

    public override void Update()
    {
        
    }


}
