using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField]
    private LayerMask PlayerLayer;

    private InteractionObj itrObj;

    private void Awake()
    {
        itrObj = transform.parent.GetComponent<InteractionObj>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayerLayer.CompareGameObjectLayer(other.gameObject))
        {
            itrObj.SetUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (PlayerLayer.CompareGameObjectLayer(collision.gameObject))
        {
            itrObj.SetUI(false);
        }
    }
}
