using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoverCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SlimeGameManager.Instance.Player.CoveredObjectList.Add(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        SlimeGameManager.Instance.Player.CoveredObjectList.Remove(collision.gameObject);
      
    }
}
