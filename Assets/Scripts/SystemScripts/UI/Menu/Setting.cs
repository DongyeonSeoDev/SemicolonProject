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




#region �ּ� 
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

public void SetChildImgs(bool isDissolve)  //material ����� ���¿����� Mask���� �ۿ����� ����. (Rect Mask 2D���� �Ⱥ������� �ٷ� ������� �����)
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

