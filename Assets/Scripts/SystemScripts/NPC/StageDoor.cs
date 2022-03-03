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
            UIManager.Instance.StartLoading(() => StageManager.Instance.NextStage(nextStageData.stageID), null); //null���� ���߿� �������� ���� �� �̺�Ʈ�� �߻���ų �� (���� �̸� ��õ��� �߱� ��)
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("���Ͱ� �������� ���� ���� �������� ������ �� �����ϴ�.");
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
            //�������� ������ ����� ��. 
        }
    }

    public override void SetUI(bool on)
    {
        //base.SetUI(on); 
    }
}
