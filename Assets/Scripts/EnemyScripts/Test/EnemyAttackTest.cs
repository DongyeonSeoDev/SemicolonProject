using UnityEngine;

public class EnemyAttackTest : MonoBehaviour // 적 공격 테스트
{
    public void EnemyAttack(int damage)
    {
        Debug.Log("적이 " + damage + "의 공격을 했습니다");
    }
}
