using UnityEngine;

public class StageGround : MonoBehaviour
{
    public Collider2D camStageCollider;

    public Transform playerSpawnPoint;

    [SerializeField] private bool autoInsertStageDoors;
    public StageDoor[] stageDoors;

    [SerializeField] private bool autoInsertPlants;
    public Pick[] plants;

    private void Awake()
    {
        if(autoInsertStageDoors)
            stageDoors = GetComponentsInChildren<StageDoor>();
        if(autoInsertPlants)    
            plants = GetComponentsInChildren<Pick>();
    }

    private void OnEnable()
    {
        for(int i = 0; i < plants.Length; i++)
        {
            plants[i].gameObject.SetActive(true);
        }
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
