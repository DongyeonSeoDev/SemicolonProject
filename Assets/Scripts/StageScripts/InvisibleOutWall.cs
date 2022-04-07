using UnityEngine;

public class InvisibleOutWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareLayer(Global.playerLayer) && !SlimeGameManager.Instance.Player.PlayerState.IsDrain)
        {
            UIManager.Instance.StartLoading(() => collision.transform.position = StageManager.Instance.MapCenterPoint, null, 0.4f, 0.2f, 0.3f);
        }
    }
}
