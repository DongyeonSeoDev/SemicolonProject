using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private InteractionObj itrObj;

    private void Awake()
    {
        itrObj = transform.parent.GetComponent<InteractionObj>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareLayer(Global.playerLayer))
        {
            itrObj.SetUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareLayer(Global.playerLayer))
        {
            itrObj.SetUI(false);
        }
    }
}
