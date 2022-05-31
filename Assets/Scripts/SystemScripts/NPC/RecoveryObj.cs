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

        recoveryActions.Clear();  //초기화하는 이유는 매개변수로 List<Action>을 전달하고 그 함수 내에서 또 어떤 함수를 추가하게 되는데 Clear를 안하면 그 추가된 함수가 또 추가되고 이게 쌓이게 된다.

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
            UIManager.Instance.RequestSystemMsg("더 이상 회복 효과를 받을 수 없습니다.");
            return;
        }

        ResetActionList();  

        UIManager.Instance.RequestSelectionWindow("이곳은 회복구역입니다.\n어떤 효과를 받으시겠습니까?", recoveryActions, new List<string>() { "AscHp", "AntiBuffRm" }, true, iconActiveConditions, true);
        canInteract = false;
    }

    void RecoveryPlayerHP(float value)
    {
        ItemUseMng.IncreaseCurrentHP(value);
    }
    void RemoveImprecation()
    {
        StateManager.Instance.RemoveAllStateAbnormality();
        //UIManager.Instance.RequestLeftBottomMsg("모든 저주가 해제되었습니다.");
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        //outlineCtrl.SetIntensity(on?10:0);
        //fsOut.gameObject.SetActive(on);
    }
}
