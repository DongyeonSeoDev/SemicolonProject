using UnityEngine;

public class StageGround : MonoBehaviour
{
    [SerializeField] private int enemyCount;
    public int EnemyCnt => enemyCount;

    public Collider2D camStageCollider;

    [SerializeField] private bool autoInsertStageDoors;
    public StageDoor[] stageDoors;

    private void Awake()
    {
        if(autoInsertStageDoors)
            stageDoors = GetComponentsInChildren<StageDoor>();
    }

    private void OnEnable()
    {
        Enemy.EnemyManager.Instance.enemyCount = enemyCount;
    }

    public void OpenDoors()
    {
        for(int i=0; i<stageDoors.Length; i++)
        {
            stageDoors[i].Open();
        }
    }

    public void CloseDoor()
    {
        for (int i = 0; i < stageDoors.Length; i++)
        {
            stageDoors[i].Close();
        }
    }
}
