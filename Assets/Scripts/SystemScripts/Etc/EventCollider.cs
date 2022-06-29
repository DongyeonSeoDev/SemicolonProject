using UnityEngine;

public class EventCollider : MonoBehaviour
{
    [SerializeField] private string eventKey;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareLayer(Global.playerLayer))
        {
            EventManager.TriggerEvent(eventKey);
            gameObject.SetActive(false);    
        }
    }
}
