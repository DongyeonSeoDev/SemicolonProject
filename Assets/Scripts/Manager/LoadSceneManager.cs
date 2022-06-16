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
    /// �ε� ���¸� ��Ÿ���ִ� Bar�� ���� �ε� ��Ȳ�� ǥ���ϸ鼭 LoadScene ����
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
    /// ���� �ε� ���¸� ��Ÿ���ִ� Bar����� �׷� ǥ�� ���� �׳� LoadScene ����
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
