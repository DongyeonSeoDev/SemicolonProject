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
            isOnEnemy = hit;

            changeCursor = true;
        }
    }

    private void OnGameStart()
    {
#if !UNITY_EDITOR
        Util.DelayFunc(() => Cursor.lockState = CursorLockMode.Confined, 5);
#endif
        SetCursor("DefaultCursor");
    }

    private void SetCursor(string cursorFileName)
    {
        cursorTexture = Resources.Load<Texture2D>("System/Sprites/Cursor/" + cursorFileName);
        Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.ForceSoftware);
    }
}
