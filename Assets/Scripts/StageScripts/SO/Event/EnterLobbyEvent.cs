using UnityEngine;

[CreateAssetMenu(fileName = "Lobby Event", menuName = "Scriptable Object/Lobby Event", order = int.MaxValue)]
public class EnterLobbyEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        //ī�޶� �������� �� ������
      
        StageManager.Instance.SaveStage("Stage0-06");
    }
}
