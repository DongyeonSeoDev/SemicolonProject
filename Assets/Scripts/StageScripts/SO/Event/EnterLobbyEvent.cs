using UnityEngine;

[CreateAssetMenu(fileName = "Lobby Event", menuName = "Scriptable Object/Lobby Event", order = int.MaxValue)]
public class EnterLobbyEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        //카메라 움직여서 맵 보여줌
        //맵 이름 텍스트 띄움

        GameManager.Instance.savedData.tutorialInfo.isEnded = true;
        GameManager.Instance.savedData.userInfo.currentStageID = "Stage0-06";
    }
}
