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
    /// ���� �ε� ���¸� ��Ÿ���ִ� Bar����� �׷� ǥ�� ���� �׳� LoadScene ����
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
        EventManager.TriggerEvent("StartStageScene");  //���� ���� ���� �̺�Ʈ ȣ��
        EventManager.ClearEvents();  //�� �Ѿ�� ���� �̺�Ʈ �ʱ�ȭ

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
