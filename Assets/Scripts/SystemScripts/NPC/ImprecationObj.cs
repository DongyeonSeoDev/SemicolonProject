using System;
using System.Collections.Generic;
using UnityEngine;

public class ImprecationObj : InteractionObj
{
    private List<Action> imprecationActions = new List<Action>();
    public FakeSpriteOutline fsOut;

    private bool canInteract;

    private void ResetActionList()
    {
        imprecationActions.Clear();

        imprecationActions.Add(() => DecreasePlayerHp(40));
        imprecationActions.Add(OnStateAbnormal);
        imprecationActions.Add(RemoveRandomItem);

        for (int i = 0; i < imprecationActions.Count; i++)
        {
            imprecationActions[i] += DefaultFunc;
        }
    }

    private void DefaultFunc()
    {
        StageManager.Instance.SetClearStage();
        EffectManager.Instance.CallFollowTargetGameEffect("ImprecationEff", GameManager.Instance.slimeFollowObj, Vector3.up, 2f);
        SoundManager.Instance.PlaySoundBox("GetDebuffSFX");
    }

    private void OnEnable()
    {
        canInteract = true;
    }
    private void OnDisable()
    {
        canInteract = false;
    }

    public override void Interaction()
    {
        if (!canInteract)
        {
            UIManager.Instance.RequestSystemMsg("�� �̻� ���� ȿ���� ���� �� �����ϴ�.");
            return;
        }

        GetRandomAntiBuff();

        /*ResetActionList();

        UIManager.Instance.RequestSelectionWindow("�̰��� ���ֱ����Դϴ�.\n� ȿ���� �����Ͻðڽ��ϱ�?", imprecationActions, new List<string>() { "DescHp", "RandAntiBuff", "RandItemRm" }, true,
            new List<Func<bool>>() {()=>Global.CurrentPlayer.PlayerStat.currentHp>=1.5f, null, () => Inventory.Instance.ActiveSlotCount > 0}, true);*/
        canInteract = false;
    }

    void GetRandomAntiBuff()
    {
        if(UnityEngine.Random.Range(0,2) == 0)
        {
            DecreasePlayerHp(20);
        }
        else
        {
            List<StatElement> list = Global.CurrentPlayer.PlayerStat.choiceStat.AllStats.FindAll(x =>
            {
                ChoiceStatSO data = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(x.id);
                return data.needStatID > 0 && data.charType == CharType.STORE
                && !data.plusStat && x.statLv < x.maxStatLv;
            });
            if(list.Count > 0)
            {
                ushort id = list.ToRandomElement().id;
                NGlobal.playerStatUI.StatUnlock(NGlobal.playerStatUI.choiceStatDic[id]);
                Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(id);
            }
            else
            {
                DecreasePlayerHp(20);
            }
        }

        DefaultFunc();
    }

    void DecreasePlayerHp(float value)
    {
        ItemUseMng.DecreaseCurrentHP(value);
    }

    void OnStateAbnormal()
    {
        //���� ���� ���ְ� ���� ������ ������ �� �ڵ����� �ӽ� �ڵ� �ۼ���
        //StateAbnormality antiBuff = (StateAbnormality)UnityEngine.Random.Range(0, Global.EnumCount<StateAbnormality>()-1);

        StateAbnormality antiBuff = StateAbnormality.None;
        do
        {
            antiBuff = (StateAbnormality)UnityEngine.Random.Range(0, Global.EnumCount<StateAbnormality>() - 1);
        } while(antiBuff == StateAbnormality.Poverty);

        StateManager.Instance.StartStateAbnormality(antiBuff);
    }

    void RemoveRandomItem()
    {
        UIManager.Instance.RequestLogMsg("���ַ� ������ �κ��丮 �� ĭ�� �������� ��� �Ҿ����ϴ�.");
        Inventory.Instance.RemoveRandomItem();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        //fsOut.gameObject.SetActive(on);
    }
}
