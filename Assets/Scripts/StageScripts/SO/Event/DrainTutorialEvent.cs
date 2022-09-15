using UnityEngine;

[CreateAssetMenu(fileName = "Drain Tutorial Event", menuName = "Scriptable Object/Map Events/Drain Tutorial Event", order = int.MaxValue)]
public class DrainTutorialEvent : MapEventSO
{
    private int tutoEnemyDeathCnt = 0;

    public string enterSubtitleId;

    public override void OnEnterEvent()
    {
        TalkUtil.ShowSubtitle(enterSubtitleId);
        EventManager.TriggerEvent("SpawnEnemy", "Stage0-05");
        BattleUIManager.Instance.StartMission(MissionType.ABSORPTIONTUTORIAL, DifficultyLevel.EASY);
        tutoEnemyDeathCnt = 0;
        EventManager.StartListening("EnemyDead", CheckTutoEnemyDead);
    }

    private void CheckTutoEnemyDead(GameObject o, string str, bool b)
    {
        Debug.Log("tutoEnemyDeathCnt : " + tutoEnemyDeathCnt);
        if(++tutoEnemyDeathCnt == 1)
        {
            StageManager.Instance.StageClear();
            EventManager.StopListening("EnemyDead", CheckTutoEnemyDead);
        }
    }
}
