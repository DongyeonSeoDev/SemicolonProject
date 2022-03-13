using UnityEngine;

public class StageCheck : MonoBehaviour
{
    private Collider2D col;

    public Door[] doors;
    public Collider2D camStageCollider;

    public int enemyCount;
    public int stageNumber;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        EventManager.StartListening("AfterPlayerRespawn", () =>
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
            EventManager.TriggerEvent("GameClear");
        }
        else
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].Open();
            }
        }

        EventManager.TriggerEvent("StageClear");

        StageManager.Instance.IsStageClear = true;

        CinemachineCameraScript.Instance.SetCinemachineConfiner(CinemachineCameraScript.Instance.boundingCollider);
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

            EventManager.TriggerEvent("EnemyMove", stageNumber);

            col.enabled = false;

            StageManager.Instance.IsStageClear = false;

            CinemachineCameraScript.Instance.SetCinemachineConfiner(camStageCollider);
        }
    }
}
