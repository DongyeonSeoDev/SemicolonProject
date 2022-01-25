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
        set { hp = value; }
    }
    [SerializeField]
    private int bodySlapDamage = 0;
    public int BodySlapDamage
    {
        get { return bodySlapDamage; }
        set { bodySlapDamage = value; }
    }
    [SerializeField]
    private int playerProjectileDamage = 0;
    public int PlayerProjectileDamage
    {
        get { return playerProjectileDamage; }
        set { playerProjectileDamage = value; }
    }
    [SerializeField]
    private int dp = 3;
    public int Dp
    {
        get { return dp; }
        set { dp = value; }
    }
    [Header("여기서부턴 일시적인 스탯 상승 값")]
    [SerializeField]
    private int additionalHp = 0;
    public int AdditionalHp
    {
        get { return additionalHp; }
        set { additionalHp = value; }
    }
    [SerializeField]
    private int additionalBodySlapDamage = 0;
    public int AdditionalBodySlapDamage
    {
        get { return additionalBodySlapDamage; }
        set { additionalBodySlapDamage = value; }
    }
    [SerializeField]
    private int additionalPlayerProjectileDamage = 0;
    public int AdditionalPlayerProjectileDamage
    {
        get { return additionalPlayerProjectileDamage; }
        set { additionalPlayerProjectileDamage = value; }
    }
    [SerializeField]
    private int additionalDp = 0;
    public int AdditionalDp
    {
        get { return additionalDp; }
        set { additionalDp = value; }
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
