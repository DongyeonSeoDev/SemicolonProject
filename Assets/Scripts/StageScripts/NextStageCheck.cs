using UnityEngine;

public class NextStageCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StageSelect.Instance.ShowUI();
        }
    }
}
