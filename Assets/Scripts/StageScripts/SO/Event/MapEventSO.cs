using UnityEngine;

[CreateAssetMenu(fileName = "Map Event", menuName = "Scriptable Object/Map Event", order = int.MaxValue)]
public class MapEventSO : ScriptableObject
{
    public virtual void OnEnterEvent()
    {
        StageManager.Instance.SetClearStage();
    }
}