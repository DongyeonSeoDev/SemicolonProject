using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Start Event", menuName = "Scriptable Object/Tutorial Start Event", order = int.MaxValue)]
public class TutorialStartEvent : MapEventSO
{
    public override void OnEnterEvent()
    {
        //UIManager.Instance.RequestLogMsg("<b>[����Ư��]</b> ���콺�� Ű���带 ȹ���Ͽ����ϴ�(?)", 4f);
    }
}
