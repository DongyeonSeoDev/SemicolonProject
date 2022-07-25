using System.Collections.Generic;
using UnityEngine;

public class SimulAbsorptionMission : Mission
{
    private readonly int simulMobCount = 3;

    public SimulAbsorptionMission(string title) : base(title)
    {
        missionType = MissionType.SIMULABSORPTION;
    }

    public override void End(bool breakDoor = false)
    {
        
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = DifficultyLevel.HARD;
    }

    public override void Start()
    {
        isEnd = false;
        isClear = false;
    }

    public override void Update()
    {
        
    }
}
