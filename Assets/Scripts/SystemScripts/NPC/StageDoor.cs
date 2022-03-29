using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using Water;

public class StageDoor : InteractionObj
{
    private SpriteRenderer spr;

    //public FakeSpriteOutline fsOut;

    public StageDataSO nextStageData;

    //[SerializeField] private bool notExistMap; //test

    public DoorDirType dirType; //door dir
    public Transform playerSpawnPos;

    public Light2D doorLight;

    private bool isOpen; //������ ������ �� �ִ� ���°� �Ǿ ��ȣ�ۿ� Ű�� ������ �� true��
    private bool isExitDoor; //�� ���� �Ա����°�
    public bool IsExitDoor { set => isExitDoor = value; }

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();

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
        if(StageManager.Instance.IsStageClear)
        {
            if(nextStageData.areaType == AreaType.BOSS)
            {
                UIManager.Instance.RequestSystemMsg("���� ���� �� ���ٴϱ�");
                return;
            }

            if (!isOpen && !isExitDoor) //���� �����ų� �Ա��� �� ���̸� ��ȣ�ۿ� �ƿ� �ȵǰ�
            {
                isOpen = true;
                StageManager.Instance.PassDir = dirType;
                UIManager.Instance.StartLoading(() => StageManager.Instance.NextStage(nextStageData.stageID), () => EventManager.TriggerEvent("StartNextStage"));
            }
        }
        else
        {
            if (StageManager.Instance.CurrentAreaType == AreaType.MONSTER)
                UIManager.Instance.RequestSystemMsg("���Ͱ� �������� ���� ���� �������� ������ �� �����ϴ�.");
            else
                UIManager.Instance.RequestSystemMsg("������ ������ �� �����ϴ�.");
        }
    }

    public void Open()
    {
        if (isExitDoor || !gameObject.activeSelf) return;

        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Open"];
        isOpen = false;
        objName = StateManager.Instance.stateCountDict[StateAbnormality.Blind] > 0 ? "???" : Global.AreaTypeToString(nextStageData.areaType);
        doorLight.gameObject.SetActive(true);
    }

    public void Close()
    {
        if (isExitDoor || !gameObject.activeSelf) return;

        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Close"];
        doorLight.gameObject.SetActive(false);
    }

    public void Pass()
    {
        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Exit"];
        isExitDoor = true;
        doorLight.gameObject.SetActive(false);
    }

    public override void SetInteractionUI(bool on)
    {
        if (StageManager.Instance.IsStageClear && !isExitDoor)
        {
            base.SetInteractionUI(on);

            PoolManager.GetItem<MobSpeciesIcon>("MobSpeciesIcon").Set(itrUI.GetComponent<RectTransform>(), nextStageData.enemySpeciesArea);

            /*if(fsOut)
               fsOut.gameObject.SetActive(on); */
        }
    }

    public override void SetUI(bool on)
    {
        if (StageManager.Instance.IsStageClear && !isExitDoor)
        {
            base.SetUI(on);
        }
    }
}
