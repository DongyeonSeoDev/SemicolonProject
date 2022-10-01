using UnityEngine;

public class FPSCheck : MonoBehaviour
{
    [Header("에디터에서만 보이게 할지")]
    public bool isEditor = true;

    [Range(1,100)]
    public int fontSize = 15;
    private int w, h;

    public Color textColor;

    private float deltaTime = 0.0f;

    private float ms, fps;

    private void Start()
    {
        textColor = Color.white;

        if(isEditor)
        {
#if !UNITY_EDITOR
            enabled = false;
#endif
        }
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        w = Screen.width;
        h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 0.02f);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / (100 - fontSize);
        style.normal.textColor = textColor;

        ms = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0}ms ({1:0.}fps)", ms, fps);
        GUI.Label(rect, text, style);
    }
}
