using UnityEngine;

[CreateAssetMenu(fileName = "Lobby Event", menuName = "Scriptable Object/Lobby Event", order = int.MaxValue)]
public class EnterLobbyEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        if (TutorialManager.Instance.IsTutorialStage)
        {
            GameManager.Instance.savedData.tutorialInfo.isEnded = true;
            StageManager.Instance.SaveStage("Stage0-06");
            CutsceneManager.Instance.PlayCutscene("TutorialCutscene2");
            NGlobal.playerStatUI.StatUnlock(NGlobal.playerStatUI.PlayerStat.eternalStat.maxHp);
            NGlobal.playerStatUI.StatUnlock(NGlobal.playerStatUI.PlayerStat.eternalStat.minDamage);
            NGlobal.playerStatUI.StatUnlock(NGlobal.playerStatUI.PlayerStat.eternalStat.maxDamage);
            NGlobal.playerStatUI.StatOpen(NGlobal.MaxHpID, true);
            NGlobal.playerStatUI.StatOpen(NGlobal.MinDamageID, true);
            NGlobal.playerStatUI.StatOpen(NGlobal.MaxDamageID, true);
        }

        

        StageManager.Instance.canNextStage = CanNextStage;
        StageManager.Instance.SetClearStage();
    }

    private bool CanNextStage()
    {
        foreach (KeyAction key in GameManager.Instance.savedData.userInfo.uiActiveDic.keyValueDic.Keys)
        {
            if (key == KeyAction.SETTING) continue;

            if (!GameManager.Instance.savedData.userInfo.uiActiveDic[key])
            {
                KeyActionManager.Instance.SetPlayerHeadText("아직 준비가 덜 된 것 같다.", 2.5f);
                return false;
            }
        }
        return true;
    }
}
