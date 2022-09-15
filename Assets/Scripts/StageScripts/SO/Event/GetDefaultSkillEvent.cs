using UnityEngine;

[CreateAssetMenu(fileName = "Get Default Skill Event", menuName = "Scriptable Object/Map Events/Get Default Skill Event", order = int.MaxValue)]
public class GetDefaultSkillEvent : MapEventSO  //Ʃ�丮�� 7���� ����
{

    public string enterSubDataId;

    private int tutoEnemyDeathCnt = 0;

    public override void OnEnterEvent()
    {
        tutoEnemyDeathCnt = 0;
        TalkUtil.ShowSubtitle(enterSubDataId);
        BattleUIManager.Instance.StartMission(MissionType.ABSORPTIONTUTORIAL2, DifficultyLevel.NORMAL);
        EventManager.TriggerEvent("SpawnEnemy", "Stage0-07");
        EventManager.StartListening("EnemyDead", CheckTutoEnemyDead);
    }

    private void CheckTutoEnemyDead(GameObject o, string str, bool b)
    {
        Debug.Log("tutoEnemyDeathCnt : " + tutoEnemyDeathCnt);
        if (++tutoEnemyDeathCnt == 3)
        {
            StageManager.Instance.StageClear();
            EventManager.StopListening("EnemyDead", CheckTutoEnemyDead);
        }
    }
}
