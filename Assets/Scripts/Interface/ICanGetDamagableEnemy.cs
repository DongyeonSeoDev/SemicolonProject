using UnityEngine;
public interface ICanGetDamagableEnemy
{
    public Transform GetTransform();
    public GameObject GetGameObject();
    public string GetEnemyId();
    public void GetDamage(float damage, bool critical, bool isKnockBack, bool isStun, Vector2 effectPosition, Vector2 direction, float knockBackPower = 20f, float stunTime = 1f, Vector3? effectSize = null);
    public float EnemyHpPercent();
}