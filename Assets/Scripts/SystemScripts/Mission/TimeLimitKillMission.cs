using UnityEngine;

public class TimeLimitKillMission : Mission
{
    private float limit;
    private float limitTimer;
    private int rest;

    private bool danger;
    private readonly float dangerTime = 10f;

    private PlayerState playerState;    

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
                limit = 100f;
                break;
            case DifficultyLevel.NORMAL:
                limit = 70f;
                break;
            case DifficultyLevel.HARD:
                limit = 45f;
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

        if (!playerState) playerState = Global.CurrentPlayer.GetComponent<PlayerState>();
        danger = false;
    }

    public override void Update()
    {
        if(!isEnd && !isClear && !playerState.IsDrain)
        {
            limitTimer -= Time.deltaTime;

            if(!danger && limitTimer < dangerTime)
            {
                danger = true;
                BattleUIManager.Instance.ShakeMissionPanel(0.4f, 10);
            }

            if(Mathf.CeilToInt(limitTimer) != rest)
            {
                string s = !danger ? $"{(int)limit}초 이내로 클리어하세요 ({--rest})" : $"{(int)limit}초 이내로 클리어하세요 (<color=red>{--rest}</color>)";
                SetMissionNameText(s);
            }

            if (limitTimer <= 0f)
            {
                MissionFailure();
            }
        }
    }
}
