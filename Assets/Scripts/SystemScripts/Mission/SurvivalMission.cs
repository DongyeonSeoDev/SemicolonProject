using UnityEngine;
using System;

//아마 튜토리얼 전용
public class SurvivalMission : Mission
{
    private float survivalTime;
    private float survivalTimer;
    private Action<bool> endAction;

    public SurvivalMission(string title, float survivalTime, Action<bool> end = null) : base(title)
    {
        this.survivalTime = survivalTime;
        missionType = MissionType.SURVIVAL1;
        this.endAction = end;
    }

    public override void End(bool breakDoor = false)
    {
        if(!breakDoor)
            StageManager.Instance.StageClear();
        endAction?.Invoke(breakDoor);
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
