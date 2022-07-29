using UnityEngine;

public class ConseKillMission : Mission
{
    private int needConseKill;  //몇 마리 연속킬 해야함?
    private int curConseKill;  //현재 연속킬 마리 수

    private bool onTimer;  //타이머 체크중인지

    //몇 초 이내의 간격으로 몇 마리 처치가 아니라 몇 초 안에 몇 마리 처치라고 하더라?
    private float timer;  //몇 초 지났는지. 한 마리 죽이면 타이머 스타트 => onTimer true, 5초 지나면 초기화. 5초안에 목표 마리 연속킬 하면 성공
    private readonly float time = 5f;  //몇 초 안에 연속킬

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

        missionName = needConseKill.ToString() + "마리를 연속으로 처치하세요";
    }

    public override void Start()
    {
        isEnd = false;
        isClear = false;
        onTimer = false;
        curConseKill = 0;
        EventManager.StartListening("EnemyDead", EnemyDead);
    }

    public override void Update()
    {
        if (!isEnd)
        {
            if (onTimer)
            {
                timer += Time.deltaTime;

                if(timer > time && curConseKill < needConseKill)
                {
                    onTimer = false;
                    curConseKill = 0;
                }
            }
        }
    }

    private void EnemyDead(GameObject o, string s, bool b)  //매개변수는 그냥 타입에 맞추기 위한것.
    {
        curConseKill++;

        if (!onTimer)
        {
            onTimer = true;
            timer = 0f;
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
