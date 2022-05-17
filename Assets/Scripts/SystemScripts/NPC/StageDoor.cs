using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using Water;
using FkTweening;

public class StageDoor : InteractionObj, IDamageableBySlimeBodySlap
{
    private SpriteRenderer spr;

    private WorldIcon icon;

    [SerializeField] private int maxHp = 2;
    private int hp;
    //public FakeSpriteOutline fsOut;

    public StageDataSO nextStageData;

    //[SerializeField] private bool notExistMap; //test

    public DoorDirType dirType; //door dir
    public Transform playerSpawnPos;

    public GameObject detectorObj;
    public Light2D doorLight;

    private bool isOpen = false;  //�� ���� ���ؼ� ������ �� �ִ� ���°� �Ǿ��°�
    private bool isEnter; //������ ������ �� �ִ� ���°� �Ǿ ��ȣ�ۿ� Ű�� ������ �� true��
    private bool isExitDoor; //�� ���� �Ա����°�
    private bool isBreak;

    public bool IsOpen => isOpen;

    private float damageableTime = 0.0f;

    public bool IsExitDoor { set => isExitDoor = value; }

    public bool IsBlindState => StateManager.Instance.stateCountDict[StateAbnormality.Blind.ToString()] > 0;

    protected override void Awake()
    {
        spr = GetComponent<SpriteRenderer>();

        if (!playerSpawnPos) playerSpawnPos = transform.GetChild(2);

        float rz = 0f;
        switch(dirType)
        {
            case DoorDirType.FRONT:
                rz = -180f;
                break;
            case DoorDirType.RIGHT:
                rz = 90f;
                break;
            case DoorDirType.LEFT:
                rz = -90f;
                break;
            case DoorDirType.BACK:
                rz = 0f;
                break;
        }
        doorLight.transform.rotation = Quaternion.Euler(0, 0, rz);
    }

    public override void Interaction()
    {
        if(StageManager.Instance.canNextStage != null)
        {
            if (!StageManager.Instance.canNextStage()) return;
        }

        if(StageManager.Instance.IsStageClear || isBreak)  //���� �ν��ų� �������� Ŭ������
        {
            if (!isEnter && !isExitDoor) //���� �����ų� �Ա��� �� ���̸� ��ȣ�ۿ� �ƿ� �ȵǰ�
            {
                isEnter = true;
                StageManager.Instance.PassDir = dirType;
                UIManager.Instance.StartLoading(() => StageManager.Instance.NextStage(nextStageData.stageID), () => EventManager.TriggerEvent("StartNextStage"));
            }
        }
        else
        {
           /* if (StageManager.Instance.CurrentAreaType == AreaType.MONSTER)
                UIManager.Instance.RequestSystemMsg("���Ͱ� �������� ���� ���� �������� ������ �� �����ϴ�.");
            else
                UIManager.Instance.RequestSystemMsg("������ ������ �� �����ϴ�.");*/
        }
    }

    public void Open() //����
    {
        if (isExitDoor || !gameObject.activeSelf || isOpen) return;

        AreaType t = StageManager.Instance.CurrentAreaType;
        if (StageManager.Instance.CurrentStageData.useOpenDoorSound || t == AreaType.MONSTER || t == AreaType.RANDOM || t == AreaType.BOSS)
        {
            SoundManager.Instance.PlaySoundBox("Door Break SFX");
        }

        isOpen = true;
        isEnter = false;
        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Open"];
        objName = IsBlindState ? "???" : Global.AreaTypeToString(nextStageData.areaType);

        detectorObj.SetActive(true);
        doorLight.gameObject.SetActive(true);
        doorLight.intensity = 0.15f;
        doorLight.DOIntensity(0.75f, 0.5f);
        EffectManager.Instance.CallGameEffect("DoorDestEff", transform.position, 1.5f);
    }

    public void Close() //�ݱ�
    {
        if (isExitDoor || !gameObject.activeSelf) return;

        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Close"];
        doorLight.gameObject.SetActive(false);
        detectorObj.SetActive(false);
        isOpen = false;
        //isBreak = false;
    }

    public void Pass() //�� ���� �Ա��� �� ����
    {
        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Exit"];
        isExitDoor = true;
        doorLight.gameObject.SetActive(false);
        detectorObj.SetActive(false);
    }

    public void DoorLightActive(bool on)
    {
        if (isExitDoor) return;

        doorLight.gameObject.SetActive(on);
    }

    public override void SetInteractionUI(bool on)
    {
        if ( (StageManager.Instance.IsStageClear || isBreak) && !isExitDoor) //�ش� �������� Ŭ�����߰ų� ���� �μ��� �����̸鼭 �Ա��� �ƴϸ�
        {
            base.SetInteractionUI(on);

            if (nextStageData.areaType == AreaType.MONSTER && !IsBlindState)  //�� ���������� ���� ���̸鼭 �Ǹ� ���� �Ȱɸ� ���¸� ���� ������ ���
            {
                if (on)
                {
                    if (!icon)
                    {
                        icon = PoolManager.GetItem<WorldIcon>("MobSpeciesIcon");
                        icon.Set(itrUI.GetComponent<RectTransform>(), nextStageData.enemySpeciesArea);
                    }
                }
                else
                {
                    if (icon)
                    {
                        icon.gameObject.SetActive(false);
                        icon = null;
                    }
                }
            }
            /*if(fsOut)
               fsOut.gameObject.SetActive(on); */
        }
    }

    public override void SetUI(bool on)
    {
        if ((StageManager.Instance.IsStageClear || isBreak) && !isExitDoor)
        {
            base.SetUI(on);
        }
    }

    private void OnEnable()
    {
        hp = maxHp;
        isBreak = false;
    }

    private void OnDisable()
    {
        isOpen = false;
    }

    public void GetDamage(int damage, float charging) //�������� �ĸ���
    {
        if (Time.time > damageableTime)
        {
            if (charging < Global.GetSlimePos.GetComponent<PlayerBodySlap>().MaxChargingTime) return;
            if (isOpen || isExitDoor || isBreak) return;
            if (StageManager.Instance.CurrentAreaType == AreaType.BOSS) return;

            damageableTime = Time.time + 1f;

            hp -= damage;
            CinemachineCameraScript.Instance.Shake(2f, 2f, 0.3f);
            EffectManager.Instance.CallGameEffect("DoorHitEff", transform.position, 1.5f);

            if (hp <= 0)
            {
               
                isBreak = true;
                hp = 0;
                Open();
            }
            else
            {
                SoundManager.Instance.PlaySoundBox("Door Hit SFX");
            }
        }
    }
}
