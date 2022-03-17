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

    [SerializeField]
    private float playTime = 1f;
    private float playTimer = 0f;

    private float volume = 1f;
    private bool isPause = false;

    private void OnEnable()
    {
        EventManager.StartListening("SoundPause", (Action<bool>)SetPause);
        EventManager.StartListening("SetVolume", (Action <float>)SetVolume);

        if (audioSource.clip != null)
        {
            audioSource.Play();
            playTimer = playTime;
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("SoundPause", (Action<bool>)SetPause);
        EventManager.StopListening("SetVolume", (Action<float>)SetVolume);
    }
    private void Update()
    {
        if(playTimer > 0f && !isPause)
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
}
