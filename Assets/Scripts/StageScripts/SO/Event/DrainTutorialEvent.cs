using UnityEngine;

[CreateAssetMenu(fileName = "Drain Tutorial Event", menuName = "Scriptable Object/Drain Tutorial Event", order = int.MaxValue)]
public class DrainTutorialEvent : MapEventSO
{
    private int tutoEnemyDeathCnt = 0;

    public override void OnEnterEvent()
    {
        tutoEnemyDeathCnt = 0;
        EventManager.StartListening("Tuto_EnemyDeathCheck", CheckTutoEnemyDead);
    }

    private void CheckTutoEnemyDead()
    {
        Debug.Log("tutoEnemyDeathCnt : " + tutoEnemyDeathCnt);
        if(++tutoEnemyDeathCnt == 2)
        {
            StageManager.Instance.SetClearStage();
            EventManager.StopListening("Tuto_EnemyDeathCheck", CheckTutoEnemyDead);
        }
    }
}
