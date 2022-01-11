using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private SlimePoolManager slimePoolManager = null;

    private PlayerInput playerInput = null;


    [SerializeField]
    private GameObject projectile = null;
    [SerializeField]
    private Transform shootPosition = null;

    [SerializeField]
    private float projectileSpeed = 1f;

    private void Awake()
    {
        slimePoolManager = SlimePoolManager.Instance;

        playerInput = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        if(playerInput.IsShoot && playerInput.MoveVector != Vector2.zero)
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

        temp.transform.position = shootPosition.position;
        temp.GetComponent<PlayerProjectile>().OnSpawn(playerInput.MoveVector, projectileSpeed);
    }
}
