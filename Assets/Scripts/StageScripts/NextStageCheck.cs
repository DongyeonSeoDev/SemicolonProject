using UnityEngine;

public class NextStageCheck : MonoBehaviour
{
    public StageSelect stageSelect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stageSelect.NextStage(0);
        }
    }
}
