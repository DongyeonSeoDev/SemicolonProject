using UnityEngine;

public class SlimeFollowObj : MonoBehaviour
{
    Transform target;

    private void Update()
    {
        target = SlimeGameManager.Instance.CurrentPlayerBody.transform;
        if(target)
        {
            transform.position = target.position;
        }
    }
}
