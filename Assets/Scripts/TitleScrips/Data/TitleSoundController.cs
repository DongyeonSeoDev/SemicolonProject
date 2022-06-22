using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TitleSoundController : MonoBehaviour
{
    [SerializeField]
    private Slider masterSoundSlider = null;
    [SerializeField]
    private Slider BGMSlider = null;
    [SerializeField]
    private Slider SFXSlider = null;

    public AudioMixer masterAudioMixer = null;

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        masterSoundSlider.value = SaveFileStream.SaveOptionData.masterSound;
        BGMSlider.value = SaveFileStream.SaveOptionData.bgmSize;
        SFXSlider.value = SaveFileStream.SaveOptionData.soundEffectSize;

        masterAudioMixer.SetFloat("Master", SaveFileStream.SaveOptionData.masterSound != -40f ? SaveFileStream.SaveOptionData.masterSound : -80f);
    }

    public void OnChangedMasterVolume()  // min : -40 , max : 0
    {
        float volume = masterSoundSlider.value;
        SaveFileStream.SaveOptionData.masterSound = volume;
        masterAudioMixer.SetFloat("Master", volume != -40f ? volume : -80f);
    }

    public void OnChangedBGMVolume()  // min : -40 , max : 0
    {
        float volume = BGMSlider.value;
        SaveFileStream.SaveOptionData.bgmSize = volume;
        SoundManager.Instance.SetBGMVolume(volume);
    }

    public void OnChangedSFXVolume()  // min : -40 , max : 0
    {
        float volume = SFXSlider.value;
        SaveFileStream.SaveOptionData.soundEffectSize = volume;
        SoundManager.Instance.SetEffectSoundVolume(volume);
    }
}
