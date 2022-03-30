using UnityEngine;
using Water;

public class Pick : InteractionObj
{
    [SerializeField] private bool isEnemyStage = true;  //���� �ִ� ��������?
    //[SerializeField] private int stageNumber;  //�� ���������� ä��������

    [SerializeField] private float pickSuccessProbability = 50f;  //ä�� ������

    [SerializeField] protected ItemSO _itemData;
    public ItemSO itemData { get { return _itemData; } }

    private SpriteRenderer spr;
    //private OutlineCtrl sprOutline;

    public FakeSpriteOutline fsOut;
    //protected int droppedCount = 1;
    //public int DroppedCnt { get { return droppedCount; } }

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = _itemData.GetSecondSprite();
        objName = _itemData.itemName;
        //sprOutline = GetComponent<OutlineCtrl>();
    }

    private void Start()
    {
        //GameManager.Instance.pickList.Add(this);
        fsOut.gameObject.SetActive(false);
        //sprOutline.SetOutlineIntensity(0);
    }

    public void FollowEffect(bool success = true)
    {
        Transform target;
        System.Action arvAction = null;
        Quaternion rot = Quaternion.identity;

        if (success) target = SlimeGameManager.Instance.CurrentPlayerBody.transform;
        else
        {
            target = PoolManager.GetItem<Transform>("EmptyObject");
            target.position = transform.position - new Vector3(0, 6);
            rot = Quaternion.Euler(0, 0, Random.Range(-60f, 60f));
            arvAction = () => target.gameObject.SetActive(false);
        }

        PoolManager.GetItem<ItemCloneEffect>("ItemFollowEffect").Set(spr.sprite, target, transform.position, rot, arvAction);
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

        fsOut.gameObject.SetActive(on);
        //sprOutline.SetOutlineIntensity(on?1:0);
    }

    public override void Interaction()
    {
        if(isEnemyStage && !StageManager.Instance.IsStageClear)
        {
            UIManager.Instance.RequestSystemMsg("���� ä���� �� �����ϴ�.");
            return;
        }

        if (!Inventory.Instance.CanCombine(_itemData.id, 1))
        {
            UIManager.Instance.RequestSystemMsg("�κ��丮�� ����ּ���.");
            return;
        }

        
        if (Random.Range(0f, 100f) < pickSuccessProbability)
        {
            CallEffect("PickSuccessEff");

            //UIManager.Instance.RequestSystemMsg("ä���� �����Ͽ����ϴ�.", Util.Change255To1Color(114, 168, 255, 255));
            EffectManager.Instance.OnWorldTextEffect("ä�� ����", transform.position, Vector3.one, EffectManager.Instance.pickupPlantSucFaiVG.normal);
            Global.MonoActionTrigger(Global.PickupPlant, this);
        }
        else
        {
            CallEffect("PickFailEff");

            //FollowEffect(false);
            EffectManager.Instance.OnWorldTextEffect("ä�� ����", transform.position, Vector3.one, EffectManager.Instance.pickupPlantSucFaiVG.cri);
            //UIManager.Instance.RequestSystemMsg("ä���� �����Ͽ����ϴ�.", Util.Change255To1Color(123, 0, 226, 255));
            gameObject.SetActive(false);
        }
    }

    void CallEffect(string key, float time = 1)
    {
        GameObject effObj = PoolManager.GetItem(key);
        effObj.transform.position = transform.position;
        effObj.GetComponent<ParticleSystem>().Play();
        Util.DelayFunc(() => effObj.SetActive(false), time);
    }
}
