using UnityEngine;

public class ConseKillMission : Mission
{
    private int needConseKill;

    public ConseKillMission()
    {
        missionType = MissionType.CONSEKILL;
    }
    public override void End(bool breakDoor = false)
    {
        
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

        missionName = needConseKill.ToString() + "������ �������� óġ�ϼ���";
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
