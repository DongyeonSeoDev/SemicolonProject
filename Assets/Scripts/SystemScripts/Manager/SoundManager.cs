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
    private float volume = 1f;
    private float pitch = 1f;
    private bool pause = false;

    private void Awake()
    {
        soundBoxes = Resources.LoadAll<SoundBox>(soundPrefabsPath).ToList();
        soundBoxes.ForEach(x => {
            if (soundBoxesDict.ContainsKey(x.SoundBoxId))
            {
                Debug.LogError("The SoundBoxId '" + x.SoundBoxId + "' is already used. Change SoundBoxId of " + x.name);
            }
            else
            {
                soundBoxesDict.Add(x.SoundBoxId, x);
            }
        });
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

                    soundBox.SetPause(pause);
                    soundBox.SetVolume(volume);
                    soundBox.SetPitch(pitch);

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

            soundBox.SetPause(pause);
            soundBox.SetVolume(volume);
            soundBox.SetPitch(pitch);
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
    public void ChangeVolume(float v)
    {
        volume = v;
        EventManager.TriggerEvent("SetVolume", volume);
    }
    public void PauseSounds(bool p)
    {
        pause = p;
        EventManager.TriggerEvent("SoundPause", p);
    }
    public void SetPitch(float p)
    {
        pitch = p;
        EventManager.TriggerEvent("SetPitch", p);
    }
}
