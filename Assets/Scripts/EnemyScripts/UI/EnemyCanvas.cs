using UnityEngine;

public class EnemyCanvas : MonoBehaviour
{
    private Canvas canvas;

    protected virtual void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Util.MainCam;
    }

    protected virtual void Update()
    {
        if (transform.rotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
