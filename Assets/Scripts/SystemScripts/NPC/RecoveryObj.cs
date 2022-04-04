using System;
using System.Collections.Generic;
using Water;
using UnityEngine.Experimental.Rendering.Universal;

public class RecoveryObj : InteractionObj
{
    private Light2D recoveryLight;
    private List<Action> recoveryActions = new List<Action>();
    public FakeSpriteOutline fsOut;

    private bool canInteract;

    private void ResetActionList()
    {
        recoveryActions.Clear();  //초기화하는 이유는 매개변수로 List<Action>을 전달하고 그 함수 내에서 또 어떤 함수를 추가하게 되는데 Clear를 안하면 그 추가된 함수가 또 추가되고 이게 쌓이게 된다.

        recoveryActions.Add(() => RecoveryPlayerHP(30));
        recoveryActions.Add(RemoveImprecation);

        for (int i = 0; i < recoveryActions.Count; i++)
        {
            recoveryActions[i] += () => 
            {
                StageManager.Instance.SetClearStage();
                EffectManager.Instance.CallFollowTargetGameEffect("RecoveryEff", SlimeGameManager.Instance.CurrentPlayerBody.transform, UnityEngine.Vector3.down, 3f);
                recoveryLight.DOIntensity(0, 1f, true, () => recoveryLight.gameObject.SetActive(false));
            };
        }
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
            UIManager.Instance.RequestSystemMsg("더 이상 회복 효과를 받을 수 없습니다.");
            return;
        }

        ResetActionList();  

        UIManager.Instance.RequestSelectionWindow("어떤 효과를 받으시겠습니까?", recoveryActions, new List<string>() { "체력 30% 회복", "저주 해제" });
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
        fsOut.gameObject.SetActive(on);
    }
}
