using UnityEngine;

public class InvisibleOutWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareLayer(Global.playerLayer))
        {
            collision.transform.position = StageManager.Instance.MapCenterPoint;
            Debug.Log(111111);
        }
    }
}
