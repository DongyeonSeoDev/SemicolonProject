using System.Collections.Generic;
using UnityEngine;

public class StageDoor : InteractionObj
{
    private SpriteRenderer spr;

    //public FakeSpriteOutline fsOut;

    public StageDataSO nextStageData;

    //[SerializeField] private bool notExistMap; //test

    public DoorDirType dirType; //door dir
    public Transform playerSpawnPos;

    private bool isOpen; //������ ������ �� �ִ� ���°� �Ǿ ��ȣ�ۿ� Ű�� ������ �� true��
    private bool isExitDoor; //�� ���� �Ա����°�
    public bool IsExitDoor { set => isExitDoor = value; }

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    public override void Interaction()
    {
        if(StageManager.Instance.IsStageClear)
        {
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
        objName = Global.AreaTypeToString(nextStageData.areaType);
    }

    public void Close()
    {
        if (isExitDoor || !gameObject.activeSelf) return;

        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Close"];
    }

    public void Pass()
    {
        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Exit"];
        isExitDoor = true;
    }

    public override void SetInteractionUI(bool on)
    {
        if (StageManager.Instance.IsStageClear && !isExitDoor)
        {
            base.SetInteractionUI(on);

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
