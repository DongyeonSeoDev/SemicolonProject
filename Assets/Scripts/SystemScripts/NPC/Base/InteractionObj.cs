using UnityEngine;

public abstract class InteractionObj : MonoBehaviour
{
    [SerializeField] protected string objName;
    public string ObjName { get { return objName; } }

    public abstract void Interaction();
}
