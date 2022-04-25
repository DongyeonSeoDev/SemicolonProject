using UnityEngine;
public interface ICanGetDamagableEnemy
{
    public void GetDamage(int damage, bool critical = false, bool isKnockBack = false, float knockBackPower = 20f, float stunTime = 1f, Vector2? direction = null);
}
