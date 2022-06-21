using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChiScrollRect : ScrollRect, IPointerEnterHandler, IPointerExitHandler
{
    private bool swallowMouseWheelScrolls = true;
    private bool isMouseOver = false;
    private bool isMouseDown = false;

    private Vector3 prevMousePos;

    private static bool IsMouseWheelRolling => Input.GetAxis("Mouse ScrollWheel") != 0;

    public static void SetAllChildRaycastTarget(Transform parent, bool active)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).GetComponent<Image>().raycastTarget = active;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    private void Update()
    {
        if(isMouseOver && Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            prevMousePos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0) && isMouseDown)
        {
            isMouseDown = false;
        }

        if(isMouseDown && isMouseOver)
        {
            Vector2 delta = Input.mousePosition - prevMousePos;

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.scrollDelta = new Vector2(0f, -delta.y * Time.deltaTime * 5f);
            OnScroll(pointerData);

            prevMousePos = Input.mousePosition;
        }

        if(isMouseOver && IsMouseWheelRolling && !isMouseDown)
        {
            float delta = Input.GetAxis("Mouse ScrollWheel");

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.scrollDelta = new Vector2(0f, delta);

            swallowMouseWheelScrolls = false;
            OnScroll(pointerData);
            swallowMouseWheelScrolls = true;
        }
    }

    public override void OnScroll(PointerEventData data)
    {
        if(IsMouseWheelRolling && swallowMouseWheelScrolls)
        {

        }
        else
        {
            if (data.scrollDelta.y <= -Mathf.Epsilon)
                data.scrollDelta = new Vector2(0, -scrollSensitivity);
            else if (data.scrollDelta.y > Mathf.Epsilon)
                data.scrollDelta = new Vector2(0f, scrollSensitivity);

            base.OnScroll(data);
        }
    }


}
