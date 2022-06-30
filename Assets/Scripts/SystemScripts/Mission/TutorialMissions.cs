using UnityEngine;
using Enemy;
using System.Collections.Generic;

public class AbsorptionTutoMission : Mission
{
    private float checkTime;
    private List<Enemy.Enemy> curEnemyList = new List<Enemy.Enemy>();

    public AbsorptionTutoMission(string title) : base(title)
    {
        
    }

    public override void End(bool breakDoor = false)
    {
        StageManager.Instance.StageClear();
        KeyActionManager.Instance.GetElement(InitGainType.SKILL2);
        EnemyManager.Instance.isOnlyAbsorption = false;
    }

    public override void Start()
    {
        checkTime = Time.time + 1f;
        EnemyManager.Instance.isOnlyAbsorption = true;
        curEnemyList = EnemyManager.Instance.enemyDictionary["Stage0-05"];
    }

    public override void Update()
    {
        if(checkTime < Time.time)
        {
            checkTime = Time.time + 1f;
            for(int i=0; i<curEnemyList.Count; i++)
            {

            }
        }
    }
}
