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

    #region Volume, Pitch, Pause관련 변수들
   

    [Range(0f, 1f)]
    [SerializeField]
    private float bgmVolume = 1f;
    public float BGMVolume
    {
        get { return bgmVolume; }
        set { bgmVolume = value; }
    }

    [Range(0f, 1f)]
    [SerializeField]
    private float effectSoundVolume = 1f;
    public float EffectSoundVolume
    {
        get { return effectSoundVolume; }
        set { effectSoundVolume = value; }
    }

    private float bgmPitch = 1f;
    public float BgmPitch
    {
        get { return bgmPitch; }
    }

    private float effectPitch = 1f;
    public float EffectPitch
    {
        get { return effectPitch; }
    }

    private bool bgmPause = false;
    public bool BgmPause
    {
        get { return bgmPause; }
    }

    private bool effectSoundsPause = false;
    public bool EffectSoundsPause
    {
        get { return effectSoundsPause; }
    }
    #endregion

    #region 타이머 관련 변수들
    private float BGMVolumeTimer = 0f;
    private float BGMVolumeTime = 0f;
    private float BGMVolumeStart = 0f;
    private float BGMVolumeTarget = 0f;

    private float BGMPitchTimer = 0f;
    private float BGMPitchTime= 0f;
    private float BGMPitchStart = 0f;
    private float BGMPitchTarget = 0f;
    #endregion

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
    //private void Start()
    //{
    //    //SetBGMPitchByLerp(1f, -0.3f, 1f);
    //}
    private void Update()
    {
        LerpBGMVolume();
        LerpBGMPitch();
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

                    soundBox.SetPause(soundBoxId, soundBox.IsBackgroundMusic ? bgmPause : effectSoundsPause);
                    soundBox.SetVolume(soundBoxId, soundBox.IsBackgroundMusic ? bgmVolume : effectSoundVolume);
                    soundBox.SetPitch(soundBoxId, soundBox.IsBackgroundMusic ? bgmPitch : effectPitch);

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

            soundBox.SetPause(soundBoxId, soundBox.IsBackgroundMusic ? bgmPause : effectSoundsPause);
            soundBox.SetVolume(soundBoxId, soundBox.IsBackgroundMusic ? bgmVolume : effectSoundVolume);
            soundBox.SetPitch(soundBoxId, soundBox.IsBackgroundMusic ? bgmPitch : effectPitch);
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

    public void StopBGM()
    {
        EventManager.TriggerEvent("StopBGMAll");
    }
    public void StopEffectSounds()
    {
        EventManager.TriggerEvent("StopEffectSoundAll");
    }

    public void SetBGMVolume(float v, bool isSetLerp = false)
    {
        bgmVolume = v;
        if (!isSetLerp)
        {
            BGMVolumeTimer = BGMVolumeTime;
        }
        EventManager.TriggerEvent("SetBGMVolumeAll", bgmVolume);
    }
    public void SetEffectSoundVolume(float v)
    {
        effectSoundVolume = v;
        EventManager.TriggerEvent("SetEffectSoundVolumeAll", effectSoundVolume);
    }

    public void SetBGMVolumeByLerp(float start, float target, float time)
    {
        BGMVolumeStart = start;
        BGMVolumeTarget = target;
        BGMVolumeTime = time;
        BGMVolumeTimer = 0f;
    }
    private void LerpBGMVolume()
    {
        if (BGMVolumeTimer < BGMVolumeTime)
        {
            BGMVolumeTimer += Time.deltaTime;

            bgmVolume = Mathf.Lerp(BGMVolumeStart, BGMPitchTime, BGMVolumeTimer / BGMVolumeTime);
            SetBGMVolume(bgmVolume, true);
        }
    }
    public void SetBGMPitchByLerp(float start, float target, float time)
    {
        BGMPitchStart = start;
        BGMPitchTarget = target;
        BGMPitchTime = time;
        BGMPitchTimer = 0f;
    }
    private void LerpBGMPitch()
    {
        if(BGMPitchTimer < BGMPitchTime)
        {
            BGMPitchTimer += Time.deltaTime;

            bgmPitch = Mathf.Lerp(BGMPitchStart, BGMPitchTarget, BGMPitchTimer / BGMPitchTime);
            SetBGMPitch(bgmPitch, true);
        }
    }
    
    public void PauseBGM(bool p)
    {
        bgmPause = p;
        EventManager.TriggerEvent("BGMPauseAll", p);
    }
    public void PauseEffectSounds(bool p)
    {
        effectSoundsPause = p;
        EventManager.TriggerEvent("EffectSoundPauseAll", p);
    }

    public void SetBGMPitch(float p, bool isSetLerp = false)
    {
        bgmPitch = p;
        if (!isSetLerp)
        {
            BGMPitchTimer = BGMPitchTime;
        }
        EventManager.TriggerEvent("SetBGMPitchAll", p);
    }
    public void SetEffectSoundsPitch(float p)
    {
        effectPitch = p;
        EventManager.TriggerEvent("SetEffectSoundsPitchAll", p);
    }
}
