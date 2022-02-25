using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoSingleton<SkillUIManager>
{
    private SkillInfoImage[] skillInfoUIArr;

    [SerializeField] private Transform skillImgUIParent;

    public SkillInfoImage defaultSkill;
    public SkillInfoImage specialSkill;
    public SkillInfoImage drain;

    private void Awake()
    {
        skillInfoUIArr = skillImgUIParent.GetComponentsInChildren<SkillInfoImage>();
    }

    public void UpdateSkillKeyCode()
    {
        for(int i=0; i<skillInfoUIArr.Length; i++)
        {
            skillInfoUIArr[i].UpdateKeyCode();
        }
    }
}