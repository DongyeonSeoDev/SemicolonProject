using UnityEngine;

[CreateAssetMenu(fileName = "Lobby Event", menuName = "Scriptable Object/Lobby Event", order = int.MaxValue)]
public class EnterLobbyEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        if (TutorialManager.Instance.IsTutorialStage)
        {
            //ī�޶� �������� �� ������
            GameManager.Instance.savedData.tutorialInfo.isEnded = true;
            StageManager.Instance.SaveStage("Stage0-06");
        }

        StageManager.Instance.canNextStage = CanNextStage;
        StageManager.Instance.SetClearStage();
    }

    private bool CanNextStage()
    {
        foreach (bool value in GameManager.Instance.savedData.userInfo.uiActiveDic.keyValueDic.Values)
        {
            if (!value)
            {
                KeyActionManager.Instance.SetPlayerHeadText("���� �غ� �� �� �� ����.", 2.5f);
                return false;
            }
        }
        return true;
    }
}
