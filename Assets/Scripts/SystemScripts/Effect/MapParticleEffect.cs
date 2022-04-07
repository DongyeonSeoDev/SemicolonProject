using UnityEngine;
using System.Collections;

public class MapParticleEffect : MonoBehaviour
{
    public ParticleSystem mapEff;

    public bool playOnAwake;
    public float duration = 1f;

    public bool isRandomInterval;
    public float interval;
    public Pair<float, float> randomInterval;

    private WaitForSeconds durationWs;
    private WaitForSeconds ws;

    private void Awake()
    {
        mapEff.Stop();
        durationWs = new WaitForSeconds(duration);
        if (!isRandomInterval) ws = new WaitForSeconds(interval);
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(StartEffectCo());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public IEnumerator StartEffectCo()
    {
        if(playOnAwake)
        {
            mapEff.Play();
            yield return durationWs;
            mapEff.Stop();
        }

        while(true)
        {
            if(!isRandomInterval)
            {
                yield return ws;
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(randomInterval.first, randomInterval.second));
            }
            mapEff.Play();
            yield return durationWs;
            mapEff.Stop();
        }
    }
}
