using System;
using System.Collections.Generic;
using Water;
using UnityEngine.Experimental.Rendering.Universal;

public class RecoveryObj : InteractionObj
{
    private Light2D recoveryLight;
    private List<Action> recoveryActions = new List<Action>();
    //public FakeSpriteOutline fsOut;
    private OutlineCtrl outlineCtrl;

    private bool canInteract;
    private UnityEngine.Vector3 effOffset = new UnityEngine.Vector3(0,-0.3f);

    private void Awake()
    {
        outlineCtrl = GetComponent<OutlineCtrl>();
    }

    private void ResetActionList()
    {
        recoveryActions.Clear();  //�ʱ�ȭ�ϴ� ������ �Ű������� List<Action>�� �����ϰ� �� �Լ� ������ �� � �Լ��� �߰��ϰ� �Ǵµ� Clear�� ���ϸ� �� �߰��� �Լ��� �� �߰��ǰ� �̰� ���̰� �ȴ�.

        recoveryActions.Add(() => RecoveryPlayerHP(30));
        recoveryActions.Add(RemoveImprecation);

        for (int i = 0; i < recoveryActions.Count; i++)
        {
            recoveryActions[i] += DefaultFunc;
        }
    }

    private void DefaultFunc()
    {
        StageManager.Instance.SetClearStage();
        EffectManager.Instance.CallFollowTargetGameEffect("RecoveryEff", GameManager.Instance.slimeFollowObj, effOffset, 3f);
        recoveryLight.DOIntensity(0, 1f, true, () => recoveryLight.gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        if (StageManager.Instance.CurrentAreaType == AreaType.RECOVERY)
        {
            canInteract = true;

            recoveryLight = PoolManager.GetItem<Light2D>("NormalPointLight2D");
            recoveryLight.transform.position = StageManager.Instance.CurrentStageGround.objSpawnPos.position;
            recoveryLight.intensity = 1;
        }
    }
    private void OnDisable()
    {
        if (StageManager.Instance.CurrentAreaType == AreaType.RECOVERY)
        {
            canInteract = false;

            recoveryLight.gameObject.SetActive(false);
        }
    }

    public override void Interaction()
    {
        if (!canInteract)
        {
            UIManager.Instance.RequestSystemMsg("�� �̻� ȸ�� ȿ���� ���� �� �����ϴ�.");
            return;
        }

        ResetActionList();  

        UIManager.Instance.RequestSelectionWindow("� ȿ���� �����ðڽ��ϱ�?", recoveryActions, new List<string>() { "ü�� 30% ȸ��", "���� ����" });
        canInteract = false;
    }

    void RecoveryPlayerHP(float value)
    {
        ItemUseMng.IncreaseCurrentHP(value);
    }
    void RemoveImprecation()
    {
        StateManager.Instance.RemoveAllStateAbnormality();
        //UIManager.Instance.RequestLeftBottomMsg("��� ���ְ� �����Ǿ����ϴ�.");
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        outlineCtrl.SetIntensity(on?10:0);
        //fsOut.gameObject.SetActive(on);
    }
}
