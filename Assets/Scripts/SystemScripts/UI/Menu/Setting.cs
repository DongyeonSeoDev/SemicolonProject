using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : GameUI
{
    public override void ExceptionHandle()
    {
        EffectManager em = EffectManager.Instance;
        if (!gameObject.activeSelf)
        {
            StoredData.SetValueKey("InvenBtnEffActive", em.inventoryBtnEffect.activeSelf);
            StoredData.SetValueKey("StatBtnEffActive", em.statBtnEffect.activeSelf);
            StoredData.SetValueKey("MonColBtnEffActive", em.monColBtnEffect.activeSelf);
            
            em.inventoryBtnEffect.SetActive(false);
            em.statBtnEffect.SetActive(false);
            em.monColBtnEffect.SetActive(false);
        }
        else
        {
            em.inventoryBtnEffect.SetActive(StoredData.GetValueData<bool>("InvenBtnEffActive"));
            em.statBtnEffect.SetActive(StoredData.GetValueData<bool>("StatBtnEffActive"));
            em.monColBtnEffect.SetActive(StoredData.GetValueData<bool>("MonColBtnEffActive"));
        }
    }
}




#region 주석 
/*public Image setting;
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
}*/
#endregion

