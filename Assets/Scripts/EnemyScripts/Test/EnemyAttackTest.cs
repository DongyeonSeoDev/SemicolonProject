using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
    public void EnemyAttack(int damage)
    {
        Debug.Log("적이 " + damage + "의 공격을 했습니다");
    }
}
