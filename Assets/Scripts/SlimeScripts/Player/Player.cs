using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int hp = 10;
    public int Hp
    {
        get { return hp; }
    }
    [SerializeField]
    private int bodySlapDamage = 0;
    public int BodySlapDamage
    {
        get { return bodySlapDamage; }
    }
    [SerializeField]
    private int dp = 3;
    public int Dp
    {
        get { return dp; }
        set { dp = value; }
    }

    void Start()
    {

    }

    void Update()
    {

    }
    public void GetDamage(int damage)
    {
        hp -= damage;
    }
}
