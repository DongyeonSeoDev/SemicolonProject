using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class PlayerReflectionScript : MonoBehaviour
{
    public void DoReflection(Enemy.Type type, Vector2 direction, float projectileDamage)
    {
        Enemy.EnemyPoolManager.Instance.GetPoolObject(type, (Vector2)SlimeGameManager.Instance.CurrentPlayerBody.transform.position).GetComponent<EnemyBullet>().Init(EnemyController.PLAYER, direction, projectileDamage, Color.green);

        SoundManager.Instance.PlaySoundBox("SlimeSkill0");
        EventManager.TriggerEvent("PlayerShoot");
    }
}
