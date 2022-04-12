using System.Collections.Generic;

public class UIActiveData : SingletonClass<UIActiveData>
{
    public Dictionary<UIType, bool> uiActiveDic = new Dictionary<UIType, bool>();

    public void Save()
    {
        foreach (UIType key in uiActiveDic.Keys)
        {
            GameManager.Instance.savedData.userInfo.uiActiveDic[key] = uiActiveDic[key];
        }
    }

    public void Init()
    {
        for (int i = 0; i < Global.EnumCount<UIType>(); i++)
        {
            uiActiveDic[(UIType)i] = true;
        }

        //따로 처리해야할 것들
        uiActiveDic[UIType.QUIT] = false;
    }
}
