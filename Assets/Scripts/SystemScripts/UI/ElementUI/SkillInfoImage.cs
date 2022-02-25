using UnityEngine.UI;
using UnityEngine;
using System;

public class SkillInfoImage : MonoBehaviour
{
    public Triple<Image, Text, Image> skillImgCoolTxtImgTriple;
    public Text keyCodeTxt;
    public NameInfoFollowingCursor nifc;
    [SerializeField] Button skillBtn;
    [SerializeField] CanvasGroup cvsg;

    [SerializeField] private SkillType skillType;

    public void Register()
    {

    }

    public void Unregister()
    {

    }

    public void UpdateKeyCode()
    {
        keyCodeTxt.text = KeyCodeToString.GetString(KeySetting.GetKeyCode((KeyAction)Enum.Parse(typeof(KeyAction), skillType.ToString())));
    }
}
