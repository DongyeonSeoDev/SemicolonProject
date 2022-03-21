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

    [Header("이 값이 true면 아래의 playTime값은 무시된다.")]
    [SerializeField]
    private bool isBackgroundMusic = true;

    [Header("이 사운드를 몇 초 동안 플레이될지에 관한 값")]
    [SerializeField]
    private float playTime = 1f;

    private float playTimer = 0f;


    private float volume = 1f;
    public float Volume
    {
        get { return volume; }
    }

    private bool isPause = false;
    public bool IsPause
    {
        get { return isPause; }
    }

    private float pitch = 1f;
    public float Pitch
    {
        get { return pitch; }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        EventManager.StartListening("SoundPause", (Action<bool>)SetPause);
        EventManager.StartListening("SetVolume", (Action <float>)SetVolume);
        EventManager.StartListening("SetPitch", (Action<float>)SetPitch);

        if (AudioSource.clip != null)
        {
            AudioSource.Play();

            if (!isBackgroundMusic)
            {
                playTimer = playTime;
            }
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("SoundPause", (Action<bool>)SetPause);
        EventManager.StopListening("SetVolume", (Action<float>)SetVolume);
        EventManager.StopListening("SetPitch", (Action<float>)SetPitch);
    }
    private void Update()
    {
        if(!(isBackgroundMusic || isPause) && playTimer > 0f)
        {
            playTimer -= Time.deltaTime;

            if(playTimer <= 0f)
            {
                SoundManager.Instance.DespawnSoundBox(this);
            }
        }
    }

    public void SetVolume(float v)
    {
        volume = v;
        AudioSource.volume = volume;
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

    public void SetPitch(float pitch)
    {
        AudioSource.pitch = pitch;
    }
}
