using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGround : MonoBehaviour
{
    [SerializeField] private int enemyCount;
    public int EnemyCnt => enemyCount;

    public Collider2D camStageCollider;

    public StageDoor[] stageDoors;

    private void OnEnable()
    {
        Enemy.EnemyManager.Instance.enemyCount = enemyCount;
    }

    public void OpenDoor()
    {
        for(int i=0; i<stageDoors.Length; i++)
        {
            stageDoors[i].Open();
        }
    }
}
