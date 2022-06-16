using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    private string nextScene = "";

    public Image progressBar = null;

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
    /// <summary>
    /// 따로 로딩 상태를 나타내주는 Bar라던가 그런 표시 없이 그냥 LoadScene 진행
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator LoadScenePregress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while(!op.isDone)
        {
            if (op.progress < 0.9f)
            {
                if (progressBar != null)
                {
                    progressBar.fillAmount = op.progress;
                }
            }
            else
            {
                timer += Time.unscaledDeltaTime;

                if (progressBar != null)
                {
                    progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                    if (progressBar.fillAmount >= 1f)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
                else
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
