using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Image setting;
    public Image[] childImgs;

    public Image viewportImg;
    public Scrollbar scrollBar;

    public void InitSet()
    {
        setting.GetComponent<DissolveCtrl>().SetFade(1);
        for(int i = 0; i < childImgs.Length; i++)
        {
            childImgs[i].material = setting.material;
        }
        
    }

    public void SetChildImgs(bool isDissolve)  //material ����� ���¿����� Mask���� �ۿ����� ����. (Rect Mask 2D���� �Ⱥ������� �ٷ� ������� �����)
    {
        if (!isDissolve)
        {
            scrollBar.value = 1;
            
        }
        viewportImg.raycastTarget = !isDissolve;

        Material mat = isDissolve ? setting.material : null;
        for(int i=0; i<childImgs.Length; i++)
        {
            childImgs[i].material = mat;
        }

        
    }
}
