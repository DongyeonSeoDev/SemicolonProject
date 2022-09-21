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
            UIManager.Instance.RequestSystemMsg("더 이상 저주 효과를 받을 수 없습니다.");
            return;
        }

        GetRandomAntiBuff();

        /*ResetActionList();

        UIManager.Instance.RequestSelectionWindow("이곳은 저주구역입니다.\n어떤 효과를 적용하시겠습니까?", imprecationActions, new List<string>() { "DescHp", "RandAntiBuff", "RandItemRm" }, true,
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
        //아직 가난 저주가 없기 때문에 원래는 이 코드지만 임시 코드 작성함
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
        UIManager.Instance.RequestLogMsg("저주로 임의의 인벤토리 한 칸의 아이템을 모두 잃었습니다.");
        Inventory.Instance.RemoveRandomItem();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        //fsOut.gameObject.SetActive(on);
    }
}
