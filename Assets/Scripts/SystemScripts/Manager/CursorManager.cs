using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoSingleton<CursorManager>
{
    [SerializeField]
    private LayerMask whatIsEnemy;

    private Vector2 cursorPosition = Vector2.zero;
    public Vector2 CursorPosition
    {
        get { return cursorPosition; }
    }

    private Texture2D cursorTexture;
    private Dictionary<string, Texture2D> cursorTextureDict = new Dictionary<string, Texture2D>();

    private readonly Vector2 cursorOffset = new Vector2(26f, 26f);

    private bool isOnEnemy = false;
    private bool mouseClick = false;

    private bool changeCursor = false;
    private void Start()
    {
        OnGameStart();
    }
    private void Update()
    {
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        CheckClick();
        CheckOnEnemy();
        SetCursor();
    }

    private void SetCursor()
    {
        if (changeCursor)
        {
            if (isOnEnemy)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    SetCursor(mouseClick ? "OnEnemyClickedCursor" : "OnEnemyCursor");
                }
            }
            else
            {
                SetCursor(mouseClick ? "DefaultClickedCursor" : "DefaultCursor");
            }

            changeCursor = false;
        }
    }

    private void CheckClick()
    {
        bool click = Input.GetMouseButton(0);

        if (click != mouseClick)
        {
            mouseClick = click;

            changeCursor = true;
        }
    }

    private void CheckOnEnemy()
    {
        RaycastHit2D hit = Physics2D.CircleCast(cursorPosition, 0.1f, Vector2.up, 0.1f, whatIsEnemy);

        if (isOnEnemy != hit)
        {
            changeCursor = true;
        }

        isOnEnemy = hit;
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
