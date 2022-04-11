using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Util.MainCam;
    }

    private void Update()
    {
        if (transform.rotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
