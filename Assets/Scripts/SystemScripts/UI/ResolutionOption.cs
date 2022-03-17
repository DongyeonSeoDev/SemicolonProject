using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionOption : MonoBehaviour
{
    //private List<Resolution> resolutions = new List<Resolution>();
    private int resolutionNum;  //�ػ� �� ��° �����ߴ���
    private int prevResolNum; //�ػ� �� ��°�� �������� �����ϰ� Ȯ�α��� ������ �� ���� �ٲ�

    private FullScreenMode screenMode = FullScreenMode.FullScreenWindow;  //��ü������� â�������
    private FullScreenMode prevScrMode;  //��ü�������, â������� ���ϰ� Ȯ�α��� ������ �� ���� �ٲ�

    [SerializeField] private List<Pair<int, int>> whResolutionList = new List<Pair<int, int>>();  //�����ϴ� �ػ� �ʺ� x ���� 
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
        for (int i = 0; i < Screen.resolutions.Length; i++)  //�ػ󵵵��� �����ͼ� ����Ʈ�� ����
        {
            //if (Screen.resolutions[i].width < 1280 || Screen.resolutions[i].height < 720) continue;

            string s = string.Concat(Screen.resolutions[i].width, ',', Screen.resolutions[i].height);
            if (rsList.Contains(s)) continue;  //�ߺ��Ǵ� �ػ󵵴� ����

            resolutions.Add(Screen.resolutions[i]);
            rsList.Add(s);
        }
    }*/

    private void Init()
    {
        //InsertResolution();

        resolutionDd.options.Clear();

        /*if (!resolutions.Exists(x => x.width == 1920))  //Ȥ�� �ػ� �������� �ʺ� 1920�ΰ� �ȳ��´ٸ� �ʱ�ȭ�ؼ� �ٽ� ������
        {
            Screen.SetResolution(1920, 1080, false);
            InsertResolution();
        }*/

        //���� �� ���¿� �°� UI�����ϰ� ��Ӵٿ� ������Ʈ
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

    public void ExitResolutionPanel()  //�ػ󵵳� Ǯ��ũ�� ��� �ٸ� ������ �����ϰ� Ȯ�� �ȴ���ä �ػ�â ������ �������� UI����
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

    private IEnumerator CheckOverResolutionCo() //1�ʸ��� �����ϴ� �ػ󵵸� �Ѿ����� üũ�ϰ� �Ѿ��ٰ� �����ϴ� �ػ��� �ִ������� �ٲ���
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
