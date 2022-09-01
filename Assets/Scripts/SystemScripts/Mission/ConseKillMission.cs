using UnityEngine;

public class ConseKillMission : Mission
{
    private int needConseKill;  //몇 마리 연속킬 해야함?
    private int curConseKill;  //현재 연속킬 마리 수

    private bool onTimer;  //타이머 체크중인지

    private int iTimer;  //몇 초 지났는지 (화면에 표시용으로)

    //몇 초 이내의 간격으로 몇 마리 처치
    private float timer;  //몇 초 지났는지. 한 마리 죽이면 타이머 스타트 => onTimer true, 또 하나 t초 이내에 죽이면 다시 t초로 갱신
    private readonly float time = 5f;  //몇 초 안에 연속킬

    private PlayerState playerState;

    public ConseKillMission()
    {
        missionType = MissionType.CONSEKILL;
    }
    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("EnemyDead", EnemyDead);
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = lv; 
        switch (lv)
        {
            case DifficultyLevel.EASY:
                needConseKill = 3;
                break;
            case DifficultyLevel.NORMAL:
                needConseKill = 4;
                break;
            case DifficultyLevel.HARD:
                needConseKill = 5;
                break;
        }

        missionName = time.ToString() + "초 이내에 적을 연속처치하세요 (0/" + needConseKill.ToString() + ")";
    }

    public override void Start()
    {
        base.Start();
        onTimer = false;
        curConseKill = 0;
        if (!playerState) playerState = Global.CurrentPlayer.GetComponent<PlayerState>();
        EventManager.StartListening("EnemyDead", EnemyDead);
    }

    public override void Update()
    {
        if (!isEnd)
        {
            if (onTimer && !playerState.IsDrain)
            {
                timer += Time.deltaTime;

                if(Mathf.CeilToInt(time - timer)  < iTimer)  //매번 미션 제목을 갱신하면 연산이 많이 발생해서 .int타입으로도 시간을 재고 1초 깎일 때마다 제목 갱신
                {
                    SetMissionNameText($"{time}초 이내에 적을 연속처치하세요 ({curConseKill}/{needConseKill}) ({--iTimer})");
                }

                if(timer > time && curConseKill < needConseKill)
                {
                    onTimer = false;
                    curConseKill = 0;
                    BattleUIManager.Instance.ShakeMissionPanel(0.4f, 10);
                    SetMissionNameText($"{time}초 이내에 적을 연속처치하세요 (0/{needConseKill})");
                }
            }
        }
    }

    private void EnemyDead(GameObject o, string s, bool b)  //매개변수는 그냥 타입에 맞추기 위한것.
    {
        iTimer = (int)time;
        timer = 0f;
        SetMissionNameText($"{time}초 이내에 적을 연속처치하세요 ({++curConseKill}/{needConseKill}) ({iTimer})");

        if (!onTimer)
        {
            onTimer = true;
        }
        else
        {
            if(curConseKill == needConseKill)
            {
                MissionSuccess();
            }
        }
    }
}
