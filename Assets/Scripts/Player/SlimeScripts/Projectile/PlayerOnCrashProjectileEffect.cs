using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnCrashProjectileEffect : MonoBehaviour
{
    public void OnSpawn(Quaternion quater, Vector2 pos)
    {
        transform.position = pos;
        transform.rotation = quater;
    }
    public void Despawn()
    {
        SlimePoolManager.Instance.AddObject(gameObject);
        gameObject.SetActive(false);
    }
}
