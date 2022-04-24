using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : MonoBehaviour
{
    public Image hpBarFillImage; // 적 HP 바 채워진것중 체력 확인용 ( 없으면 UI 만들어야 함 ( Assets > Prefabs > EnemyPrefabs > EnemyUI 참고 ) )
    public Image hpBarDamageFillImage; // 적 HP 바 채워진것중 데미지 확인용

    public int maxHP = 10;
    public int hp = 10;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GetDamage()
    {

    }
    public void SetHP()
    {

    }
}
