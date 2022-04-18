using UnityEngine;

public class SlimeFollowObj : MonoBehaviour
{
    Transform target;
    public Vector3 offset = Vector3.zero;

  
    private void Update()
    {
        target = SlimeGameManager.Instance.CurrentPlayerBody.transform;
        if(target)
        {
            transform.position = target.position + offset;
        }
    }
}
