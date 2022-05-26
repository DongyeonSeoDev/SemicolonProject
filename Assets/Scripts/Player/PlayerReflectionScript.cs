using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerReflectionProjectileData
{
    public GameObject projectile;
    public float projectileSpeed;
    public float shootPosOffset;
}
public class PlayerReflectionScript : MonoBehaviour
{
    public void DoReflection(GameObject projectile, float projectileSpeed, Vector2 direction, float shootPosOffset)
    {
        GameObject temp = null;
        bool findInDic = false;

        (temp, findInDic) = SlimePoolManager.Instance.Find(projectile);

        if (findInDic && temp != null)
        {
            temp.SetActive(true);
        }
        else
        {
            temp = Instantiate(projectile, SlimePoolManager.Instance.transform);
        }

        temp.transform.position = (Vector2)SlimeGameManager.Instance.CurrentPlayerBody.transform.position + (direction * shootPosOffset);
        temp.GetComponent<PlayerProjectile>().OnSpawn(direction, projectileSpeed);

        SoundManager.Instance.PlaySoundBox("SlimeSkill0");
        EventManager.TriggerEvent("PlayerShoot");
    } // 이 코드 Reflection으로

    public void DoReflection(PlayerReflectionProjectileData projectileData, Vector2 direction)
    {
        GameObject temp = null;
        bool findInDic = false;

        (temp, findInDic) = SlimePoolManager.Instance.Find(projectileData.projectile);

        if (findInDic && temp != null)
        {
            temp.SetActive(true);
        }
        else
        {
            temp = Instantiate(projectileData.projectile, SlimePoolManager.Instance.transform);
        }

        temp.transform.position = (Vector2)SlimeGameManager.Instance.CurrentPlayerBody.transform.position + (direction * projectileData.shootPosOffset);
        temp.GetComponent<PlayerProjectile>().OnSpawn(direction, projectileData.projectileSpeed);

        SoundManager.Instance.PlaySoundBox("SlimeSkill0");
        EventManager.TriggerEvent("PlayerShoot");
    } // 이 코드 Reflection으로
}
