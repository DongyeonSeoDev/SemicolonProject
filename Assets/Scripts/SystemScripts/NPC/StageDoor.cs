using System.Collections.Generic;
using UnityEngine;

public class StageDoor : InteractionObj
{
    private SpriteRenderer spr;

    //public FakeSpriteOutline fsOut;

    public StageDataSO nextStageData;

    [SerializeField] private bool notExistMap; //test

    public DoorDirType dirType; //door dir
    public Transform playerSpawnPos;

    private bool isOpen; //문으로 입장할 수 있는 상태가 되어서 상호작용 키를 눌렀을 때 true로
    private bool isExitDoor; //이 문이 입구였는가
    public bool IsExitDoor { set => isExitDoor = value; }

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
                UIManager.Instance.RequestSystemMsg("좋은말 할 때 돌아가라~\n-양진욱-", 65, 1);
                return;
            }

            if (!isOpen && !isExitDoor)
            {
                isOpen = true;
                StageManager.Instance.PassDir = dirType;
                UIManager.Instance.StartLoading(() => StageManager.Instance.NextStage(nextStageData.stageID), () => EventManager.TriggerEvent("StartNextStage", nextStageData.stageName));
            }
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("몬스터가 남아있을 때는 다음 지역으로 입장할 수 없습니다.");
        }
    }

    public void Open()
    {
        if (isExitDoor || !gameObject.activeSelf) return;

        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Open"];
        isOpen = false;
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
