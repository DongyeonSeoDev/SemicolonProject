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

    public void OnPointerEnter(PointerEventData eventData)  //마우스 오버
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)  //마우스 나감
    {
        isMouseOver = false;
    }

    private void Update()
    {
        if(isMouseOver && Input.GetMouseButtonDown(0))  //마우스가 UI위에 올라갔고 마우스 누름
        {
            isMouseDown = true;
            prevMousePos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0) && isMouseDown)  //마우스 눌림 상태였는데 뗌
        {
            isMouseDown = false;
        }

        if(isMouseDown && isMouseOver)  //마우스를 대고 드래그 함
        {
            Vector2 delta = Input.mousePosition - prevMousePos;

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            //vertical과 horizontal 변숫값에 따라서 스크롤 방향 결정
            if (vertical && horizontal) pointerData.scrollDelta = new Vector2(delta.x * 0.1f, -delta.y * 0.003f);
            else if(!vertical && !horizontal) pointerData.scrollDelta = Vector2.zero;
            else pointerData.scrollDelta = vertical ? new Vector2(0f, -delta.y * 0.003f) : new Vector2(delta.x * 0.1f, 0f);
            OnScroll(pointerData);

            prevMousePos = Input.mousePosition;
        }

        if(isMouseOver && IsMouseWheelRolling && !isMouseDown)  //마우스를 대지는 않고 휠을 움직임
        {
            float delta = Input.GetAxis("Mouse ScrollWheel");

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.scrollDelta = new Vector2(0f, delta);

            swallowMouseWheelScrolls = false;
            OnScroll(pointerData);
            swallowMouseWheelScrolls = true;
        }
    }

    public override void OnScroll(PointerEventData data)  //스크롤뷰 기능
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

            data.scrollDelta *= 2.5f;  //드래그 속도에 비해서 마우스 휠 속도가 느려서 보정해줌
            base.OnScroll(data);
        }
    }


}
