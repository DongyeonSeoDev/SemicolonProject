using DG.Tweening;
using UnityEngine;
using System;

public class AcquisitionDropIcon : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;

    private Transform target;

    private bool isDropping; //떨어중인가
    private bool isFollowing; //플레이어쪽으로 가는 중인가
    private bool isTweening;

    private float followStartTime;

    private float curSpeed;
    [SerializeField] private float startSpeed = 4.5f;
    [SerializeField] private float acceleration = 1.8f;

    private Action end;

    private void Init()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rigid = GetComponent<Rigidbody2D>();
        }
    }

    public void Set(Sprite spr, Vector2 startPos, float gravity, Vector2 velocity, Action endAction)
    {
        Init();
        spriteRenderer.sprite = spr;
        isDropping = true;
        isFollowing = false;
        isTweening = false;

        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        spriteRenderer.color = Color.white;

        rigid.gravityScale = gravity;
        rigid.velocity = velocity;
        end = endAction;

        end += () =>
        {
            isTweening = false;
            gameObject.SetActive(false);
        };

        followStartTime = Time.time + 1.2f;
    }

    private void Update()
    {
        if(isDropping && Time.time > followStartTime)
        {
            isDropping = false;
            isFollowing = true;

            rigid.gravityScale = 0;
            rigid.velocity = Vector3.zero;
            target = Global.GetSlimePos;

            curSpeed = startSpeed;
        }
        if(isFollowing)
        {
            if (target == null) target = Global.GetSlimePos;
            Vector3 v = target.position - transform.position;
            transform.position += v.normalized * curSpeed * Time.deltaTime;
            curSpeed += acceleration * Time.deltaTime;

            if(v.sqrMagnitude < 0.8f)
            {
                isFollowing = false;
                isTweening = true;

                transform.DOScale(Vector3.zero, 0.65f).OnComplete(() => end?.Invoke());
            }
        }
        if(isTweening)
        {
            if (target == null) target = Global.GetSlimePos;
            transform.position = target.position;
        }
    }
}
