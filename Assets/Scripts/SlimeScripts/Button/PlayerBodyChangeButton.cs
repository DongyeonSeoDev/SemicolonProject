using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyChangeButton : MonoBehaviour
{
    private SlimeGameManager slimeGameManager = null;

    [SerializeField]
    private string changeBodyId = "NULL";

    private void Start()
    {
        slimeGameManager = SlimeGameManager.Instance;
    }
    public void BodyChange()
    {
        slimeGameManager.PlayerBodyChange(changeBodyId);
    }
}
