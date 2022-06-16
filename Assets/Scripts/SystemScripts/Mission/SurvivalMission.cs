using UnityEngine;
public class SurvivalMission : Mission
{
    private float survivalTime;
    private float survivalTimer;

    public SurvivalMission(short id, string title, float survivalTime) : base(id, title)
    {
        this.survivalTime = survivalTime;  
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
