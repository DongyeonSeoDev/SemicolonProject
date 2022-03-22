using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundBox : MonoBehaviour
{
    [SerializeField]
    private string soundBoxId = "";
    public string SoundBoxId
    {
        get { return soundBoxId; }
        set { soundBoxId = value; }
    }

    [SerializeField]
    private AudioSource audioSource = null;
    private AudioSource AudioSource
    {
        get
        {
            if(audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            return audioSource;
        }
    }

    [Header("이 값이 true면 아래의 playTime관련 값들은 무시된다.")]
    [SerializeField]
    private bool isBackgroundMusic = false;

    [Header("이 값이 true면 아래의 playTime값은 무시하고, 끝까지 재생한다.")]
    [SerializeField]
    private bool playToEnd = false;

    [Header("이 사운드를 몇 초 동안 플레이될지에 관한 값")]
    [SerializeField]
    private float playTime = 1f;

    private float playTimer = 0f;

    [SerializeField]
    private float defaultVolume = 1f;
    private float volume = 1f;
    public float Volume
    {
        get { return volume; }
    }

    [SerializeField]
    private float defaultPitch = 1f;
    private float pitch = 1f;
    public float Pitch
    {
        get { return pitch; }
    }

    private bool isPause = false;
    public bool IsPause
    {
        get { return isPause; }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        EventManager.StartListening("StopSoundAll", (Action)SoundStop);
        EventManager.StartListening("SoundPauseAll", (Action<bool>)SetPause);
        EventManager.StartListening("SetVolumeAll", (Action<float>)SetVolume);
        EventManager.StartListening("SetPitchAll", (Action<float>)SetPitch);

        EventManager.StartListening("StopSound", (Action<string>)SoundStop);
        EventManager.StartListening("PuaseSound", (Action<string, bool>)SetPause);
        EventManager.StartListening("SetVolume", (Action<string, float>)SetVolume);
        EventManager.StartListening("SetPitch", (Action<string, float>)SetPitch);

        OnPlay();
    }
    private void OnPlay()
    {
        volume = defaultVolume;
        AudioSource.volume = volume;

        pitch = defaultPitch;
        AudioSource.pitch = pitch;

        if (AudioSource.clip != null)
        {
            AudioSource.Play();

            if (!isBackgroundMusic)
            {
                if(playToEnd)
                {
                    playTime = AudioSource.clip.length;
                }

                playTimer = playTime;
            }
            else
            {
                playTime = float.PositiveInfinity;
                AudioSource.loop = true;
            }
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("SoundStopAll", (Action)SoundStop);
        EventManager.StopListening("SoundPauseAll", (Action<bool>)SetPause);
        EventManager.StopListening("SetVolumeAll", (Action<float>)SetVolume);
        EventManager.StopListening("SetPitchAll", (Action<float>)SetPitch);

        EventManager.StopListening("SoundStop", (Action<string>)SoundStop);
        EventManager.StopListening("SoundPause", (Action<string, bool>)SetPause);
        EventManager.StopListening("SetVolume", (Action<string, float>)SetVolume);
        EventManager.StopListening("SetPitch", (Action<string, float>)SetPitch);
    }
    private void Update()
    {
        if(!(isBackgroundMusic || isPause) && playTimer > 0f)
        {
            playTimer -= Time.deltaTime;

            if(playTimer <= 0f)
            {
                SoundStop();
            }
        }
    }
    public void SoundStop()
    {
        SoundManager.Instance.DespawnSoundBox(this);
    }
    public void SoundStop(string boxId)
    {
        if(boxId != soundBoxId)
        {
            return;
        }

        SoundManager.Instance.DespawnSoundBox(this);
    }
    public void SetPause(bool pause)
    {
        isPause = pause;

        if (pause)
        {
            AudioSource.Pause();

            return;
        }

        AudioSource.UnPause();
    }
    public void SetPause(string boxId, bool pause)
    {
        if(boxId != soundBoxId)
        {
            return;
        }

        isPause = pause;

        if (pause)
        {
            AudioSource.Pause();

            return;
        }

        AudioSource.UnPause();
    }
    public void SetVolume(float v)
    {
        volume = v * defaultVolume;
        AudioSource.volume = volume;
    }
    public void SetVolume(string boxId, float v)
    {
        if(boxId != soundBoxId)
        {
            return ;
        }

        volume = v * defaultVolume;
        AudioSource.volume = volume;
    }

    public void SetPitch(float p)
    {
        pitch = p * defaultPitch;
        AudioSource.pitch = pitch;
    }
    public void SetPitch(string boxId, float p)
    {
        if(boxId != soundBoxId)
        {
            return;
        }

        pitch = p * defaultPitch;
        AudioSource.pitch = pitch;
    }
}
