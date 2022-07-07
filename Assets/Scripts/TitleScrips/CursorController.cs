using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private Texture2D cursorTexture;
    private Dictionary<string, Texture2D> cursorTextureDict = new Dictionary<string, Texture2D>();

    private readonly Vector2 cursorOffset = new Vector2(26f, 26f);

    private bool mouseClick = false;

    private void Start()
    {
        OnGameStart();
    }

    private void Update()
    {
        CheckClick();
    }
    private void SetCursor(bool mouseClick)
    {
        SetCursor(mouseClick ? "DefaultClickedCursor" : "DefaultCursor");
    }
    private void CheckClick()
    {
        bool click = Input.GetMouseButton(0);

        if (click != mouseClick)
        {
            mouseClick = click;

            SetCursor(click);
        }
    }
    private void OnGameStart()
    {
#if !UNITY_EDITOR
        Util.DelayFunc(() => Cursor.lockState = CursorLockMode.Confined, 5);
#endif
        cursorTextureDict.Add("DefaultCursor", GetCursor("DefaultCursor"));
        cursorTextureDict.Add("DefaultClickedCursor", GetCursor("DefaultClickedCursor"));
        cursorTextureDict.Add("OnEnemyCursor", GetCursor("OnEnemyCursor"));
        cursorTextureDict.Add("OnEnemyClickedCursor", GetCursor("OnEnemyClickedCursor"));

        SetCursor("DefaultCursor");
    }
    private Texture2D GetCursor(string cursorFileName)
    {
        Texture2D texture2D = Resources.Load<Texture2D>("System/Sprites/Cursor/" + cursorFileName);

        return texture2D;
    }
    private void SetCursor(string cursorFileName)
    {
        if (cursorTextureDict.TryGetValue(cursorFileName, out cursorTexture))
        {
            Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.ForceSoftware);
        }
    }
}
