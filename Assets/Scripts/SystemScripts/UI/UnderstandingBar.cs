using UnityEngine;

public class UnderstandingBar : MonoBehaviour
{
    private RectTransform rectTr;

    public Vector3 offset;

    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        Transform target = SlimeGameManager.Instance.CurrentPlayerBody.transform;

        if (target)
        {
            rectTr.anchoredPosition = Util.WorldToScreenPosForScreenSpace(target.position + offset, Util.WorldCvs);
        }
    }
}
