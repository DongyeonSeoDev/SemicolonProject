using UnityEngine;
public interface ICanGetDamagableEnemy
{
    public Transform GetTransform();
    public GameObject GetGameObject();
    public void GetDamage(int damage, bool critical, bool isKnockBack, bool isStun, bool isShowText = true, float knockBackPower = 20f, float stunTime = 1f, Vector2? direction = null);
    public float EnemyHpPercent();
}
