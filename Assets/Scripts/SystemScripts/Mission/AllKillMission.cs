
public class AllKillMission : Mission
{
    private int kill;
    private int monsterCount;

    private readonly float[] killRate = new float[3] { 0.2f, 0.5f, 0.8f };

    public AllKillMission(string title) : base(title)
    {
        missionType = MissionType.ALLKILL;
    }
    public override void End(bool breakDoor = false)
    {
        EventManager.StopListening("EnemyDead", EnemyDead);
        EventManager.StopListening("StageClear", MissionSuccess);
    }

    public override void Start()
    {
        base.Start();
        EventManager.StartListening("EnemyDead", EnemyDead);
        EventManager.StartListening("StageClear", MissionSuccess);
    }

    public override void Update()
    {
        
    }

    public override void SetLv(DifficultyLevel lv)
    {
        missionLevel = DifficultyLevel.EASY;
        kill = 0;
        monsterCount = StageManager.Instance.GetCurStageEnemyCount();
        missionName = $"��� ���� óġ�ϼ��� (<color=red>{kill}</color>/{monsterCount})";
    }

    private void EnemyDead(UnityEngine.GameObject o, string s, bool b)  //�Ű������� �׳� Ÿ�Կ� ���߱� ���Ѱ�.
    {
        float rate = ++kill / (float)monsterCount;
        string t = "";
        if (rate < killRate[0]) t = $"��� ���� óġ�ϼ��� (<color=red>{kill}</color>/{monsterCount})";
        else if(rate < killRate[1]) t = $"��� ���� óġ�ϼ��� (<color=#FF7600>{kill}</color>/{monsterCount})";
        else t = $"��� ���� óġ�ϼ��� ({kill}/{monsterCount})";
        SetMissionNameText(t);
    }
}
