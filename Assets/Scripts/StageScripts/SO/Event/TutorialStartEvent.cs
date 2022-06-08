using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Start Event", menuName = "Scriptable Object/Tutorial Start Event", order = int.MaxValue)]
public class TutorialStartEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        //UIManager.Instance.RequestLogMsg("<b>[시작특전]</b> 마우스와 키보드를 획득하였습니다(?)", 4f);
    }
}
