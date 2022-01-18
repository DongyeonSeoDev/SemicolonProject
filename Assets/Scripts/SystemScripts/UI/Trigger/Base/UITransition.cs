using UnityEngine;

public abstract class UITransition : MonoBehaviour
{
    public bool transitionEnable = true;

    public abstract void Transition(bool on);

}
