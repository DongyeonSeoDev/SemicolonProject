using UnityEngine;
using System;

//아마 튜토리얼 전용
public class SurvivalMission : Mission
{
    private float survivalTime;
    private float survivalTimer;
    private Action<bool> endAction;

    private int cur;

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
        base.Start();
        survivalTimer = 0f;
    }

    public override void Update()
    {
        survivalTimer += Time.deltaTime;

        if (Mathf.FloorToInt(survivalTimer) != cur)
        {
            ++cur;
            SetMissionNameText($"{survivalTime}초 동안 살아남으세요 ({survivalTime - cur})");
        }

        if (survivalTimer > survivalTime)
        {
            isEnd = true;
        }
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = lv;
        cur = 0;
    }
}
