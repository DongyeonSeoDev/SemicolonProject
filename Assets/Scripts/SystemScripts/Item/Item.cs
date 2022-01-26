using UnityEngine;
using Water;

public class Item : MonoBehaviour
{
    [SerializeField] protected ItemSO _itemData;
    public ItemSO itemData { get { return _itemData; } }

    protected int droppedCount = 1;
    public int DroppedCnt { get { return droppedCount; } }

    protected int rotateDir;
    protected float rotateSpeed;
    protected bool isDropping = false;

    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;
    protected Animator anim;

    protected Vector2 spawnPos;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public virtual void SetData(int id, int droppedCount = 1)
    {
        _itemData = GameManager.Instance.GetItemData(id);
        spriteRenderer.sprite = _itemData.GetSprite();
        this.droppedCount = droppedCount;

        transform.rotation = Quaternion.identity;
        spawnPos = transform.position;

        rigid.gravityScale = 1;
        rigid.velocity = Vector2.up * Random.Range(3f, 6f);
        rotateDir = Random.Range(-1, 2);
        rotateSpeed = Random.Range(3f, 9f);

        isDropping = true;
    }

    protected virtual void Update()
    {
        if(isDropping)
        {
            transform.Rotate(Global.Z90 * rotateDir * rotateSpeed * Time.deltaTime);
            if(transform.position.y <= spawnPos.y)
            {
                rigid.gravityScale = 0;
                rigid.velocity = Vector3.zero;

                isDropping = false;

                anim.enabled = true;
            }
        }
    }

    protected virtual void OnDisable()
    {
        rigid.gravityScale = 0;
        rigid.velocity = Vector3.zero;
        anim.enabled = false;
        isDropping = false;
    }
}
