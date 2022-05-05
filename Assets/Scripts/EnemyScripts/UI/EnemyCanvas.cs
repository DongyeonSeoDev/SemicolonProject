using UnityEngine;

public class EnemyCanvas : MonoBehaviour
{
    private Canvas canvas;

    protected virtual void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Util.MainCam;
    }
}
