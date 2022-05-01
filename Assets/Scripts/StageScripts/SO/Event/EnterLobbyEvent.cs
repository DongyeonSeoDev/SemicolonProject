using UnityEngine;

[CreateAssetMenu(fileName = "Lobby Event", menuName = "Scriptable Object/Lobby Event", order = int.MaxValue)]
public class EnterLobbyEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        //카메라 움직여서 맵 보여줌
      
        StageManager.Instance.SaveStage("Stage0-06");
    }
}
