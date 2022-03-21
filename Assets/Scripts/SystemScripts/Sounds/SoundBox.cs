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

    [Header("�� ���� true�� �Ʒ��� playTime���� ���õȴ�.")]
    [SerializeField]
    private bool isBackgroundMusic = true;

    [Header("�� ���带 �� �� ���� �÷��̵����� ���� ��")]
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

    private void OnEnable()
    {
        EventManager.StartListening("SoundPause", (Action<bool>)SetPause);
        EventManager.StartListening("SetVolume", (Action <float>)SetVolume);
        EventManager.StartListening("SetPitch", (Action<float>)SetPitch);

        if (audioSource.clip != null)
        {
            audioSource.Play();

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
        audioSource.volume = volume;
    }

    public void SetPause(bool pause)
    {
        isPause = pause;

        if (pause)
        {
            audioSource.Pause();

            return;
        }

        audioSource.UnPause();
    }

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }
}
