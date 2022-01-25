using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private Stat playerStat = null;
    private SlimePoolManager slimePoolManager = null;

    [SerializeField]
    private LayerMask whatIsEnemy;

    private Rigidbody2D rigid = null;

    private Vector2 moveVec = Vector2.zero;

    private float moveSpeed = 1f;

    [SerializeField]
    private float moveTime = 3f;
    private float moveTimer = 0f;

    private void Awake() 
    {
        slimePoolManager = SlimePoolManager.Instance;

        rigid = GetComponent<Rigidbody2D>();
    }
    private void Start() 
    {
        playerStat = SlimeGameManager.Instance.PlayerStat;
    }

    void Update()
    {
        Move();
        CheckMoveTime();
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(whatIsEnemy.CompareGameObjectLayer(other.gameObject))
        {
            Enemy.Enemy enemy = other.GetComponent<Enemy.Enemy>();

            enemy.GetDamage(playerStat.eternalStat.damage + playerStat.additionalEternalStat.damage);
        }

        Despawn();
    }
    public void OnSpawn(Vector2 direction, float speed)
    {
        moveVec = direction;
        moveSpeed = speed;

        moveTimer = moveTime;
    }
    private void Move()
    {
        rigid.velocity = moveVec * moveSpeed;
    }
    private void CheckMoveTime()
    {
        if(moveTimer > 0f)
        {
            moveTimer -= Time.deltaTime;

            if(moveTimer <= 0f)
            {
                moveTimer = 0f;

                Despawn();
            }
        }
    }
    private void Despawn()
    {
        moveSpeed = 0f;
        moveVec = Vector2.zero;

        slimePoolManager.AddObject(gameObject);
        gameObject.SetActive(false);
    }
}
