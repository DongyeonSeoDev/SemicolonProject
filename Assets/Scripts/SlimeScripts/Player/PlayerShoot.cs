using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private SlimePoolManager slimePoolManager = null;

    private PlayerInput playerInput = null;
    private PlayerState playerState = null;


    [SerializeField]
    private GameObject projectile = null;

    private Dictionary<State.MovingState, Transform> shootPositions = new Dictionary<State.MovingState, Transform>();
    public Dictionary<State.MovingState, Transform> ShootPositions
    {
        get { return shootPositions; }
        set {shootPositions = value;}
    }

    [SerializeField]
    private float projectileSpeed = 1f;

    private void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;

        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
    }

    void FixedUpdate()
    {
        if (playerInput.IsShoot)
        {
            Shoot();

            playerInput.IsShoot = false;
        }
    }
    private void Shoot()
    {
        GameObject temp = null;

        bool findInDic = false;

        (temp, findInDic) = slimePoolManager.Find(projectile);

        if (findInDic)
        {
            temp.SetActive(true);
        }
        else
        {
            temp = Instantiate(projectile, transform);
        }

        temp.transform.position = shootPositions[playerState.LastPlayerMovingPoint].position;
        temp.GetComponent<PlayerProjectile>().OnSpawn(playerInput.LastMoveVector, projectileSpeed);

        SlimeEventManager.TriggerEvent("PlayerShoot");
    }
}
