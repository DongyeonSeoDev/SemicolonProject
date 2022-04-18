using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    private UIColor colorTr;
    private Image img; 

    public void OnSelected(bool onClick)
    {
        if(colorTr == null)
        {
            colorTr = GetComponent<UIColor>();
            img = GetComponent<Image>();    
        }

        colorTr.transitionEnable = !onClick;
        img.color.SetColorAlpha(onClick ? 100 : 0);
    }
}
