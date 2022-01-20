using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileShootPoint : MonoBehaviour
{
    private PlayerShoot playerShooot = null;
    
    [SerializeField]
    private State.MovingState shootDirection;

    private void Awake() 
    {
        playerShooot = transform.parent.transform.parent.GetComponent<PlayerShoot>();
    }
    void Start()
    {
        playerShooot.ShootPositions.Add(shootDirection, transform);
    }
}
