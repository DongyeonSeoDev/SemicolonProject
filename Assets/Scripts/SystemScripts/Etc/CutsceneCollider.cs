using UnityEngine;

public class CutsceneCollider : MonoBehaviour
{
    [SerializeField] private string cutsceneID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareLayer(Global.playerLayer))
        {
            CutsceneManager.Instance.PlayCutscene(cutsceneID);
            gameObject.SetActive(false);
        }
    }
}
