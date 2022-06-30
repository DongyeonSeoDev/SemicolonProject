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
        EventManager.StopListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
    }

    public override void Start()
    {
        checkTime = Time.time + 1f;
        EnemyManager.Instance.isOnlyAbsorption = true;
        curEnemyList = EnemyManager.Instance.enemyDictionary["Stage0-05"];
        EventManager.StartListening("TryAbsorbMob", (System.Action<bool>)TryDrain);
    }

    private void TryDrain(bool suc)
    {
        if(!suc)
        {
            TalkManager.Instance.SetSubtitle("흡수는 적의 체력이 적을 때만 가능해", 0.2f, 2f);
        }
    }

    public override void Update()
    {
        if(checkTime < Time.time)
        {
            checkTime = Time.time + 1f;
            for(int i=0; i<curEnemyList.Count; i++)
            {
                curEnemyList[i].GetDamage(5, false, false, false,
                    curEnemyList[i].transform.position, curEnemyList[i].transform.position - Global.GetSlimePos.position);
            }
        }
    }
}
