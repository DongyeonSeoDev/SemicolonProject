using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDoor : InteractionObj
{
    private SpriteRenderer spr;

    public FakeSpriteOutline fsOut;

    public StageDataSO nextStageData;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Close();
    }

    public override void Interaction()
    {
        if(StageManager.Instance.IsStageClear)
        {
            UIManager.Instance.StartLoading(() => StageManager.Instance.NextStage(nextStageData.stageID), null); //null에는 나중에 스테이지 입장 시 이벤트를 발생시킬 것 (지역 이름 잠시동안 뜨기 등)
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("몬스터가 남아있을 때는 다음 지역으로 입장할 수 없습니다.");
        }
    }

    public void Open()
    {
        spr.sprite = StageManager.Instance.openDoorSpr;
    }

    public void Close()
    {
        spr.sprite = StageManager.Instance.closeDoorSpr;
    }

    public override void SetInteractionUI(bool on)
    {
        if (StageManager.Instance.IsStageClear)
        {
            base.SetInteractionUI(on);

            fsOut.gameObject.SetActive(on);
            //스테이지 정보도 띄워야 함. 
        }
    }

    public override void SetUI(bool on)
    {
        //base.SetUI(on); 
    }
}
