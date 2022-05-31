using System;
using System.Collections.Generic;
using Water;
using UnityEngine.Experimental.Rendering.Universal;
using FkTweening;
using UnityEngine;

public class RecoveryObj : InteractionObj
{
    [SerializeField] private Light2D recoveryLight;
    private List<Action> recoveryActions = new List<Action>();
    private List<Func<bool>> iconActiveConditions = new List<Func<bool>>();
    //public FakeSpriteOutline fsOut;
    private OutlineCtrl outlineCtrl;

    private bool canInteract;
    private Vector3 effOffset = new Vector3(0,-0.3f);

   /* private void Awake()
    {
        outlineCtrl = GetComponent<OutlineCtrl>();
    }*/

    private void ResetActionList()
    {
        if(iconActiveConditions.Count == 0)
        {
            iconActiveConditions.Add(() => !StateManager.Instance.IsPlayerFullHP);
            iconActiveConditions.Add(() => !StateManager.Instance.IsPlayerNoImpr);
        }

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
        SoundManager.Instance.PlaySoundBox("GetBuffSFX");
        recoveryLight.DOIntensity(0, 1f, true, () => recoveryLight.gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        if (StageManager.Instance.CurrentAreaType == AreaType.RECOVERY)
        {
            canInteract = true;

            //recoveryLight = PoolManager.GetItem<Light2D>("NormalPointLight2D");
            recoveryLight.transform.position = StageManager.Instance.CurrentStageGround.objSpawnPos.position;
            recoveryLight.gameObject.SetActive(true);
            recoveryLight.intensity = 0.4f;
            /*recoveryLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").SetValue(recoveryLight, new int[3]
            {
                0, -1221289887, -992757899
            });*/
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

        UIManager.Instance.RequestSelectionWindow("�̰��� ȸ�������Դϴ�.\n� ȿ���� �����ðڽ��ϱ�?", recoveryActions, new List<string>() { "AscHp", "AntiBuffRm" }, true, iconActiveConditions, true);
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
        //outlineCtrl.SetIntensity(on?10:0);
        //fsOut.gameObject.SetActive(on);
    }
}
