using UnityEngine;
public class SurvivalMission : Mission
{
    private float survivalTime;
    private float survivalTimer;

    public SurvivalMission(string title, float survivalTime) : base(title)
    {
        this.survivalTime = survivalTime;
        missionType = MissionType.SURVIVAL;
    }

    public override void End(bool breakDoor = false)
    {
        if(!breakDoor)
            StageManager.Instance.StageClear();
    }

    public override void Start()
    {
        isEnd = false;
        survivalTimer = 0f;
    }

    public override void Update()
    {
        survivalTimer += Time.deltaTime;

        if(survivalTimer > survivalTime)
        {
            isEnd = true;
        }
    }
}
