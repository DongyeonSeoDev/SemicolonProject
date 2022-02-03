using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionOption : MonoBehaviour
{
    private List<Resolution> resolutions = new List<Resolution>();
    private int resolutionNum;

    private FullScreenMode screenMode = FullScreenMode.FullScreenWindow;

    public Dropdown resolutionDd;
    public Toggle fullScrTg;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        //resolutions.AddRange(Screen.resolutions);
        for(int i=0; i<Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width < 1280 || Screen.resolutions[i].height < 720) continue;

            resolutions.Add(Screen.resolutions[i]);
        }
        resolutionDd.options.Clear();

        int optionNum = 0, w = Screen.width, h = Screen.height;
        foreach(Resolution rs in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = rs.width + " x " + rs.height;
            resolutionDd.options.Add(option);

            if (rs.width == w && rs.height == h)
            {
                resolutionDd.value = optionNum;
                resolutionNum = optionNum;
            }
            optionNum++;
        }
        resolutionDd.RefreshShownValue();
        fullScrTg.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow);
    }

    public void ResolutionDdChanged(int num)
    {
        resolutionNum = num;
    }

    public void FullScreenTgChanged(bool full)
    {
        screenMode = full ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OnClickOKBtn()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    }
}
