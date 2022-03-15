using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Image setting;
    public Image[] childImgs;

    public void InitSet()
    {
        setting.GetComponent<DissolveCtrl>().SetFade(1);
        for(int i = 0; i < childImgs.Length; i++)
        {
            childImgs[i].material = setting.material;
        }
    }
}
