using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Image setting;
    public Image[] childImgs;

    public Image viewportImg;

    public RectTransform contentsRectTr;
    private Vector3 startRectPos;

    public void InitSet()
    {
        setting.GetComponent<DissolveCtrl>().SetFade(1);
        for(int i = 0; i < childImgs.Length; i++)
        {
            childImgs[i].material = setting.material;
        }

        startRectPos = contentsRectTr.anchoredPosition;
    }

    public void SetChildImgs(bool isDissolve)  //material 적용된 상태에서는 Mask범위 밖에서도 보임. (Rect Mask 2D쓰면 안보이지만 바로 사라져서 어색함)
    {
        if (isDissolve)
        {
            contentsRectTr.anchoredPosition = startRectPos;
        }
        viewportImg.raycastTarget = !isDissolve;

        Material mat = isDissolve ? setting.material : null;
        for(int i=0; i<childImgs.Length; i++)
        {
            childImgs[i].material = mat;
        }

        
    }
}
