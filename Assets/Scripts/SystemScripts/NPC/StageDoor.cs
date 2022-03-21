using System.Collections.Generic;
using UnityEngine;

public class StageDoor : InteractionObj
{
    private SpriteRenderer spr;

    public FakeSpriteOutline fsOut;

    public StageDataSO nextStageData;

    [SerializeField] private bool notExistMap;

    public DoorDirType dirType;

    private bool isOpen; //������ ������ �� �ִ� ���°� �Ǿ ��ȣ�ۿ� Ű�� ������ �� true��

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        objName = Global.AreaTypeToString(nextStageData.areaType);
    }

    public override void Interaction()
    {
        if(StageManager.Instance.IsStageClear)
        {
            if(notExistMap)
            {
                UIManager.Instance.RequestSystemMsg("���ư���~", 65, 1);
                return;
            }

            if (!isOpen)
            {
                isOpen = true;
                UIManager.Instance.StartLoading(() => StageManager.Instance.NextStage(nextStageData.stageID), () => EventManager.TriggerEvent("StartNextStage", nextStageData.stageName));
            }
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("���Ͱ� �������� ���� ���� �������� ������ �� �����ϴ�.");
        }
    }

    public void Open()
    {
        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Open"];
        isOpen = false;
    }

    public void Close()
    {
        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Close"];
    }

    public override void SetInteractionUI(bool on)
    {
        if (StageManager.Instance.IsStageClear)
        {
            base.SetInteractionUI(on);

            if(fsOut)
               fsOut.gameObject.SetActive(on); 
        }
    }

    public override void SetUI(bool on)
    {
        if (StageManager.Instance.IsStageClear)
        {
            base.SetUI(on);
        }
    }
}
