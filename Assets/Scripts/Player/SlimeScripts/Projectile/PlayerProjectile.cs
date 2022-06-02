using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private SlimePoolManager slimePoolManager = null;
    private PlayerInput playerInput = null;

    private Rigidbody2D rigid = null;
    private Animator anim = null;

    [SerializeField]
    private GameObject onCrashEffect = null;

    [SerializeField]
    private LayerMask whatIsCrashable;
    [SerializeField]
    private LayerMask whatIsEnemy;

    private Vector2 moveVec = Vector2.zero;
    private Vector2 lastMoveVec = Vector2.zero;

    [Header("Shoot이 기본 데미지의 몇배의 데미지를 줄 것인가에 대한 값")]
    [SerializeField]
    private float damageMagnificationOfShoot = 0.2f;
    private float projectileDamage = 0f; // 해당 값이 0 이상이면 데미지를 줄 떄 이 값을 적용함

    private float moveSpeed = 1f;

    [SerializeField]
    private float moveTime = 3f;
    private float moveTimer = 0f;

    [SerializeField]
    private float knockBackPower = 0f;

    private bool isStop = false;

    private void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;
        playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isStop)
        {
            Move();
            CheckMoveTime();
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerStart", PlayerStart);
        EventManager.StartListening("PlayerStop", PlayerStop);
        EventManager.StartListening("PlayerDead", Despawn);
        EventManager.StartListening("ExitCurrentMap", Despawn);
    }
    private void OnDisable()
    {
        projectileDamage = 0f;

        EventManager.StopListening("PlayerStart", PlayerStart);
        EventManager.StopListening("PlayerStop", PlayerStop);
        EventManager.StopListening("PlayerDead", Despawn);
        EventManager.StopListening("ExitCurrentMap", Despawn);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isCritical = false;

        if (whatIsCrashable.CompareGameObjectLayer(other.gameObject))
        { 
            if (whatIsEnemy.CompareGameObjectLayer(other.gameObject))
            {
                ICanGetDamagableEnemy enemy = other.GetComponent<ICanGetDamagableEnemy>();

                if (enemy != null)
                {
                    if (projectileDamage > 0f)
                    {
                        SlimeGameManager.Instance.Player.GiveDamage(enemy, projectileDamage, projectileDamage, transform.position, moveVec, true, knockBackPower, 0f);
                    }
                    else
                    {
                        SlimeGameManager.Instance.Player.Mag_GiveDamage(enemy, SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, transform.position, moveVec, damageMagnificationOfShoot, true, knockBackPower, 0f);
                    }
                    EventManager.TriggerEvent("OnEnemyAttack");
                }
                else
                {
                    EventManager.TriggerEvent("OnAttackMiss");
                }
            }
            else 
            {
                EventManager.TriggerEvent("OnAttackMiss");
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
    public void OnSpawn(Vector2 direction, float speed, float damage)
    {
        //r = distance
        //x = direction.x

        OnSpawn(direction, speed);

        projectileDamage = damage;
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
    private void PlayerStart()
    {
        if (isStop)
        {
            isStop = false;
            moveVec = lastMoveVec;
            anim.speed = 1f;
        }
    }
    private void PlayerStop()
    {
        if (!isStop)
        {
            isStop = true;
            lastMoveVec = moveVec;
            rigid.velocity = Vector2.zero;
            anim.speed = 0f;
        }
    }
}
