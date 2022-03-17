using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionOption : MonoBehaviour
{
    //private List<Resolution> resolutions = new List<Resolution>();
    private int resolutionNum;  //해상도 몇 번째 선택했는지
    private int prevResolNum; //해상도 몇 번째로 변경할지 선택하고 확인까지 눌러야 이 값도 바뀜

    private FullScreenMode screenMode = FullScreenMode.FullScreenWindow;  //전체모드인지 창모드인지
    private FullScreenMode prevScrMode;  //전체모드할지, 창모드할지 택하고 확인까지 눌러야 이 값도 바뀜

    [SerializeField] private List<Pair<int, int>> whResolutionList = new List<Pair<int, int>>();  //지원하는 해상도 너비 x 높이 
    public Dropdown resolutionDd;
    public Toggle fullScrTg;

    public (int, int) MaxScrWH => (whResolutionList[whResolutionList.Count-1].first, whResolutionList[whResolutionList.Count - 1].second);

    private void Start()
    {
        Init();
        StartCoroutine(CheckOverResolutionCo());
    }

     /*private void InsertResolution()
    {
        resolutions.AddRange(Screen.resolutions);
        resolutions.Clear();
        List<string> rsList = new List<string>();
        for (int i = 0; i < Screen.resolutions.Length; i++)  //해상도들을 가져와서 리스트에 넣음
        {
            //if (Screen.resolutions[i].width < 1280 || Screen.resolutions[i].height < 720) continue;

            string s = string.Concat(Screen.resolutions[i].width, ',', Screen.resolutions[i].height);
            if (rsList.Contains(s)) continue;  //중복되는 해상도는 빼줌

            resolutions.Add(Screen.resolutions[i]);
            rsList.Add(s);
        }
    }*/

    private void Init()
    {
        //InsertResolution();

        resolutionDd.options.Clear();

        /*if (!resolutions.Exists(x => x.width == 1920))  //혹시 해상도 설정에서 너비가 1920인게 안나온다면 초기화해서 다시 나오게
        {
            Screen.SetResolution(1920, 1080, false);
            InsertResolution();
        }*/

        //현재 내 상태에 맞게 UI세팅하고 드롭다운 업데이트
        int optionNum = 0, w = Screen.width, h = Screen.height;
        for(int i = 0; i < whResolutionList.Count; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = whResolutionList[i].first + " x " + whResolutionList[i].second;
            resolutionDd.options.Add(option);

            if (whResolutionList[i].first == w && whResolutionList[i].second == h)
            {
                resolutionDd.value = optionNum;
                resolutionNum = optionNum;
                prevResolNum = resolutionNum;
            }
            optionNum++;
        }
        /*foreach (Resolution rs in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = rs.width + " x " + rs.height;
            resolutionDd.options.Add(option);

            if (rs.width == w && rs.height == h)
            {
                resolutionDd.value = optionNum;
                resolutionNum = optionNum;
                prevResolNum = resolutionNum;
            }
            optionNum++;
        }*/

        resolutionDd.RefreshShownValue();

        fullScrTg.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow);

        screenMode = Screen.fullScreenMode;
        prevScrMode = screenMode;

        UIManager.Instance.OnChangedResolution();
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
        Screen.SetResolution(whResolutionList[resolutionNum].first, whResolutionList[resolutionNum].second, screenMode);
        EventManager.TriggerEvent("ChangeResolution");
        prevResolNum = resolutionNum;
        prevScrMode = screenMode;
    }

    public void ExitResolutionPanel()  //해상도나 풀스크린 모드 다른 값으로 설정하고 확인 안누른채 해상도창 나가면 이전으로 UI리셋
    {
        if(prevResolNum != resolutionNum)
        {
            resolutionNum = prevResolNum;
            resolutionDd.value = resolutionNum;
        }

        if(prevScrMode != screenMode)
        {
            screenMode = prevScrMode;
            fullScrTg.isOn = screenMode.Equals(FullScreenMode.FullScreenWindow);
        }
    }

    public void OnResolutionUIUpdate()
    {
        fullScrTg.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow);
        screenMode = Screen.fullScreenMode;
        prevScrMode = screenMode;

        resolutionDd.value = whResolutionList.Count - 1;
        resolutionNum = resolutionDd.value;
        prevResolNum = resolutionNum;
    }

    private IEnumerator CheckOverResolutionCo() //1초마다 지원하는 해상도를 넘었는지 체크하고 넘었다고 지원하는 해상도중 최대사이즈로 바꿔줌
    {
        EventManager.StartListening("ChangeResolution", OnResolutionUIUpdate);
        WaitForSecondsRealtime wsr = new WaitForSecondsRealtime(1);

        while(true)
        {
            if (Screen.width > MaxScrWH.Item1 || Screen.height > MaxScrWH.Item2)
            {
                Screen.SetResolution(MaxScrWH.Item1, MaxScrWH.Item2, Screen.fullScreenMode);
                EventManager.TriggerEvent("ChangeResolution");
            }
            yield return wsr;
        }
    }
}
