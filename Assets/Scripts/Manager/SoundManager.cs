using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private List<SoundBox> soundBoxes = new List<SoundBox>();
    private Dictionary<string, SoundBox> soundBoxesDict = new Dictionary<string, SoundBox>();
    private Dictionary<string, Queue<SoundBox>> soundBoxesDictForPooling = new Dictionary<string, Queue<SoundBox>>();

    private readonly string soundPrefabsPath = "Prefabs/SoundPrefabs";

    [SerializeField]
    private string currentBGMSoundBoxId = "NULL";

    [SerializeField]
    private float volume = 1f;
    private float pitch = 1f;
    private bool pause = false;

    private void Awake()
    {
        soundBoxes = Resources.LoadAll<SoundBox>(soundPrefabsPath).ToList();

        soundBoxes.ForEach(x => {
            if (soundBoxesDict.ContainsKey(x.SoundBoxId))
            {
                Debug.LogError("The SoundBoxId '" + x.SoundBoxId + "' is already used. Change SoundBoxId of '" + x.name + "'");
            }
            else
            {
                soundBoxesDict.Add(x.SoundBoxId, x);
            }
        });
    }
    private void OnEnable()
    {
        EventManager.StartListening("StartBGM", ChangeBGMSoundBox);
    }
    private void OnDisable()
    {
        EventManager.StopListening("StartBGM", ChangeBGMSoundBox);
    }
    /// <summary>
    /// 스테이지 BGM을 바꿈.
    /// </summary>
    /// <param SoundBox="soundBox"></param>
    public void ChangeBGMSoundBox(SoundBox soundBox)
    {
        string soundBoxId = soundBox.SoundBoxId;

        ChangeBGMSoundBox(soundBoxId);
    }
    /// <summary>
    /// 스테이지 BGM을 바꿈.
    /// </summary>
    /// <param id="soundBoxId"></param>
    public void ChangeBGMSoundBox(string soundBoxId)
    {
        Debug.Log(soundBoxId);

        if (soundBoxesDict.ContainsKey(soundBoxId))
        {
            EventManager.TriggerEvent("StopSound", currentBGMSoundBoxId);

            PlaySoundBox(soundBoxId);

            currentBGMSoundBoxId = soundBoxId;
        }
        else
        {
            Debug.LogWarning("The SoundBoxId '" + soundBoxId + "' is not Contain.");
        }
    }
    public void PlaySoundBox(SoundBox soundBox)
    {
        string soundBoxId = soundBox.SoundBoxId;

        PlaySoundBox(soundBoxId);
    }
    public void PlaySoundBox(string soundBoxId)
    {
        GameObject soundBoxObj = null;
        SoundBox soundBox = null;

        if (soundBoxesDict.ContainsKey(soundBoxId))
        {
            if(soundBoxesDictForPooling.ContainsKey(soundBoxId))
            {
                if(soundBoxesDictForPooling[soundBoxId].Count > 0)
                {
                    soundBox = soundBoxesDictForPooling[soundBoxId].Dequeue();
                    soundBox.gameObject.SetActive(true);

                    soundBox.SetPause(soundBoxId, pause);
                    soundBox.SetVolume(soundBoxId, volume);
                    soundBox.SetPitch(soundBoxId, pitch);

                    return;
                }
            }
            else
            {
                Queue<SoundBox> queue = new Queue<SoundBox>();

                soundBoxesDictForPooling.Add(soundBoxId, queue);
            }

            soundBoxObj = Instantiate(soundBoxesDict[soundBoxId].gameObject, transform);
            soundBox = soundBoxObj.GetComponent<SoundBox>();

            soundBox.SetPause(soundBoxId, pause);
            soundBox.SetVolume(soundBoxId, volume);
            soundBox.SetPitch(soundBoxId, pitch);
        }
        else
        {
            Debug.LogWarning(soundBoxId + " is not Contain.");
        }
    }
    public void DespawnSoundBox(SoundBox soundBox)
    {
        soundBoxesDictForPooling[soundBox.SoundBoxId].Enqueue(soundBox);
        soundBox.gameObject.SetActive(false);
    }
    public void StopSounds()
    {
        EventManager.TriggerEvent("StopSoundAll");
    }
    public void ChangeVolume(float v)
    {
        volume = v;
        EventManager.TriggerEvent("SetVolumeAll", volume);
    }
    public void PauseSounds(bool p)
    {
        pause = p;
        EventManager.TriggerEvent("SoundPauseAll", p);
    }
    public void SetPitch(float p)
    {
        pitch = p;
        EventManager.TriggerEvent("SetPitchAll", p);
    }
}
