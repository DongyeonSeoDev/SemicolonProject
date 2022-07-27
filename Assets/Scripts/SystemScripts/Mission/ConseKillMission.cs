using UnityEngine;

public class ConseKillMission : Mission
{
    private int needConseKill;  //�� ���� ����ų �ؾ���?
    private int curConseKill;  //���� ����ų ���� ��

    private bool onTimer;  //Ÿ�̸� üũ������

    //�� �� �̳��� �������� �� ���� óġ�� �ƴ϶� �� �� �ȿ� �� ���� óġ��� �ϴ���?
    private float timer;  //�� �� ��������. �� ���� ���̸� Ÿ�̸� ��ŸƮ => onTimer true, 5�� ������ �ʱ�ȭ. 5�ʾȿ� ��ǥ ���� ����ų �ϸ� ����
    private readonly float time = 5f;  //�� �� �ȿ� ����ų

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

        missionName = needConseKill.ToString() + "������ �������� óġ�ϼ���";
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

    private void EnemyDead(GameObject o, string s, bool b)  //�Ű������� �׳� Ÿ�Կ� ���߱� ���Ѱ�.
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
