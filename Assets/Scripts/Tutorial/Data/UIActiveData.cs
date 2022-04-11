using System.Collections.Generic;

public class UIActiveData : SingletonClass<UIActiveData>
{
    public Dictionary<UIType, bool> uiActiveDic = new Dictionary<UIType, bool>();
}
