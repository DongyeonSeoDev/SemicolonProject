using UnityEngine;
public interface ICanGetDamagableEnemy
{
    public Transform GetTransform();
    public GameObject GetGameObject();
    public void GetDamage(float damage, bool critical, bool isKnockBack, bool isStun, Vector3 direction, Vector3 position, bool isShowText = true, float knockBackPower = 20f, float stunTime = 1f, Vector3? size = null);
    public float EnemyHpPercent();
    public string GetEnemyName();
}
