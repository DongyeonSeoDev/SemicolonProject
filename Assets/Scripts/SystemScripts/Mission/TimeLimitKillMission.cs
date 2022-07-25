using UnityEngine;

public class TimeLimitKillMission : Mission
{
    private float limitTime;

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
        switch (lv)
        {
            case DifficultyLevel.EASY:
                limitTime = 60f;
                break;
            case DifficultyLevel.NORMAL:
                limitTime = 45f;
                break;
            case DifficultyLevel.HARD:
                limitTime = 30f;
                break;
        }
        missionName = ((int)limitTime).ToString() + "초 이내로 클리어하세요";
        EventManager.StartListening("StageClear", MissionSuccess);
    }

    public override void Start()
    {
        isEnd = false;
        isClear = false;
    }

    public override void Update()
    {
        if(!isEnd && !isClear)
        {
            limitTime -= Time.deltaTime;
            if (limitTime <= 0f)
            {
                MissionFailure();
            }
        }
    }
}
