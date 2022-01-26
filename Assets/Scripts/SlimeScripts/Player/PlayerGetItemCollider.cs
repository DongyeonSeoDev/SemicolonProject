using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetItemCollider : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsItem;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(whatIsItem.CompareGameObjectLayer(other.gameObject))
        {
            Item item = other.GetComponent<Item>();

            if(item != null)
            {
                Inventory.Instance.GetItem(item);
            }
        }    
    }
}
