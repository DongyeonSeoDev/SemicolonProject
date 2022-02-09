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
    protected Vector2 spawnPos;


    [SerializeField] protected Transform itemSprTrm;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid;
    protected Animator anim;
    

    protected virtual void Awake()
    {
        spriteRenderer = itemSprTrm.GetComponent<SpriteRenderer>();
        anim = itemSprTrm.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    public virtual void SetData(int id, Vector3 enemyPos, int droppedCount = 1)
    {
        _itemData = GameManager.Instance.GetItemData(id);
        spriteRenderer.sprite = _itemData.GetSprite();
        this.droppedCount = droppedCount;

        transform.position = enemyPos;
        itemSprTrm.rotation = Quaternion.identity;
        spawnPos = enemyPos;

        rigid.gravityScale = 1;
        rigid.velocity = Vector2.up * Random.Range(4f, 6f);
        rotateDir = Random.Range(-1, 2);
        rotateSpeed = Random.Range(3f, 9f);

        isDropping = true;

        GameManager.Instance.droppedItemList.Add(this);
    }

    public void FollowEffect()
    {
        PoolManager.GetItem("ItemFollowEffect").GetComponent<ItemCloneEffect>().Set(spriteRenderer.sprite, SlimeGameManager.Instance.CurrentPlayerBody.transform, transform.position, itemSprTrm.rotation);
    }

    protected virtual void Update()
    {
        if(isDropping)
        {
            itemSprTrm.Rotate(Global.Z90 * rotateDir * rotateSpeed * Time.deltaTime);
            if(transform.position.y < spawnPos.y)
            {
                rigid.gravityScale = 0;
                rigid.velocity = Vector3.zero;
                itemSprTrm.rotation = Quaternion.identity;

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
