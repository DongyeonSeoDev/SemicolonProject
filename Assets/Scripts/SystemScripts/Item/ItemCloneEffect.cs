using UnityEngine;
using DG.Tweening;
using System;

public class ItemCloneEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform target;

    private float currentSpeed;
    private bool isFollowing, isTweening;
    private Vector3 startSize;

    private Action arriveAction;

    [SerializeField] private float startSpeed = 5.5f;
    [SerializeField] protected float acceleration = 4f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startSize = transform.localScale;
    }

    public void Set(Sprite sprite, Transform target, Vector2 pos, Quaternion rot, Action arriveAction = null)
    {
        transform.position = pos;
        transform.rotation = rot;
        spriteRenderer.sprite = sprite;
        currentSpeed = startSpeed;
        this.target = target;
        this.arriveAction = arriveAction;

        transform.localScale = startSize;
        spriteRenderer.color = Color.white;

        Util.DelayFunc(() => isFollowing = true, 0.2f, this);
    }

    private void Update()
    {
        if(isFollowing)
        {
            if(target == null)
            {
                if(SlimeGameManager.Instance.CurrentPlayerBody != null)
                {
                    target = SlimeGameManager.Instance.CurrentPlayerBody.transform;
                }
                else
                {
                    return;
                }
            }

            Vector3 dir = target.position - transform.position;
            transform.position += dir.normalized * currentSpeed * Time.deltaTime;
            currentSpeed += acceleration * Time.deltaTime;

            if(!isTweening)
            {
                if (dir.sqrMagnitude < 0.07f)
                {
                    isTweening = true;
                    arriveAction?.Invoke();
                    transform.DOScale(Global.zeroPointThree, 0.3f);
                    spriteRenderer.DOColor(Color.clear, 0.4f).OnComplete(() => gameObject.SetActive(false));
                }
            }
        }
    }

    private void OnDisable()
    {
        isFollowing = false;
        isTweening = false;
        arriveAction = null;
    }
}
