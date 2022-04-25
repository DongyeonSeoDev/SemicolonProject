using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneManager : MonoSingleton<CutsceneManager>
{
    private Dictionary<string, TimelineAsset> timeLineDict = new Dictionary<string, TimelineAsset>();
    public TimelineAsset[] timeLineAssets;
    public PlayableDirector pd;

    private void Awake()
    {
        for (int i = 0; i < timeLineAssets.Length; i++)
        {
            timeLineDict.Add(timeLineAssets[i].name, timeLineAssets[i]);
        }
    }

    public void PlayCutscene(string key)
    {
        if (!timeLineDict.ContainsKey(key))
        {
            Debug.LogWarning("Not Exist Key : " + key);
            return;
        }

        pd.playableAsset = timeLineDict[key];
        EventManager.TriggerEvent("StartCutScene");
        pd.Play();
    }

    public void StopCutscene()
    {
        EventManager.TriggerEvent("EndCutScene");
        pd.Stop();
    }

    public void PauseCutscene()
    {
        pd.Pause();
    }

    public void ResumeCutscene()
    {
        pd.Resume();
    }

}
