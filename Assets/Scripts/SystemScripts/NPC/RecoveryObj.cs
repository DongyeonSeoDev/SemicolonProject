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

    private bool canInteract;
    private Vector3 effOffset = new Vector3(0,-0.3f);

    private List<StatElement> availableStats;

    private void ResetActionList()
    {
        if(iconActiveConditions.Count == 0)
        {
            iconActiveConditions.Add(() => !StateManager.Instance.IsPlayerFullHP);
            iconActiveConditions.Add(() =>
            {
                availableStats = Global.CurrentPlayer.PlayerStat.choiceStat.AllStats.FindAll(x =>
                {
                    ChoiceStatSO data = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(x.id);
                    return data.needStatID > 0 && data.charType == CharType.STORE
                    && data.plusStat && !NGlobal.playerStatUI.IsUnlockStat(x.id);
                });
                return availableStats.Count > 0;
            });
            //iconActiveConditions.Add(() => !StateManager.Instance.IsPlayerNoImpr);
        }

        recoveryActions.Clear();  //�ʱ�ȭ�ϴ� ������ �Ű������� List<Action>�� �����ϰ� �� �Լ� ������ �� � �Լ��� �߰��ϰ� �Ǵµ� Clear�� ���ϸ� �� �߰��� �Լ��� �� �߰��ǰ� �̰� ���̰� �ȴ�.

        recoveryActions.Add(() => RecoveryPlayerHP(30));
        recoveryActions.Add(GetPlusStatProp);
        //recoveryActions.Add(RemoveImprecation);

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
        //recoveryLight.DOIntensity(0, 1f, true, () => recoveryLight.gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        if (StageManager.Instance.CurrentAreaType == AreaType.RECOVERY)
        {
            canInteract = true;

            //recoveryLight = PoolManager.GetItem<Light2D>("NormalPointLight2D");
            recoveryLight.transform.position = StageManager.Instance.CurrentStageGround.objSpawnPos.position;
            //recoveryLight.gameObject.SetActive(true);
            recoveryLight.intensity = 0.4f;
            /*recoveryLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").SetValue(recoveryLight, new int[3]
            {
                0, -1221289887, -992757899
            });*/
        }
    }

    public void ActiveRecovLight()
    {
        recoveryLight.gameObject.SetActive(true);
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

        UIManager.Instance.RequestSelectionWindow("�̰��� ȸ�������Դϴ�.\n� ȿ���� �����ðڽ��ϱ�?", recoveryActions, new List<string>() { "AscHp", "GetPStat" }, true, iconActiveConditions, true);
        canInteract = false;
    }

    void RecoveryPlayerHP(float value)
    {
        ItemUseMng.IncreaseCurrentHP(value);
    }
    void RemoveImprecation()
    {
        StateManager.Instance.RemoveAllStateAbnormality();
    }
    void GetPlusStatProp()
    {
        ushort id = availableStats.ToRandomElement().id;
        NGlobal.playerStatUI.StatUnlock(NGlobal.playerStatUI.choiceStatDic[id]);
        Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(id);
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
    }
}
