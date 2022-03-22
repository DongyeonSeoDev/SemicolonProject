using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Start()
    {
        OnGameStart();
    }
    private void Update()
    {
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseClick = Input.GetMouseButton(0);

        CheckOnEnemy();

        if (isOnEnemy)
        {
            SetCursor(mouseClick ? "OnEnemyClickedCursor" : "OnEnemyCursor");
        }
        else
        {
            SetCursor(mouseClick ? "DefaultClickedCursor" : "DefaultCursor");
        }
    }

    private void CheckOnEnemy()
    {
        RaycastHit2D hit = Physics2D.CircleCast(cursorPosition, 0.1f, Vector2.up, 0.1f, whatIsEnemy);

        isOnEnemy = hit;
    }

    private void OnGameStart()
    {
#if UNITY_EDITOR
        SetCursor("DefaultCursor");
#endif
    }

    private void SetCursor(string cursorFileName)
    {
        Util.DelayFunc(() => Cursor.lockState = CursorLockMode.Confined, 5);
        cursorTexture = Resources.Load<Texture2D>("System/Sprites/Cursor/" + cursorFileName);
        Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.ForceSoftware);
    }
}
