using UnityEngine;
using Enemy;

public class PlayerReflectionScript : MonoBehaviour
{
    public void DoReflection(Type type, Vector2 direction, float projectileDamage)
    {
        EnemyBullet enemyBullet = EnemyPoolManager.Instance.GetPoolObject(type, (Vector2)SlimeGameManager.Instance.CurrentPlayerBody.transform.position).GetComponent<EnemyBullet>();
        
        enemyBullet.Init(EnemyController.PLAYER, direction, projectileDamage, Color.green);
        enemyBullet.isReflection = true;

        SoundManager.Instance.PlaySoundBox("SlimeSkill0");
        EventManager.TriggerEvent("PlayerShoot");
    }
}
