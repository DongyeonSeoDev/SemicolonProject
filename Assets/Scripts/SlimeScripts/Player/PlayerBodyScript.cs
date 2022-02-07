using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyScript : MonoBehaviour
{
    private void OnEnable() 
    {
        SlimeGameManager.Instance.CurrentPlayerBody = gameObject;
    }
}
