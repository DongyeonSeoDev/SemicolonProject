using UnityEngine.UI;
using UnityEngine;

public class InventoryUI : Water.GameUI
{
    private RectMask2D rectMask;

    private bool onTransition = false, offTransition = false;

    public float speed = 13f;
    public float padding = 650;

    private void Awake()
    {
        rectMask = GetComponent<RectMask2D>();
    }

    private void Update()
    {
        if(onTransition)
        {
            if(rectMask.padding.y > 0)
            {
                rectMask.padding = new Vector4(0, rectMask.padding.y - speed, rectMask.padding.z - speed, 0);
            }
            else
            {
                rectMask.padding = Vector4.zero;
                onTransition = false;
            }
        }
        if(offTransition)
        {
            if (rectMask.padding.y < padding)
                rectMask.padding = new Vector4(0, rectMask.padding.y + speed, rectMask.padding.z + speed, 0);
            else
                onTransition = false;
        }
    }

    public override void ActiveTransition(UIType type)
    {
        offTransition = false;
        rectMask.padding = new Vector4(0, padding, padding, 0);
        onTransition = true;
    }

    public override void InActiveTransition()
    {
        onTransition = false;
        offTransition = true;
    }
}
