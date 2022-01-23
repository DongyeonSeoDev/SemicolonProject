using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    private Camera mainCam;
    private Canvas canvas;

    private void Start()
    {
        mainCam = Camera.main;

        canvas = GetComponent<Canvas>();
        canvas.worldCamera = mainCam;
    }
}
