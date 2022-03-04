using UnityEngine;
using UnityEngine.UI;

public class CustomContentsSizeFilter : MonoBehaviour
{
    private RectTransform rectTrm;

    [SerializeField] private RectTransform contentRectTrm;

    [Space(15)]
    [SerializeField] private bool baseToTextLength;
    [SerializeField] private Text contentText;
    public float offsetPerChar = 15f;

    [Space(15)]
    public float widthOffset = 10f;
    public float heightOffset = 10f;

    public float minWidth = 20f;
    public float minHeight = 100f;

    public float maxWidth = 5000f;
    public float maxHeight = 5000f;

    private void Awake()
    {
        rectTrm = GetComponent<RectTransform>();
        if(!contentRectTrm) contentRectTrm = transform.GetChild(0).GetComponent<RectTransform>();
        if(baseToTextLength && !contentText) contentText = contentRectTrm.GetComponent<Text>(); 
    }

    public void UpdateSize()
    {
        if (!baseToTextLength)
        {
            rectTrm.sizeDelta = new Vector2(Mathf.Clamp(contentRectTrm.rect.width + widthOffset, minWidth, maxWidth),
                Mathf.Clamp(contentRectTrm.rect.height + heightOffset, minHeight, maxHeight));
        }
        else
        {
            rectTrm.sizeDelta = new Vector2(Mathf.Clamp(contentText.text.Length * offsetPerChar, minWidth, maxWidth),
                Mathf.Clamp(contentRectTrm.rect.height + heightOffset, minHeight, maxHeight));
        }
    }

    [ContextMenu("UpdateSize")]
    void UpdateSizeForEditor()
    {
        if (!baseToTextLength)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(contentRectTrm.rect.width + widthOffset, minWidth, maxWidth),
            Mathf.Clamp(contentRectTrm.rect.height + heightOffset, minHeight, maxHeight));
        }
        else
        {
            rectTrm.sizeDelta = new Vector2(Mathf.Clamp(contentText.text.Length * offsetPerChar, minWidth, maxWidth),
                Mathf.Clamp(contentRectTrm.rect.height + heightOffset, minHeight, maxHeight));
        }
    }
}
