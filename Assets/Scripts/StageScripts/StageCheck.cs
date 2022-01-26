using UnityEngine;

public class StageCheck : MonoBehaviour
{
    private Collider2D col;

    public Door[] doors;

    public int enemyCount;
    public int stageNumber;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        SlimeEventManager.StartListening("PlayerDead", () =>
        {
            col.enabled = true;

            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].Open();
            }
        });
    }

    public void StageClear()
    {
        if (stageNumber == 2)
        {
            SlimeEventManager.TriggerEvent("GameClear");
        }
        else
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].Open();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].Close();

                Enemy.EnemyManager.Instance.stageCheck = this;
                Enemy.EnemyManager.Instance.enemyCount = enemyCount;
            }

            for (int i = 0; i < Enemy.EnemyManager.Instance.enemyList[stageNumber].Count; i++)
            {
                Enemy.EnemyManager.Instance.enemyList[stageNumber][i].MoveEnemy();
            }

            col.enabled = false;
        }
    }
}
