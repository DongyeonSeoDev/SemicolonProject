using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SingletonClass<ObjectManager> 
{
    public Dictionary<string, InteractionObj> itrObjDic = new Dictionary<string, InteractionObj>();
}
