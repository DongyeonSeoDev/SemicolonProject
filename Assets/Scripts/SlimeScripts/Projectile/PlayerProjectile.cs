using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private SlimePoolManager slimePoolManager = null;
    private PlayerInput playerInput = null;

    [SerializeField]
    private GameObject onCrashEffect = null;

    [SerializeField]
    private LayerMask whatIsCrashable;
    [SerializeField]
    private LayerMask whatIsEnemy;

    private Rigidbody2D rigid = null;

    private Vector2 moveVec = Vector2.zero;

    private float moveSpeed = 1f;

    [SerializeField]
    private float moveTime = 3f;
    private float moveTimer = 0f;

    [SerializeField]
    private float knockBackPower = 0f;

    private void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;
        playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();

        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        CheckMoveTime();
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", Despawn);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", Despawn);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (whatIsCrashable.CompareGameObjectLayer(other.gameObject))
        {
            if (whatIsEnemy.CompareGameObjectLayer(other.gameObject))
            {
                Enemy.Enemy enemy = other.GetComponent<Enemy.Enemy>();

                if (enemy != null)
                {
                    SlimeGameManager.Instance.Player.GiveDamage(enemy, SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, 0f, knockBackPower, true);
                }
            }

            ShowOnCrashEffect();
            Despawn();
        }
    }
    private void ShowOnCrashEffect()
    {
        GameObject target = null;
        bool foundObj = false;

        (target, foundObj) = slimePoolManager.Find(onCrashEffect);

        if(foundObj && target != null)
        {
            target.SetActive(true);
        }
        else
        {
            target = Instantiate(onCrashEffect, SlimePoolManager.Instance.transform);
        }

        PlayerOnCrashProjectileEffect effect = target.GetComponent<PlayerOnCrashProjectileEffect>();

        effect.OnSpawn(transform.rotation, transform.position);
    }
    public void OnSpawn(Vector2 direction, float speed)
    {
        //r = distance
        //x = direction.x

        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

        moveVec = direction;

        moveTimer = moveTime;
        moveSpeed = speed;
    }
    private void Move()
    {
        rigid.velocity = moveVec * moveSpeed;
    }
    private void CheckMoveTime()
    {
        if (moveTimer > 0f)
        {
            moveTimer -= Time.deltaTime;

            if (moveTimer <= 0f)
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
