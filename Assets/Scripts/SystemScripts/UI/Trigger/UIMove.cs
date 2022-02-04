using UnityEngine;
using UnityEngine.EventSystems;

public class UIMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Transform moveUI;

    private bool isMoving = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isMoving = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isMoving) moveUI.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        isMoving = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isMoving = false;
    }
}
