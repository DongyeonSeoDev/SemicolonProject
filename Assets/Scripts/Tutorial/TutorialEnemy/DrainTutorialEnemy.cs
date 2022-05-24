using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class DrainTutorialEnemy : TutorialEnemy
    {
        public override void GetDamage(float damage, bool critical, bool isKnockBack, bool isStun, Vector2 effectPosition, Vector2 direction, float knockBackPower = 20, float stunTime = 1, Vector3? effectSize = null)
        {
        }
    }
}
