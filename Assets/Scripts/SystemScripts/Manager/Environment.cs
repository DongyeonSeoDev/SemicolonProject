using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Environment : MonoSingleton<Environment>
{
    public Light2D mainLight;

    private void Awake()
    {
        if (!mainLight) mainLight = FindObjectOfType<Light2D>();

        DefineEvent();
    }

    private void DefineEvent()
    {
        TimeManager.timePauseAction += ()=> mainLight.DOIntensity(0.7f, 0.5f, true);
        TimeManager.timeResumeAction += () => mainLight.DOIntensity(1f, 0.5f, true);
    }
}
