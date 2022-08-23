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

    public void OnPointerEnter(PointerEventData eventData)  //���콺 ����
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)  //���콺 ����
    {
        isMouseOver = false;
    }

    private void Update()
    {
        if(isMouseOver && Input.GetMouseButtonDown(0))  //���콺�� UI���� �ö󰬰� ���콺 ����
        {
            isMouseDown = true;
            prevMousePos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0) && isMouseDown)  //���콺 ���� ���¿��µ� ��
        {
            isMouseDown = false;
        }

        if(isMouseDown && isMouseOver)  //���콺�� ��� �巡�� ��
        {
            Vector2 delta = Input.mousePosition - prevMousePos;

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            //vertical�� horizontal �������� ���� ��ũ�� ���� ����
            if (vertical && horizontal) pointerData.scrollDelta = new Vector2(delta.x * 0.1f, -delta.y * 0.003f);
            else if(!vertical && !horizontal) pointerData.scrollDelta = Vector2.zero;
            else pointerData.scrollDelta = vertical ? new Vector2(0f, -delta.y * 0.003f) : new Vector2(delta.x * 0.1f, 0f);
            OnScroll(pointerData);

            prevMousePos = Input.mousePosition;
        }

        if(isMouseOver && IsMouseWheelRolling && !isMouseDown)  //���콺�� ������ �ʰ� ���� ������
        {
            float delta = Input.GetAxis("Mouse ScrollWheel");

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.scrollDelta = new Vector2(0f, delta);

            swallowMouseWheelScrolls = false;
            OnScroll(pointerData);
            swallowMouseWheelScrolls = true;
        }
    }

    public override void OnScroll(PointerEventData data)  //��ũ�Ѻ� ���
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

            data.scrollDelta *= 2.5f;  //�巡�� �ӵ��� ���ؼ� ���콺 �� �ӵ��� ������ ��������
            base.OnScroll(data);
        }
    }


}
