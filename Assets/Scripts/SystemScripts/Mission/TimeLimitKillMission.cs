using UnityEngine;

public class TimeLimitKillMission : Mission
{
    private float limit;
    private float limitTimer;
    private int rest;

    public TimeLimitKillMission() 
    {
        missionType = MissionType.TIMELIMITKILL;
    }
    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("StageClear", MissionSuccess);
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = lv;
        switch (lv)
        {
            case DifficultyLevel.EASY:
                limit = 60f;
                break;
            case DifficultyLevel.NORMAL:
                limit = 45f;
                break;
            case DifficultyLevel.HARD:
                limit = 30f;
                break;
        }
        limitTimer = limit;
        rest = (int)limit;
        missionName = rest.ToString() + "초 이내로 클리어하세요 (" + rest.ToString() + ")";
        EventManager.StartListening("StageClear", MissionSuccess);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if(!isEnd && !isClear)
        {
            limitTimer -= Time.deltaTime;

            if(Mathf.CeilToInt(limitTimer) != rest)
            {
                SetMissionNameText($"{(int)limit}초 이내로 클리어하세요 ({--rest})");
            }

            if (limitTimer <= 0f)
            {
                MissionFailure();
            }
        }
    }
}
