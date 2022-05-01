using UnityEngine;

[CreateAssetMenu(fileName = "Lobby Event", menuName = "Scriptable Object/Lobby Event", order = int.MaxValue)]
public class EnterLobbyEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        //ī�޶� �������� �� ������
        //�� �̸� �ؽ�Ʈ ���

        GameManager.Instance.savedData.tutorialInfo.isEnded = true;
        GameManager.Instance.savedData.userInfo.currentStageID = "Stage0-06";
    }
}
