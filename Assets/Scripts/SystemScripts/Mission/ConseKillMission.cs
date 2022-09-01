using UnityEngine;

public class ConseKillMission : Mission
{
    private int needConseKill;  //�� ���� ����ų �ؾ���?
    private int curConseKill;  //���� ����ų ���� ��

    private bool onTimer;  //Ÿ�̸� üũ������

    private int iTimer;  //�� �� �������� (ȭ�鿡 ǥ�ÿ�����)

    //�� �� �̳��� �������� �� ���� óġ
    private float timer;  //�� �� ��������. �� ���� ���̸� Ÿ�̸� ��ŸƮ => onTimer true, �� �ϳ� t�� �̳��� ���̸� �ٽ� t�ʷ� ����
    private readonly float time = 5f;  //�� �� �ȿ� ����ų

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

        missionName = time.ToString() + "�� �̳��� ���� ����óġ�ϼ��� (0/" + needConseKill.ToString() + ")";
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

                if(Mathf.CeilToInt(time - timer)  < iTimer)  //�Ź� �̼� ������ �����ϸ� ������ ���� �߻��ؼ� .intŸ�����ε� �ð��� ��� 1�� ���� ������ ���� ����
                {
                    SetMissionNameText($"{time}�� �̳��� ���� ����óġ�ϼ��� ({curConseKill}/{needConseKill}) ({--iTimer})");
                }

                if(timer > time && curConseKill < needConseKill)
                {
                    onTimer = false;
                    curConseKill = 0;
                    BattleUIManager.Instance.ShakeMissionPanel(0.4f, 10);
                    SetMissionNameText($"{time}�� �̳��� ���� ����óġ�ϼ��� (0/{needConseKill})");
                }
            }
        }
    }

    private void EnemyDead(GameObject o, string s, bool b)  //�Ű������� �׳� Ÿ�Կ� ���߱� ���Ѱ�.
    {
        iTimer = (int)time;
        timer = 0f;
        SetMissionNameText($"{time}�� �̳��� ���� ����óġ�ϼ��� ({++curConseKill}/{needConseKill}) ({iTimer})");

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
