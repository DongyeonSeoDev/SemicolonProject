using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    private List<SoundBox> soundBoxes = new List<SoundBox>();
    public List<SoundBox> SoundBoxes
    {
        get { return soundBoxes; }
    }

    private Dictionary<string, SoundBox> soundBoxesDict = new Dictionary<string, SoundBox>();
    private Dictionary<string, Queue<SoundBox>> soundBoxesDictForPooling = new Dictionary<string, Queue<SoundBox>>();

    [SerializeField]
    private float volume = 1f;
    private bool pause = false;

    void Start()
    {
        soundBoxes.ForEach(x => soundBoxesDict.Add(x.SoundBoxId, x));
    }
    public void PlaySoundBox(SoundBox soundBox)
    {
        string soundBoxId = soundBox.SoundBoxId;

        PlaySoundBox(soundBoxId);
    }
    public void PlaySoundBox(string soundBoxId)
    {
        SoundBox soundBox = null;

        if (soundBoxesDict.ContainsKey(soundBoxId))
        {
            if(soundBoxesDictForPooling.ContainsKey(soundBoxId))
            {
                if(soundBoxesDictForPooling[soundBoxId].Count > 0)
                {
                    soundBox = soundBoxesDictForPooling[soundBoxId].Dequeue();
                    soundBox.gameObject.SetActive(true);

                    soundBox.SetVolume(volume);
                    soundBox.SetPause(pause);

                    return;
                }
            }
            else
            {
                Queue<SoundBox> queue = new Queue<SoundBox>();
                queue.Enqueue(soundBoxesDict[soundBoxId]);

                soundBoxesDictForPooling.Add(soundBoxId, queue);
            }

            soundBox = Instantiate(soundBoxesDict[soundBoxId].gameObject, transform).GetComponent<SoundBox>();

            soundBox.SetVolume(volume);
            soundBox.SetPause(pause);
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
}
