using UnityEngine;

public class ScreenToWorldPos : MonoBehaviour  //2d sprite를 UI처럼 보이게 할 때
{
    private Camera mainCam;

    public Vector3 screenPoint;
    public bool autoSetScrPoint;
    public float followSpeed = 3.5f;

    private void Start()
    {
        mainCam = Util.MainCam;
        if (autoSetScrPoint)
        {
            Vector3 curPos = transform.position;
            Vector3 camPos = Util.MainCam.transform.position;
            transform.position = new Vector3(curPos.x + camPos.x,curPos.y + camPos.y,0);
            screenPoint = Util.WorldToScreenPoint(transform.position);
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, mainCam.ScreenToWorldPoint(screenPoint), Time.deltaTime * followSpeed);
    }
}
