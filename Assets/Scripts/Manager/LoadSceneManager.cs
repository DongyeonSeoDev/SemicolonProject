using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    private string nextScene = "";

    public Image progressBar = null;
    public TextMeshProUGUI progressText = null;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 로딩 상태를 나타내주는 Bar를 통해 로딩 현황을 표시하면서 LoadScene 진행
    /// </summary>
    /// <param name="progressBar"></param>
    /// <param name="sceneName"></param>
    public void LoadScene(Image progressBar, string sceneName)
    {
        this.progressBar = progressBar;

        nextScene = sceneName;

        StartCoroutine(LoadScenePregress());
    }
    public void LoadScene(Image progressBar, TextMeshProUGUI progressText, string sceneName)
    {
        this.progressBar = progressBar;
        this.progressText = progressText;

        nextScene = sceneName;

        StartCoroutine(LoadScenePregress());
    }
    public void LoadScene(TextMeshProUGUI progressText, string sceneName)
    {
        this.progressText = progressText;

        nextScene = sceneName;

        StartCoroutine(LoadScenePregress());
    }
    /// <summary>
    /// 따로 로딩 상태를 나타내주는 Bar라던가 그런 표시 없이 그냥 LoadScene 진행
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        EventManager.TriggerEvent("StartStageScene");  
        EventManager.ClearEvents();  

        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator LoadScenePregress()
    {
        EventManager.TriggerEvent("StartStageScene");  //게임 시작 전에 이벤트 호출
        EventManager.ClearEvents();  //씬 넘어가기 전에 이벤트 초기화

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        progressBar.fillAmount = 0;

        if (progressText != null)
        {
            progressText.text = string.Format("{0:0.00}%", progressBar.fillAmount * 100f);
        }

        while (!op.isDone)
        {
            if (op.progress < 0.9f)
            {
                if (progressBar != null)
                {
                    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, Time.unscaledDeltaTime);
                }
            }
            else
            {
                timer += Time.unscaledDeltaTime;

                if (progressBar != null)
                {
                    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                }
                else
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }

            if(progressText != null)
            {
                progressText.text = string.Format("{0:0.00}%", progressBar.fillAmount * 100f);
            }

            if (progressBar.fillAmount >= 1f)
            {
                op.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
    }
}
